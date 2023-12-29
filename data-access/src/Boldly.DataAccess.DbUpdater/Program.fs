open System
open System.CommandLine
open System.CommandLine.Invocation
open Boldly.DataAccess.DbUpdater
open Microsoft.Extensions.Configuration
open Serilog
open Serilog.Events

let createLogger (verbose: bool) =
    let logLevel =
        if verbose then
            LogEventLevel.Verbose
        else
            LogEventLevel.Information

    LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Console(restrictedToMinimumLevel = logLevel)
        .CreateLogger()

let verboseSwitch = Option<bool>([| "--verbose"; "-v" |], "Detailed output.")
let databaseOption = Option<DbUpdatesRunner.Database>([|"--database"; "--db"; "-d"|], "Database to use", IsRequired = true)

let createDbUpdatesCommand (config: IConfiguration) =
    let handler (ctx: InvocationContext) =
        let verbose = ctx.ParseResult.GetValueForOption verboseSwitch
        let database = ctx.ParseResult.GetValueForOption databaseOption
        use logger = createLogger verbose
        try
            DbUpdatesRunner.runDbUpdates logger config database
        with
        | ex -> logger.Error(ex.Message)

    let runUpdatesCommand =
        Command("run-updates", "executes all new db updates.")

    runUpdatesCommand.AddOption(databaseOption)
    runUpdatesCommand.SetHandler (fun (ctx: InvocationContext) -> handler ctx)
    runUpdatesCommand


[<EntryPoint>]
let main args =

    let config =
        ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddUserSecrets("Boldly.DataAccess.DbUpdater")
            .AddEnvironmentVariables()
            .Build()

    let rootCommand = RootCommand()
    rootCommand.AddGlobalOption(verboseSwitch)

    rootCommand.Add(createDbUpdatesCommand config)

    rootCommand.Invoke args
