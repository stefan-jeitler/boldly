module Boldly.DataAccess.DbUpdater.DbUpdatesRunner

open Boldly.DataAccess.DbUpdater.Postgres
open Boldly.DataAccess.DbUpdater.SqlServer
open Microsoft.Data.SqlClient
open Npgsql
open Semver
open Serilog.Core

type DbUpdate =
    { Version: SemVersion
      Update: unit -> unit
      UpdateSchemaVersion: SemVersion -> unit }

let private runSingleUpdate (logger: Logger) dbUpdate =
    let version = dbUpdate.Version
    logger.Information("Update database to Version {version}", version.ToString())

    dbUpdate.Update ()
    dbUpdate.UpdateSchemaVersion version

let private findDuplicates (updates: DbUpdate list) =
    updates
    |> List.groupBy _.Version
    |> List.choose (function
        | v, u when u.Length > 1 -> Some v
        | _ -> None)

let runUniqueUpdates
    (logger: Logger)
    (getLatestSchemaVersion: unit -> SemVersion option)
    (updates: DbUpdate list)
    =
    let latestSchemaVersion = getLatestSchemaVersion ()
    let runSingleUpdate = runSingleUpdate logger

    let updateAlreadyApplied (u: DbUpdate) =
        match latestSchemaVersion with
        | Some l -> u.Version <= l
        | None -> false

    let executedUpdates =
        updates
        |> Seq.skipWhile updateAlreadyApplied
        |> Seq.map runSingleUpdate
        |> Seq.toList

    match executedUpdates with
    | [] -> logger.Information("No database updates to execute")
    | u ->
        let latestSchemaVersionAfterUpdate =
            getLatestSchemaVersion()
            |> Option.map _.ToString()
            |> Option.defaultValue "N/A"

        logger.Information("{count} db update(s) executed.", u.Length)
        logger.Information("Latest schema version: {version}", latestSchemaVersionAfterUpdate)

    ()

let private runUpdates
    (logger: Logger)
    (dbName: string)
    (getLatestSchemaVersion: unit -> SemVersion option)
    (updates: DbUpdate list)
    =
    let latestSchemaVersion = getLatestSchemaVersion ()
    logger.Information("Selected database: {dbName}", dbName)

    let duplicates = findDuplicates updates

    match duplicates with
    | [] ->
        match latestSchemaVersion with
        | Some v -> logger.Verbose("Latest schema version: {version}", v)
        | None -> logger.Verbose("No updates applied yet")

        runUniqueUpdates logger getLatestSchemaVersion updates
    | d ->
        logger.Error(
            "There are duplicate updates. Version(s) {duplicates}",
            (d |> List.map _.ToString() |> String.concat ", ")
        )

module Postgres =
    let runUpdates (logger: Logger) (acquireDbConnection: unit -> NpgsqlConnection) =
        use dbConnection = acquireDbConnection ()
        Db.Postgres.createSchemaVersionTable dbConnection |> ignore

        let updates =
            PostgresUpdates.updates
            |> List.map (fun x ->
                { Version = x.Version
                  Update = fun () -> x.Update dbConnection
                  UpdateSchemaVersion = Db.Postgres.updateSchemaVersion dbConnection })

        let dbName = Db.Postgres.dbName dbConnection
        let getLatestSchemaVersion () = Db.Postgres.latestSchemaVersion dbConnection

        logger.Verbose("Run updates against {0}", "Postgres")
        runUpdates logger dbName getLatestSchemaVersion updates

module SqlServer =
    let runUpdates (logger: Logger) (acquireDbConnection: unit -> SqlConnection) =
        use dbConnection = acquireDbConnection ()
        Db.SqlServer.createSchemaVersionTable dbConnection |> ignore

        let updates =
            SqlServerUpdates.updates
            |> List.map (fun x ->
                { Version = x.Version
                  Update = fun () -> x.Update dbConnection
                  UpdateSchemaVersion = Db.SqlServer.updateSchemaVersion dbConnection })

        let dbName = Db.SqlServer.dbName dbConnection
        let getLatestSchemaVersion () = Db.SqlServer.latestSchemaVersion dbConnection

        logger.Verbose("Run updates against {0}", "SqlServer")
        runUpdates logger dbName getLatestSchemaVersion updates
