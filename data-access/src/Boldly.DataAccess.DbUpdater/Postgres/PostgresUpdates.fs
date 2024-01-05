module Boldly.DataAccess.DbUpdater.Postgres.PostgresUpdates

open Boldly.DataAccess.DbUpdater.Postgres.Updates
open Npgsql
open Semver

type PostgresDbUpdate =
    { Version: SemVersion
      Update: NpgsqlConnection -> unit }

let private v x =
    SemVersion.Parse(x, SemVersionStyles.Strict)

let private (>>>) (a: NpgsqlConnection -> unit) (b: NpgsqlConnection -> unit) =
    fun c ->
        c |> a
        c |> b

let updates: PostgresDbUpdate list =
    [ { Version = v "0.0.0"
        Update = Uuid.enableUuids >>> Functions.createGuidFunction }
      { Version = v "0.0.1"
        Update = Version.create } ]
