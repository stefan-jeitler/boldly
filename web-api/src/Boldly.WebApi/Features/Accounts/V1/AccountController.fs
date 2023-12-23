namespace Boldly.WebApi.Features.Account.V1

open System
open System.Net.Mime
open Asp.Versioning
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Boldly.WebApi.Features.Account.V1.DTOs

[<ApiController>]
[<ApiVersion("1.0")>]
[<Route("api/v{version:apiVersion}/[controller]")>]
[<Produces(MediaTypeNames.Application.Json)>]
type AccountController() =
    inherit ControllerBase()

    [<HttpGet(Name = "GetAccount")>]
    [<ProducesResponseType(typeof<AccountDto>, StatusCodes.Status200OK)>]
    member _.Get() : ActionResult =

        let dummyAccount: AccountDto = {
            Name = "Some Account"
            CreatedAt = DateTimeOffset.UtcNow
        }

        base.Ok(dummyAccount)