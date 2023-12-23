module Boldly.Domain.Versioning.Types

open System
open Boldly.Domain.Common.Types
open Boldly.Domain.Common

type VersionId = VersionId of Guid
type ProductId = ProductId of Guid
type UserId = UserId of Guid

type VersionValue = Undefined

type VersionState =
    | Candidate
    | Deleted of DateTime
    | Released of DateTime

[<NoEquality; NoComparison>]
type Version =
    { Id: VersionId
      ProductId: ProductId
      Value: VersionValue
      Name: Name option
      CreatedBy: UserId
      CreatedAt: DateTime
      State: VersionState }
