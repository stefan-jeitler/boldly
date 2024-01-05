module Boldly.Domain.Versioning.UseCases.DataAccess

open Boldly.Domain.Common.Types
open Boldly.Domain.Versioning.Types

type FindVersion = VersionValue -> AsyncOption<Version>

