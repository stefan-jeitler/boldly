module Boldly.Domain.Adapters.Application.ApplicationAdapter

open Boldly.Domain.Versioning.PlatformAdministration.Types
open Boldly.Domain.Versioning.PlatformAdministration.DataAccess


let findApplication : LoadApplication =
    fun (appId: ApplicationId) -> failwith "not yet implemented"