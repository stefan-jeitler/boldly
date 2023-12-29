module Boldly.WebApi.Features.Application.V1.ApplicationsController

open System
open System.Net.Mime
open Asp.Versioning
open Boldly.WebApi.Features.Application.V1.Dto
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc

[<ApiController>]
[<ApiVersion("1.0")>]
[<Route("api/v{version:apiVersion}/[controller]")>]
[<Produces(MediaTypeNames.Application.Json)>]
type ApplicationsController() =
    inherit ControllerBase()

    [<HttpGet(Name = "GetApplications")>]
    [<ProducesResponseType(typeof<ApplicationDto>, StatusCodes.Status200OK)>]
    member _.Get() : ActionResult =

        let dummyAccount: ApplicationDto = {
            Id = Guid.NewGuid()
            Name = "Some Account"
        }

        base.Ok(dummyAccount)