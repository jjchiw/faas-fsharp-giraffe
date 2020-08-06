module FunctionServer

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Function


let webApp =
    choose [ GET >=> route "/" >=> Function.Handler
             POST >=> route "/" >=> Function.Handler
             PUT >=> route "/" >=> Function.Handler
             DELETE >=> route "/" >=> Function.Handler ]

let configureApp (app: IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe webApp

let configureServices (services: IServiceCollection) =
    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun webHostBuilder ->
        webHostBuilder.Configure(configureApp).ConfigureServices(configureServices)
        |> ignore).Build().Run()
    0

// module root

// open System
// open System.Text
// open Function

// [<EntryPoint>]
// let main argv =
//     let buffer = StringBuilder()

//     let rec readInput (sb:StringBuilder) =
//         match Console.ReadLine() with
//         | null -> sb.ToString()
//         | str ->
//             sb.AppendLine(str) |> ignore
//             readInput sb

//     let input = readInput buffer
//     Function.Handle input

//     0
