module Boldly.DataAccess.DbUpdater.Postgres.Updates.Version

open Npgsql
open Dapper

let create (dbConnection: NpgsqlConnection) = 
    let createVersionSql = """
CREATE TABLE IF NOT EXISTS "version"
(
    id UUID CONSTRAINT version_id_pkey PRIMARY KEY,
    application_id UUID CONSTRAINT version_application_nn NOT NULL,
    environment_id UUID CONSTRAINT version_application_nn NOT NULL,
    "version_discriminator" TEXT CONSTRAINT version_versiondiscriminator_nn NOT NULL,
    "version_value" TEXT CONSTRAINT version_versionvalue_nn NOT NULL,
    "state_discriminator" TEXT CONSTRAINT version_versiondiscriminator_nn NOT NULL,
    "state_value" TIMESTAMP,
    name TEXT,
    created_by_user UUID CONSTRAINT version_createdbyuser_nn NOT NULL,
    created_at TIMESTAMP CONSTRAINT version_createad_nn NOT NULL
)
    """
    
    dbConnection.Execute(createVersionSql) |> ignore
