open System
open System.CommandLine
open System.CommandLine.Invocation
open Boldly.DataAccess.DbUpdater
open Boldly.DataAccess.DbUpdater.DbUpdatesRunner
open Microsoft.Data.SqlClient
open Microsoft.Extensions.Configuration
open Serilog
open Serilog.Events
open Npgsql

type Database =
    | Postgres = 0
    | SqlServer = 1

let createLogger (verbose: bool) =
    let logLevel =
        if verbose then
            LogEventLevel.Verbose
        else
            LogEventLevel.Information

    LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Console(
            restrictedToMinimumLevel = logLevel,
            outputTemplate = "[{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}{Exception}"
        )
        .CreateLogger()

let verboseSwitch = Option<bool>([| "--verbose"; "-v" |], "Detailed output.")

let databaseOption =
    Option<Database>([| "--database"; "--db"; "-d" |], "Database to use", IsRequired = true)

let createDbUpdatesCommand (config: IConfiguration) =
    let handler (ctx: InvocationContext) =
        let verbose = ctx.ParseResult.GetValueForOption verboseSwitch
        let database = ctx.ParseResult.GetValueForOption databaseOption
        use logger = createLogger verbose

        try
            match database with
            | Database.Postgres ->
                Postgres.runUpdates logger (fun () -> new NpgsqlConnection(config.GetConnectionString("Postgres")))
            | Database.SqlServer ->
                SqlServer.runUpdates logger (fun () -> new SqlConnection(config.GetConnectionString("SqlServer")))
            | _ -> logger.Error("Not Supported Database")
        with ex ->
            logger.Error(ex, ex.Message)

    let runUpdatesCommand = Command("run-updates", "executes all new db updates.")

    runUpdatesCommand.AddOption(databaseOption)
    runUpdatesCommand.SetHandler handler
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
