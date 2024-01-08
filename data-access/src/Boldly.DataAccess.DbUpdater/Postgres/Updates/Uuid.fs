module Boldly.DataAccess.DbUpdater.Postgres.Updates.Uuid

open Npgsql
open Dapper

let enableOsspExtension (dbConnection: NpgsqlConnection) =
    dbConnection.Execute("""CREATE EXTENSION IF NOT EXISTS "uuid-ossp";""") |> ignore
    