module Boldly.DataAccess.DbUpdater.Postgres.Updates.Uuid

open Npgsql
open Dapper

let enableUuids (dbConnection: NpgsqlConnection) =
    dbConnection.Execute("""CREATE EXTENSION IF NOT EXISTS "uuid-ossp";""") |> ignore
    