module Boldly.Domain.Versioning.UseCases.AddVersion

open System
open Boldly.Domain.Versioning.Types
open Boldly.Domain.Versioning.PlatformAdministration
open Semver

type EnvironmentModel =
    | Name of string
    | Id of Guid

type AddVersionRequestModel = {
    ApplicationId: Guid
    Value: string
    Name: string
    Release: EnvironmentModel list
}

open DataAccess

type AddVersion = AddVersionRequestModel -> VersionId
let interactor (findApplication: FindApplication) (findVersion: FindVersion) : AddVersion =
    fun requestModel ->
        
        let version: Version = {
            Id = VersionId (Guid.NewGuid())
            ApplicationId = ApplicationId (Guid.NewGuid())
            EnvironmentId = EnvironmentId (Guid.NewGuid())
            Value = VersionValue.SemanticVersion (SemVersion.Parse(requestModel.Value))
            State = VersionState.Candidate
            Name = None
            Changes = []
            CreatedAt = DateTime.UtcNow
            CreatedBy = UserId (Guid.NewGuid()) 
        }
        
        failwith "not yet implemented"