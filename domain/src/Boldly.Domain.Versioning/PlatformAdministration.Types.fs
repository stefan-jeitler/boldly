module Boldly.Domain.Versioning.PlatformAdministration

open System
open Boldly.Domain.Common
open Boldly.Domain.Common.Types

type ApplicationId = ApplicationId of Guid
type EnvironmentId = EnvironmentId of Guid
type UserId = UserId of Guid

[<NoEquality; NoComparison>]
type Environment = {
    Id: EnvironmentId
    Name: Name
}

[<NoEquality; NoComparison>]
type Application = {
    Id: ApplicationId
    Name: Name
    Environments: Environment list
}

type FindApplication = ApplicationId -> AsyncResult<Application, Undefined>

