module Boldly.WebApi.Clients.ClientGenerator

open System
open System.IO
open System.Text
open Asp.Versioning.ApiExplorer
open Microsoft.Extensions.DependencyInjection
open NJsonSchema.CodeGeneration.TypeScript
open NSwag.CodeGeneration.OperationNameGenerators
open NSwag.CodeGeneration.TypeScript
open Serilog
open Swashbuckle.AspNetCore.Swagger
open Microsoft.OpenApi.Extensions
open Microsoft.OpenApi
open NSwag

let private tsClientGeneratorSettings apiVersionGroupName =
    let settings =
        TypeScriptClientGeneratorSettings(
            ClassName = $"{{controller}}Client{apiVersionGroupName}",
            Template = TypeScriptTemplate.Fetch,
            ExceptionClass = "ChangeBlogApiException",
            PromiseType = PromiseType.Promise,
            WrapResponses = true,
            GenerateClientInterfaces = true,
            GenerateClientClasses = true,
            GenerateDtoTypes = true
        )

    settings.TypeScriptGeneratorSettings.TypeScriptVersion <- 5.1m
    settings.TypeScriptGeneratorSettings.EnumStyle <- TypeScriptEnumStyle.StringLiteral
    settings.TypeScriptGeneratorSettings.DateTimeType <- TypeScriptDateTimeType.Date
    settings.OperationNameGenerator <- MultipleClientsFromOperationIdOperationNameGenerator()

    settings

let generateTsClient (services: IServiceProvider) (file: FileInfo) =
    let logger = services.GetRequiredService<ILogger>()
    logger.Information("Start generating TS Clients")

    let swaggerProvider = services.GetRequiredService<ISwaggerProvider>()

    let apiVersionDescriptions =
        services.GetRequiredService<IApiVersionDescriptionProvider>()

    let generateClient (description: ApiVersionDescription) =
        let apiVersionGroupName = description.ApiVersion.ToString("'v'VVV")
        logger.Information("Generate client for version {version}", apiVersionGroupName)
        let document = swaggerProvider.GetSwagger(apiVersionGroupName, null, "/")
        let swaggerFile = document.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0)

        task {
            let! nswagDocument = OpenApiDocument.FromJsonAsync(swaggerFile)

            let settings = tsClientGeneratorSettings (apiVersionGroupName.ToUpper())

            let generator = TypeScriptClientGenerator(nswagDocument, settings)
            let tsClient = generator.GenerateFile()

            let fileNameWithVersion =
                Path.ChangeExtension(file.Name, $"{apiVersionGroupName.ToUpper()}.ts")

            let finalFileName = Path.Combine(file.Directory.FullName, fileNameWithVersion)

            do! File.WriteAllTextAsync(finalFileName, tsClient, Encoding.UTF8)
            logger.Information("Client for version {version} successfully created", apiVersionGroupName)
            logger.Information("Output File: {file}", finalFileName)
        }

    task {
        for description in apiVersionDescriptions.ApiVersionDescriptions do
            do! generateClient description
    }
