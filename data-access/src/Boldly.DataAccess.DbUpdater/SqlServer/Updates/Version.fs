module Boldly.DataAccess.DbUpdater.SqlServer.Updates.Version

open Microsoft.Data.SqlClient
open Dapper

let create (dbConnection: SqlConnection) =
    let createVersionSql =
        """
IF OBJECT_ID(N'dbo.Version', N'U') IS NULL
CREATE TABLE Version
(
    Id UNIQUEIDENTIFIER CONSTRAINT Version_Id_PK PRIMARY KEY,
    ApplicationId UNIQUEIDENTIFIER CONSTRAINT Version_ApplicationId_NN NOT NULL,
    EnvironmentId UNIQUEIDENTIFIER CONSTRAINT Version_EnvironmentId_NN NOT NULL,
    VersionLabel VARCHAR(4000) CONSTRAINT Version_VersionLabel_NN NOT NULL,
    VersionValue VARCHAR(4000) CONSTRAINT Version_VersionValue_NN NOT NULL,
    State VARCHAR(4000) CONSTRAINT Version_State_NN NOT NULL,
    StateValue DATETIME2,
    Name VARCHAR(50),
    CreatedBy UNIQUEIDENTIFIER CONSTRAINT Version_CreatedBy_NN NOT NULL,
    CreatedAt DATETIME2 CONSTRAINT Version_CreatedAt_NN NOT NULL
)
"""

    dbConnection.Execute(createVersionSql) |> ignore
