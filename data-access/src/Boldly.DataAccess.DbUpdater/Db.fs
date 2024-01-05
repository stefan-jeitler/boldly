module Boldly.DataAccess.DbUpdater.Db

open System.Data
open Dapper
open Microsoft.Data.SqlClient
open Semver

module SqlServer =
    let tableExists (dbConnection: IDbConnection) (tableName: string) =
        let tableExistsSql =
            """
                SELECT IIF(exists (SELECT NULL
                                   FROM INFORMATION_SCHEMA.TABLES
                                   WHERE TABLE_SCHEMA = 'dbo'
                                     AND TABLE_NAME = @tableName), 1, 0)
            """

        let parameters = {| tableName = tableName.ToLower() |}

        dbConnection.ExecuteScalar<bool>(tableExistsSql, parameters)

    let dbName (dbConnection: IDbConnection) =
        dbConnection.ExecuteScalar<string>("SELECT DB_NAME()")

    let latestSchemaVersion (dbConnection: IDbConnection) =
        let tableExists = tableExists dbConnection "SchemaVersion"

        if not tableExists then
            None
        else
            let versionSql = "SELECT TOP 1 Version FROM SchemaVersion"
            let latestVersion = dbConnection.ExecuteScalar<string>(versionSql)

            match latestVersion with
            | null -> None
            | v -> Some(SemVersion.Parse(v, SemVersionStyles.Strict))

    let createSchemaVersionTable (dbConnection: IDbConnection) =
        let tableExists = tableExists dbConnection "SchemaVersion"

        let createSchemaVersionSql =
            """
CREATE TABLE SchemaVersion
(
    Version VARCHAR(100) CONSTRAINT SchemaVersion_Version_NN NOT NULL,
    UpdatedAt DATETIME2 CONSTRAINT SchemaVersion_UpdatedAt_NN NOT NULL
)
"""

        if tableExists then
            0
        else
            dbConnection.Execute(createSchemaVersionSql)

    let updateSchemaVersion (dbConnection: SqlConnection) (version: SemVersion) =
        let updateSql =
            "UPDATE TOP (1) SchemaVersion SET Version = @version, UpdatedAT = GETUTCDATE()"

        let insertSql = "INSERT INTO SchemaVersion VALUES (@version, GETUTCDATE())"

        let parameter = {| version = version.ToString() |}

        let updated = dbConnection.Execute(updateSql, parameter)

        match updated with
        | 0 -> dbConnection.Execute(insertSql, parameter) |> ignore
        | _ -> ()
