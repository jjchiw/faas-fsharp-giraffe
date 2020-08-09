module Function

open FSharp.Data
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Giraffe
open System.IO
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Hosting

type RequestParameters = JsonProvider<"""{"name": "John Doe", "age": 31}""">

let Post (next: HttpFunc) (ctx: HttpContext) =
    task {
        let reader = new StreamReader(ctx.Request.Body)
        let! payload = reader.ReadToEndAsync()
        let requestParameters = RequestParameters.Parse(payload)

        let result =
            sprintf "Hello %s, you are %i years old." requestParameters.Name requestParameters.Age

        return! json result next ctx
    }

let NotHandled (next: HttpFunc) (ctx: HttpContext) =
    setStatusCode HttpStatusCodes.MethodNotAllowed next ctx

let Handler (next: HttpFunc) (ctx: HttpContext) =
    match ctx.Request.Method with
    | "GET" -> NotHandled next ctx
    | "POST" -> Post next ctx
    | "PUT" -> NotHandled next ctx
    | "DELETE" -> NotHandled next ctx
    | _ -> NotHandled next ctx

let routes: HttpHandler =
    choose [ GET >=> route "/" >=> Handler
             POST >=> route "/" >=> Handler
             PUT >=> route "/" >=> Handler
             DELETE >=> route "/" >=> Handler ]

let configureAppConfiguration (context: WebHostBuilderContext) (config: IConfigurationBuilder) =
    config.AddJsonFile("appsettings.json", false, true)
          .AddJsonFile(sprintf "appsettings.%s.json" context.HostingEnvironment.EnvironmentName, true)
          .AddEnvironmentVariables()
    |> ignore


let configureApp (app: IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe routes

let configureServices (services: IServiceCollection) =
    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore
