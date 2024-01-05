module Boldly.DataAccess.DbUpdater.Postgres.Updates.Functions

open Npgsql
open Dapper

let createGuidFunction (dbConnection: NpgsqlConnection) =

    let createGuidFunctionSql =
        """
CREATE OR REPLACE FUNCTION GUID() RETURNS uuid 
AS 'select uuid_generate_v4();'
LANGUAGE SQL
"""

    dbConnection.Execute(createGuidFunctionSql) |> ignore
