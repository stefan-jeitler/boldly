module Boldly.DataAccess.DbUpdater.DbUpdatesRunner

open Boldly.DataAccess.DbUpdater.Postgres
open Boldly.DataAccess.DbUpdater.SqlServer
open Microsoft.Data.SqlClient
open Npgsql
open Semver
open Serilog.Core

type DbUpdate<'a when 'a :> System.Data.IDbConnection> =
    { Version: SemVersion
      Update: 'a -> unit
      UpdateSchemaVersion: 'a -> SemVersion -> unit }

let private someOrDefault d =
    function
    | Some x -> x
    | None -> d

let private executeUpdate (logger: Logger) dbConnection dbUpdate =
    let version = dbUpdate.Version
    logger.Information("Update database to Version {version}", version.ToString())

    dbUpdate.Update dbConnection
    dbUpdate.UpdateSchemaVersion dbConnection version

let private findDuplicates (updates: DbUpdate<_> list) =
    updates
    |> List.groupBy _.Version
    |> List.choose (function
        | v, u when u.Length > 1 -> Some v
        | _ -> None)

let private runUniqueUpdates
    (logger: Logger)
    (dbConnection: 'a)
    (dbName: string)
    (getLatestSchemaVersion: 'a -> SemVersion option)
    (updates: DbUpdate<'a> list)
    =
    let latestSchemaVersion = getLatestSchemaVersion dbConnection
    logger.Information("Selected database: {dbName}", dbName)

    match latestSchemaVersion with
    | Some v -> logger.Verbose("Latest schema version: {version}", v)
    | None -> logger.Verbose("No updates applied yet")

    let runSingleUpdate = executeUpdate logger dbConnection
    let updateAlreadyApplied (u: DbUpdate<_>) =
        match latestSchemaVersion with
        | Some l -> u.Version <= l
        | None -> false

    let executedUpdates =
        updates |> Seq.skipWhile updateAlreadyApplied |> Seq.map runSingleUpdate |> Seq.toList

    match executedUpdates with
    | [] -> logger.Information("No database updates to execute")
    | u ->
        let latestSchemaVersionAfterUpdate =
            dbConnection
            |> getLatestSchemaVersion
            |> Option.map _.ToString()
            |> Option.defaultValue "N/A"

        logger.Information("{count} db update(s) executed.", u.Length)
        logger.Information("Latest schema version: {version}", latestSchemaVersionAfterUpdate)

    ()

module Postgres =
    let runUpdates (logger: Logger) (acquireDbConnection: unit -> NpgsqlConnection) =
        use dbConnection = acquireDbConnection ()
        Db.Postgres.createSchemaVersionTable dbConnection |> ignore

        let updates =
            PostgresUpdates.updates
            |> List.map (fun x ->
                { Version = x.Version
                  Update = x.Update
                  UpdateSchemaVersion = Db.Postgres.updateSchemaVersion })

        let dbName = Db.Postgres.dbName dbConnection

        let runUniquePostgresUpdates =
            runUniqueUpdates logger dbConnection dbName Db.Postgres.latestSchemaVersion

        let duplicates = findDuplicates updates 
        match duplicates with
        | [] -> runUniquePostgresUpdates updates
        | d ->
            logger.Error(
                "There are duplicate updates. Version(s) {duplicates}",
                (d |> List.map _.ToString() |> String.concat ", ")
            )

module SqlServer =
    let runUpdates (logger: Logger) (acquireDbConnection: unit -> SqlConnection) =
        use dbConnection = acquireDbConnection ()
        Db.SqlServer.createSchemaVersionTable dbConnection |> ignore

        let updates =
            SqlServerUpdates.updates
            |> List.map (fun x ->
                { Version = x.Version
                  Update = x.Update
                  UpdateSchemaVersion = Db.SqlServer.updateSchemaVersion })

        let dbName = Db.SqlServer.dbName dbConnection

        let runUniqueSqlServerUpdates =
            runUniqueUpdates logger dbConnection dbName Db.SqlServer.latestSchemaVersion

        let duplicates = findDuplicates updates
        match duplicates with
        | [] -> runUniqueSqlServerUpdates updates
        | d ->
            logger.Error(
                "There are duplicate updates. Version(s) {duplicates}",
                (d |> List.map _.ToString() |> String.concat ", ")
            )
