﻿module Boldly.DataAccess.DbUpdater.SqlServer.SqlServerUpdates

open Boldly.DataAccess.DbUpdater.SqlServer.Updates
open Microsoft.Data.SqlClient
open Semver

type SqlServerDbUpdate =
    { Version: SemVersion
      Update: SqlConnection -> unit }

let private v x =
    SemVersion.Parse(x, SemVersionStyles.Strict)

let updates: SqlServerDbUpdate list =
    [ { Version = v "0.0.0"
        Update = Version.create } ]
