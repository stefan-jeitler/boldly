module Boldly.DataAccess.DbUpdater.DbUpdatesAnalysis

open Boldly.DataAccess.DbUpdater.Postgres
open Boldly.DataAccess.DbUpdater.SqlServer
open Semver
open Serilog.Core

let containsBreakingChanges (logger: Logger) (dbName: string) (updates: SemVersion list) (latestSchemaVersion: SemVersion) =
    let latestDbUpdatesVersion =
        updates
        |> List.max

    logger.Information("Selected database: {dbName}", dbName)
    logger.Verbose("Latest db version: {dbVersion}", latestSchemaVersion.ToString())
    logger.Verbose("Latest update version: {updatesVersion}", latestDbUpdatesVersion.ToString())
    
    if latestSchemaVersion.Major <> latestDbUpdatesVersion.Major then
        logger.Information("Breaking changes detected.")
        true
    else
        logger.Information("There are no breaking changes.")
        false

module Postgres =
    open Npgsql

    let containsBreakingChanges (logger: Logger) (acquireDbConnection: unit -> NpgsqlConnection) =
        let dbConnection = acquireDbConnection ()
        let dbName = Db.Postgres.dbName dbConnection
        let latestSchemaVersion = Db.Postgres.latestSchemaVersion dbConnection

        logger.Verbose("Check {0} for breaking changes", "Postgres")
        let updates =
            PostgresUpdates.updates
            |> List.map _.Version
        
        match latestSchemaVersion, updates with
        | None, [] ->
            logger.Information("No updates applied yet and no updates to apply")
            false
        | None, _ ->
            logger.Information("No updates applied yet")
            true
        | Some lsv, u -> containsBreakingChanges logger dbName u lsv

module SqlServer =
    open Microsoft.Data.SqlClient

    let containsBreakingChanges (logger: Logger) (acquireDbConnection: unit -> SqlConnection) =
        let dbConnection = acquireDbConnection ()
        let dbName = Db.SqlServer.dbName dbConnection
        let latestSchemaVersion = Db.SqlServer.latestSchemaVersion dbConnection

        logger.Verbose("Check {0} for breaking changes", "Postgres")
        let updates =
            SqlServerUpdates.updates
            |> List.map _.Version
        
        match latestSchemaVersion, updates with
        | None, [] ->
            logger.Information("No updates applied yet and no updates to apply")
            false
        | None, _ ->
            logger.Information("No updates applied yet")
            true
        | Some lsv, u -> containsBreakingChanges logger dbName u lsv
    