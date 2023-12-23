namespace ChangeBlog.WebApi.Modules

#nowarn "20"

open Autofac
open Boldly.WebApi.OpenApi
open Microsoft.Extensions.Options
open Swashbuckle.AspNetCore.SwaggerGen

type RootModule() =
    inherit Module()

    override _.Load(builder) =
        builder
            .RegisterType<OpenApiDocumentsConfiguration>()
            .As<IConfigureOptions<SwaggerGenOptions>>()

        ()
