module Boldly.Domain.Versioning.PlatformAdministration

open System
open Boldly.Domain.Common

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
