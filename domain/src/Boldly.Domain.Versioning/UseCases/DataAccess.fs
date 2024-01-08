module Boldly.Domain.Versioning.UseCases.DataAccess

open Boldly.Domain.Common.Types
open Boldly.Domain.Versioning.Types

module Version =
    type FindVersion = string -> AsyncOption<Version>

