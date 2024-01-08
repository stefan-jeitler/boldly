module Boldly.DataAccess.DbUpdater.Db

open System.Data
open Dapper
open Semver

module SqlServer =
    open Microsoft.Data.SqlClient

    let tableExists (dbConnection: SqlConnection) (tableName: string) =
        let tableExistsSql =
            """
                SELECT IIF(exists (SELECT NULL
                                   FROM INFORMATION_SCHEMA.TABLES
                                   WHERE TABLE_SCHEMA = 'dbo'
                                     AND TABLE_NAME = @tableName), 1, 0)
            """

        dbConnection.ExecuteScalar<bool>(tableExistsSql, {| tableName = tableName |})

    let dbName (dbConnection: IDbConnection) =
        dbConnection.ExecuteScalar<string>("SELECT DB_NAME()")

    let latestSchemaVersion (dbConnection: SqlConnection) =
        let tableExists = tableExists dbConnection "SchemaVersion"

        if not tableExists then
            None
        else
            let versionSql = "SELECT TOP 1 Version FROM SchemaVersion"
            let latestVersion = dbConnection.ExecuteScalar<string>(versionSql)

            match latestVersion with
            | null -> None
            | v -> Some(SemVersion.Parse(v, SemVersionStyles.Strict))

    let createSchemaVersionTable (dbConnection: SqlConnection) =
        let tableExists = tableExists dbConnection "SchemaVersion"

        let createSchemaVersionSql =
            """
CREATE TABLE SchemaVersion
(
    Version NVARCHAR(100) CONSTRAINT SchemaVersion_Version_NN NOT NULL,
    UpdatedAt DATETIME2 CONSTRAINT SchemaVersion_UpdatedAt_NN NOT NULL
)
"""

        if tableExists then
            ()
        else
            dbConnection.Execute(createSchemaVersionSql) |> ignore

    let updateSchemaVersion (dbConnection: SqlConnection) (version: SemVersion) =
        let updateSql =
            "UPDATE TOP (1) SchemaVersion SET Version = @version, UpdatedAt = GETUTCDATE()"

        let insertSql = "INSERT INTO SchemaVersion VALUES (@version, GETUTCDATE())"

        let parameter = {| version = version.ToString() |}

        let updated = dbConnection.Execute(updateSql, parameter)

        match updated with
        | 0 -> dbConnection.Execute(insertSql, parameter) |> ignore
        | _ -> ()

module Postgres =
    open Npgsql

    let tableExists (dbConnection: NpgsqlConnection) (tableName: string) =
        let tableExistsSql =
            """
            SELECT EXISTS (
                SELECT FROM information_schema.tables
                WHERE table_name = @tableName
            )"""

        dbConnection.ExecuteScalar<bool>(tableExistsSql, {| tableName = tableName.ToLower() |})

    let latestSchemaVersion (dbConnection: NpgsqlConnection) =
        let tableExists = tableExists dbConnection "schema_version"

        if not tableExists then
            None
        else
            let versionSql = "SELECT version FROM schema_version"
            let latestVersion = dbConnection.ExecuteScalar<string>(versionSql)

            match latestVersion with
            | null -> None
            | v -> Some(SemVersion.Parse(v, SemVersionStyles.Strict))

    let createSchemaVersionTable (dbConnection: NpgsqlConnection) =

        let createSchemaVersionSql =
            """
CREATE TABLE IF NOT EXISTS schema_version
(
    version text CONSTRAINT schemaversion_version_nn NOT NULL,
    updated_at TIMESTAMP CONSTRAINT schemaversion_updatedat_nn NOT NULL
)"""

        dbConnection.Execute(createSchemaVersionSql)

    let updateSchemaVersion (dbConnection: NpgsqlConnection) (version: SemVersion) =
        let updateSql = "UPDATE schema_version SET version = @version, updated_at = now() AT TIME ZONE 'utc'"

        let insertSql =
            "INSERT INTO schema_version (version, updated_at) VALUES(@version, now() AT TIME ZONE 'utc')"

        let parameter = {| version = version.ToString() |}

        let updated = dbConnection.Execute(updateSql, parameter)

        match updated with
        | 0 -> dbConnection.Execute(insertSql, parameter) |> ignore
        | _ -> ()


    let dbName (dbConnection: NpgsqlConnection) =
        dbConnection.ExecuteScalar<string>("SELECT current_database()")
