module Boldly.DataAccess.DbUpdater.DbUpdatesRunner

open Microsoft.Extensions.Configuration
open Serilog.Core

type Database =
    | Postgres = 0
    | SqlServer = 1

let runPostgresUpdates (logger: Logger) (config: IConfiguration) =
    logger.Information("Run updates against Postgres")
    ()
    
let runSqlServerUpdates (logger: Logger) (config: IConfiguration) =
    logger.Information("Run updates against SqlServer")
    ()

let runDbUpdates (logger: Logger) (config: IConfiguration) (database: Database) =
    logger.Verbose("Run Updates with verbose logging")

    match database with
    | Database.Postgres -> runPostgresUpdates logger config
    | Database.SqlServer -> runSqlServerUpdates logger config
    | _ -> logger.Error("Not Supported Database")