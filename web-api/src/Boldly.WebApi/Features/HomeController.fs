namespace ChangeBlog.WebApi.Features

open System.Net.Mime
open System.Reflection
open Asp.Versioning
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Hosting

type ApiInfo =
    { Name: string
      Version: string
      Environment: string }


[<ApiController>]
[<Route("api")>]
[<ApiVersionNeutral>]
[<Produces(MediaTypeNames.Application.Json)>]
type HomeController(hostEnvironment: IHostEnvironment) =
    inherit ControllerBase()

    let apiVersion =
        lazy
            (let executingAssembly = Assembly.GetExecutingAssembly()

             let informationalVersion =
                 executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()

             let assemblyVersion = executingAssembly.GetName().Version.ToString()

             if informationalVersion <> null then
                 informationalVersion.InformationalVersion
             else
                 assemblyVersion)

    [<HttpGet("info", Name = "GetApiInfo")>]
    [<ProducesResponseType(typeof<ApiInfo>, StatusCodes.Status200OK)>]
    [<AllowAnonymous>]
    member _.Get() : ActionResult =
        let apiInfo =
            { Name = hostEnvironment.ApplicationName
              Version = apiVersion.Value
              Environment = hostEnvironment.EnvironmentName }

        base.Ok(apiInfo)

