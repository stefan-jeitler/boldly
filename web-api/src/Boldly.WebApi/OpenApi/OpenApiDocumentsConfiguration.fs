namespace Boldly.WebApi.OpenApi

open Asp.Versioning.ApiExplorer
open Microsoft.Extensions.Options
open Microsoft.OpenApi.Models
open Swashbuckle.AspNetCore.SwaggerGen
open Microsoft.Extensions.DependencyInjection

type OpenApiDocumentsConfiguration(apiVersionDescriptions: IApiVersionDescriptionProvider) =
    interface IConfigureOptions<SwaggerGenOptions> with
        member _.Configure(options: SwaggerGenOptions) =
            for description in apiVersionDescriptions.ApiVersionDescriptions do
                options.SwaggerDoc(
                    description.GroupName,
                    OpenApiInfo(
                        Title = $"Boldly.WebApi {description.ApiVersion}",
                        Version = description.ApiVersion.ToString()
                    )
                )

            ()
