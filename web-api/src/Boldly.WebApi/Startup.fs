namespace Boldly.WebApi

#nowarn "20"

open Asp.Versioning
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Mvc.ApiExplorer
open Microsoft.AspNetCore.Routing
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

type Startup(configuration: IConfigurationRoot) =
    member _.ConfigureServices(services: IServiceCollection) =
        services
            .AddControllers()
            .AddJsonOptions(fun o -> o.JsonSerializerOptions.PropertyNameCaseInsensitive <- true)

        services.Configure<RouteOptions>(fun (o: RouteOptions) -> o.LowercaseUrls <- true)

        services
            .AddApiVersioning(fun o ->
                o.ApiVersionReader <- UrlSegmentApiVersionReader()
                o.DefaultApiVersion <- ApiVersion(1, 0))
            .AddMvc()
            .AddApiExplorer(fun o ->
                o.GroupNameFormat <- "'v'VVV"
                o.SubstituteApiVersionInUrl <- true)

        services.AddEndpointsApiExplorer()

        services.AddSwaggerGen(fun s ->
            let controllerName (desc: ApiDescription) =
                desc.ActionDescriptor.RouteValues["controller"]

            let actionName (desc: ApiDescription) =
                desc.ActionDescriptor.RouteValues["action"]

            s.CustomOperationIds(fun x -> $"{controllerName x}_{actionName x}"))

        ()

    member _.Configure (app: WebApplication) (lifetime: IHostApplicationLifetime) =
        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        app.UseSwagger()

        app.UseSwaggerUI(fun o ->
            for description in app.DescribeApiVersions() do
                o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName)

            o.RoutePrefix <- "")

        ()
