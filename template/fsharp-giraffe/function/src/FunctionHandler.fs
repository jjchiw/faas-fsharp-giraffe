module Function

open FSharp.Data
open Microsoft.AspNetCore.Http
open Giraffe
open System.IO
open FSharp.Control.Tasks.V2.ContextInsensitive

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
    // clearResponse
    // >=> setStatusCode 500
    // >=>
    setStatusCode HttpStatusCodes.MethodNotAllowed next ctx
// text "Not Handled" next ctx

let Handler (next: HttpFunc) (ctx: HttpContext) =
    match ctx.Request.Method with
    | "GET" -> NotHandled next ctx
    | "POST" -> Post next ctx
    | "PUT" -> NotHandled next ctx
    | "DELETE" -> NotHandled next ctx
    | _ -> NotHandled next ctx
