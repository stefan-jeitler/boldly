module Boldly.DataAccess.DbUpdater.Postgres.Updates.Uuid

open Npgsql
open Dapper

let enableUuids (dbConnection: NpgsqlConnection) =
    let sql = """CREATE EXTENSION IF NOT EXISTS "uuid-ossp";"""

    dbConnection.Execute(sql) |> ignore