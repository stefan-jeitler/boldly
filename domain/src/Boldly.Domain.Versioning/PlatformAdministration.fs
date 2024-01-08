namespace Boldly.Domain.Versioning.PlatformAdministration

open System
open Boldly.Domain.Common
open Boldly.Domain.Common.Types

module Types =
    type ApplicationId = ApplicationId of Guid
    type EnvironmentId = EnvironmentId of Guid
    type UserId = UserId of Guid
    
    type VersionType =
    | SemanticVersion
    | UserDefinedVersioningScheme of RegExPattern : string
    | FreeValue

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
        VersioningScheme: VersionType
    }

module DataAccess =
    open Types

    type LoadApplicationError =
        | NotFound of ApplicationId

    type LoadApplication = ApplicationId -> AsyncResult<Application, LoadApplicationError>
