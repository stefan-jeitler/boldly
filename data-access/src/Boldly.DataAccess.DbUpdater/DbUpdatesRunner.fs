module Boldly.DataAccess.DbUpdater.DbUpdatesRunner

open Boldly.DataAccess.DbUpdater.Postgres
open Boldly.DataAccess.DbUpdater.SqlServer
open Microsoft.Data.SqlClient
open Npgsql
open Serilog.Core

let private findDuplicates (updates: 'a list) (selector: 'a -> 'b) =
    updates
    |> List.groupBy selector
    |> List.choose (function
        | v, u when u.Length > 1 -> Some v
        | _ -> None)

let private someOrDefault d =
    function
    | Some x -> x
    | None -> d

module SqlServer =
    let private executeUpdate (logger: Logger) (dbConnection: SqlConnection) (dbUpdate: SqlServerUpdates.DbUpdate) =
        let version = dbUpdate.Version
        logger.Information("Update database to Version {version}", version.ToString())
        dbUpdate.Update dbConnection
        Db.SqlServer.updateSchemaVersion dbConnection version

    let private runUniqueSqlServerUpdates
        (logger: Logger)
        (dbConnection: SqlConnection)
        (updates: SqlServerUpdates.DbUpdate list)
        =
        let dbName = Db.SqlServer.dbName dbConnection
        let latestSchemaVersion = Db.SqlServer.latestSchemaVersion dbConnection

        logger.Information("Selected database: {dbName}", dbName)

        match latestSchemaVersion with
        | Some v -> logger.Verbose("Latest schema version: {version}", v)
        | None -> logger.Verbose("No updates applied yet")

        let runUpdate = executeUpdate logger dbConnection

        let updateAlreadyApplied (u: SqlServerUpdates.DbUpdate) =
            match latestSchemaVersion with
            | Some l -> u.Version <= l
            | None -> false

        let executedUpdates =
            updates |> Seq.skipWhile updateAlreadyApplied |> Seq.map runUpdate |> Seq.toList

        match executedUpdates with
        | [] -> logger.Information("No database updates to execute")
        | u ->
            let latestSchemaVersionAfterUpdate =
                dbConnection
                |> Db.SqlServer.latestSchemaVersion
                |> Option.map _.ToString()
                |> someOrDefault "N/A"

            logger.Information("{count} db update(s) executed.", u.Length)
            logger.Information("Latest schema version: {version}", latestSchemaVersionAfterUpdate)

        ()

    let runUpdates (logger: Logger) (acquireDbConnection: unit -> SqlConnection) =
        use dbConnection = acquireDbConnection ()
        Db.SqlServer.createSchemaVersionTable dbConnection |> ignore

        let duplicates = findDuplicates SqlServerUpdates.updates _.Version

        match duplicates with
        | [] -> runUniqueSqlServerUpdates logger dbConnection SqlServerUpdates.updates
        | d ->
            logger.Error(
                "There are duplicate updates. Version(s) {duplicates}",
                (d |> List.map _.ToString() |> String.concat ", ")
            )

module Postgres =
    let runUpdates (logger: Logger) (acquireDbConnection: unit -> NpgsqlConnection) =
        logger.Information("Run updates against Postgres")

        let duplicates = findDuplicates PostgresUpdates.updates _.Version
        ()
