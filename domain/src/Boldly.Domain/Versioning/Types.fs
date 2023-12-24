module Boldly.Domain.Versioning.Types

open System
open Boldly.Domain.Common
open Semver

type VersionId = VersionId of Guid
type ProductId = ProductId of Guid
type UserId = UserId of Guid
type EnvironmentId = EnvironmentId of Guid

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
    | Pending
    | Assigned of VersionId

// based on: https://keepachangelog.com/en/1.0.0/
type DefaultChangeType =
    | Added
    | Changed
    | Deprecated
    | Fixed
    | Security

type ChangeTypes =
    | Default of DefaultChangeType list
    | UserDefined of TechnicalName list
    | FreeText of TechnicalName list

[<NoComparison; NoEquality>]
type Issue = {
    Id: TechnicalName
    Link: Uri option
}

[<NoEquality; NoComparison>]
type Change = {
    Id: ChangeId
    ProductId: ProductId
    State: ChangeState
    Text: Text
    Types: ChangeTypes
    Issues: Issue list
    CreatedBy: UserId
    CreatedAt: DateTime
}

[<NoEquality; NoComparison>]
type Version =
    { Id: VersionId
      ProductId: ProductId
      EnvironmentId: EnvironmentId
      Value: VersionValue
      State: VersionState
      Name: Name option
      Changes: Change list
      CreatedBy: UserId
      CreatedAt: DateTime }

