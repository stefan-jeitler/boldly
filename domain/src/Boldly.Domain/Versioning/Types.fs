module Boldly.Domain.Versioning.Types

open System
open Boldly.Domain.Common
open Boldly.Domain.Common.Types
open Semver

type VersionId = VersionId of Guid
type ProductId = ProductId of Guid
type UserId = UserId of Guid

type VersionValue =
    | SemanticVersion of SemVersion
    | UserDefinedVersioningScheme of Name
    | FreeValue of Name

type VersionState =
    | Candidate
    | Deleted of DateTime
    | Released of DateTime

[<NoEquality; NoComparison>]
type Version =
    { Id: VersionId
      ProductId: ProductId
      Environment: Name
      Value: VersionValue
      State: VersionState
      Name: Name option
      CreatedBy: UserId
      CreatedAt: DateTime }

type ChangeId = ChangeId of Guid

type ChangeState =
    | Pending
    | Assigned of VersionId

type DefaultChangeType =
    | Feature
    | Bugfix
    | Security
    | Removed
    | Docs

type ChangeTypes =
    | Default of NonEmptySet<DefaultChangeType>
    | UserDefined of NonEmptySet<TechnicalName>
    | FreeText of NonEmptySet<TechnicalName>

[<NoComparison; NoEquality>]
type Issue = {
    Id: string
    Link: Uri option
}

type Change = {
    Id: ChangeId
    ProductId: ProductId
    State: ChangeState
    Text: Text
    Types: ChangeTypes
    Issues: Issue list
    Index: uint
    CreatedBy: UserId
    CreatedAt: DateTime
}

