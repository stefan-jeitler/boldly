namespace Boldly.WebApi

#nowarn "20"

open System.CommandLine
open System.IO
open System.Reflection
open System.Threading.Tasks
open Autofac
open Autofac.Extensions.DependencyInjection
open Boldly.WebApi.Clients
open Microsoft.AspNetCore.Builder
open Serilog

module Program =
    let run (args: string array) (command: WebApplication -> Task) =
        let builder = WebApplication.CreateBuilder(args)
        let startup = Startup(builder.Configuration)

        builder.Host.UseServiceProviderFactory(AutofacServiceProviderFactory())

        builder.Host.UseSerilog(fun context services configuration ->
            configuration.ReadFrom
                .Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()

            ())

        startup.ConfigureServices builder.Services

        builder.Host.ConfigureContainer<ContainerBuilder>(fun ctx b ->
            b.RegisterAssemblyModules(Assembly.GetExecutingAssembly()) |> ignore)

        let app = builder.Build()
        startup.Configure app app.Lifetime

        command app

    let createTsClientCommand args =
        let command =
            Command("generate-ts-client", "Generates a TS Client for the api's endpoints")

        let fileOption = Option<FileInfo>("--file", "Output file")

        let generateClient file =
            run args (fun app -> ClientGenerator.generateTsClient app.Services file)

        command.AddOption(fileOption)
        command.SetHandler(generateClient, fileOption)
        command

    [<EntryPoint>]
    let main args =
        let rootCommand = RootCommand("Runs the Web Api")
        let runApp () = run args _.RunAsync()
        rootCommand.SetHandler(runApp)

        let generateTsClient = createTsClientCommand args
        rootCommand.AddCommand(generateTsClient)

        rootCommand.InvokeAsync(args) |> Async.AwaitTask |> Async.RunSynchronously
