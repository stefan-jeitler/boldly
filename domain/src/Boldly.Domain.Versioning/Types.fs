module Boldly.Domain.Versioning.Types

open System
open Boldly.Domain.Common
open Boldly.Domain.Versioning.PlatformAdministration.Types
open Semver

type VersionId = VersionId of Guid
type VersionValue =
    | SemanticVersion of SemVersion
    | UserDefinedVersioningScheme of Text
    | FreeValue of Text

type VersionState =
    | Candidate
    | Deleted of DateTime
    | Released of DateTime

type ChangeId = ChangeId of Guid

type ChangeState =
    | Pending of ApplicationId
    | Assigned of VersionId

// based on: https://keepachangelog.com/en/1.0.0/
type DefaultChangeType =
    | Added
    | Changed
    | Deprecated
    | Removed
    | Fixed
    | Security

type ChangeTypes =
    | Default of DefaultChangeType list
    | UserDefined of Name list
    | FreeText of Name list

[<NoComparison; NoEquality>]
type Issue = {
    Id: TechnicalName
    Link: Uri option
}

[<NoEquality; NoComparison>]
type Change = {
    Id: ChangeId
    State: ChangeState
    Text: Text
    Types: ChangeTypes
    Issues: Issue list
    CreatedBy: UserId
    CreatedAt: DateTime
}

[<NoEquality; NoComparison>]
type Version = {
      Id: VersionId
      Application: Application
      ReleasedTo: Environment list
      Value: VersionValue
      State: VersionState
      Name: Name option
      Changes: Change list
      CreatedBy: UserId
      CreatedAt: DateTime
}