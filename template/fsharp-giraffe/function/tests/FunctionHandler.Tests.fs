module Tests

open Xunit
open System.Net
open System.Net.Http
open System.Text
open Microsoft.AspNetCore.TestHost
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open System
open System.IO
open HttpFunc
open Fixtures
open FSharp.Control.Tasks.V2.ContextInsensitive
open Function

let createHost () =
    WebHostBuilder().UseContentRoot(Directory.GetCurrentDirectory()).UseEnvironment("Test")
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(Action<IServiceCollection> configureServices)

[<Fact>]
let ``POST / should respond hello phrase`` () =
    task {
        use server = new TestServer(createHost ())
        use client = server.CreateClient()

        use content =
            new StringContent(serializeObject (getPerson), Encoding.UTF8, "application/json")

        let! response = post client "/" content
        let! json = response |> ensureSuccess |> readText

        shouldEqual "\"Hello Pascual, you are 200 years old.\"" json
    }

[<Fact>]
let ``GET / MethodNotAllowed`` () =
    task {
        use server = new TestServer(createHost ())
        use client = server.CreateClient()
        let! response = get client "/"
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode)
    }
