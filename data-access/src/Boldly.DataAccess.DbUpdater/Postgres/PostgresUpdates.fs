module Boldly.DataAccess.DbUpdater.Postgres.PostgresUpdates

open Boldly.DataAccess.DbUpdater.Postgres.Updates
open Npgsql
open Semver

type DbUpdate =
    { Version: SemVersion
      Update: NpgsqlConnection -> unit }

let private v x =
    SemVersion.Parse(x, SemVersionStyles.Strict)

let updates: DbUpdate list = [
    { Version = v "0.0.0"; Update = Uuid.enableUuids }
]
