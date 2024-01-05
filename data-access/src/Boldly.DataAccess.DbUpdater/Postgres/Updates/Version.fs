module Boldly.DataAccess.DbUpdater.Postgres.Updates.Version

open Npgsql

let create (dbConnection: NpgsqlConnection) = 
    let createVersionSql = """
CREATE TABLE IF NOT EXISTS "version"
(
	id UUID CONSTRAINT version_id_pkey PRIMARY KEY,
	application_id UUID CONSTRAINT version_applicationid_nn NOT NULL,
)
    """    
    ()