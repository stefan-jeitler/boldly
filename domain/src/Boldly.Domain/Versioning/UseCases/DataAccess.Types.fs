module Boldly.Domain.Versioning.UseCases.DataAccess

open Boldly.Domain.Common.Types
open Boldly.Domain.Versioning.AccountAdministration
open Boldly.Domain.Versioning.Types

type FindVersion = VersionValue -> AsyncOption<Version>

// different Bounded Context
type FindApplication = ApplicationId -> AsyncResult<Application, Undefined>