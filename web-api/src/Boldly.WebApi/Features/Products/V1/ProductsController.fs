namespace Boldly.WebApi.Features.Products.V1

open System
open System.Net.Mime
open Asp.Versioning
open Boldly.WebApi.Features.Products.V1.DTOs
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc

[<ApiController>]
[<ApiVersion("1.0")>]
[<Route("api/v{version:apiVersion}/[controller]")>]
[<Produces(MediaTypeNames.Application.Json)>]
type ProductsController() =
    inherit ControllerBase()

    [<HttpGet(Name = "GetProduct")>]
    [<ProducesResponseType(typeof<ProductDto>, StatusCodes.Status200OK)>]
    member _.Get() : ActionResult =

        let dummyProduct: ProductDto =
            { Name = "Some Product"
              CreatedAt = DateTimeOffset.UtcNow }

        base.Ok(dummyProduct)
