# OpenFaaS FSharp (dotnet 7) and Giraffe template

This template provides additional context and control over the HTTP response from your function.

## Supported platforms

-   x86_64, ARM_64

## Trying the template

```
faas-cli template pull https://github.com/houstonhaynes/faas-fsharp-giraffe
faas-cli new myfunction --lang fsharp-giraffe
```

## Example usage - fsharp-giraffe

### Success and JSON body

```fsharp
let Handler (next: HttpFunc) (ctx: HttpContext) =
    task {
        let reader = new StreamReader(ctx.Request.Body)
        let! payload = reader.ReadToEndAsync()
        let requestParameters = RequestParameters.Parse(payload)

        let result =
            sprintf "Hello %s, you are %i years old." requestParameters.Name requestParameters.Age

        return! json result next ctx
    }
```

### Custom HTTP status code

```fsharp
let Handler (next: HttpFunc) (ctx: HttpContext) =
    let result = "NotFound"
    ctx.Response.StatusCode <- HttpStatusCodes.NotAcceptable
    text result
}
```

### Failure code and plain-text body:

```fsharp
let Handler (next: HttpFunc) (ctx: HttpContext) =
    clearResponse >=> setStatusCode 500
```

### Redirect (setting Location header):

```fsharp
let Handler (next: HttpFunc) (ctx: HttpContext) =
    ctx.Response.Headers.Add("Location",  StringValues("https://google.com"))
    setStatusCode HttpStatusCodes.MovedPermanently
    >=> text "Page has moved."
```

### Path-based routing (multiple-handlers):

```fsharp
type RequestParameters = JsonProvider<"""{"name": "John Doe", "age": 31}""">

let Post (next: HttpFunc) (ctx: HttpContext) =
    task {
        let reader = new StreamReader(ctx.Request.Body)
        let! payload = reader.ReadToEndAsync()
        let requestParameters = RequestParameters.Parse(payload)

        let result =
            sprintf "Hello %s, you are %i years old." requestParameters.Name requestParameters.Age

        ctx.Response.StatusCode <- HttpStatusCodes.NotAcceptable
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

let routes : HttpHandler =
   choose [ GET >=> route "/" >=> Handler
            POST >=> route "/" >=> Handler
            PUT >=> route "/" >=> Handler
            DELETE >=> route "/" >=> Handler ]
```

Other reference:

-   [Giraffe](https://github.com/giraffe-fsharp/Giraffe)

## Example usage - giraffe

This template provides fsharp (dotnetcore 3.1) (LTS) and full access to [giraffe](https://github.com/giraffe-fsharp/Giraffe) [aspnetcore](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.1) for building microservices for [OpenFaaS](https://www.openfaas.com), Docker, Knative and Cloud Run.

With this template you can create a new microservice and deploy it to a platform like [OpenFaaS](https://www.openfaas.com) for:

-   scale-to-zero
-   horizontal scale-out
-   metrics & logs
-   automated health-checks
-   sane Kubernetes defaults like running as a non-root user

### Minimal example with one route

```fsharp
let routes : HttpHandler =
    choose [
        GET >=> route "/hello" >=> text "hello"
        RequestErrors.notFound (text "Not Found") ]
```

### Minimal example with one route and `nuget` package

```
dotnet add package Humanizer
```

```fsharp
let routes : HttpHandler =
    choose [
        GET >=> route "/" >=> text (DateTime.UtcNow.AddHours(-30.0).Humanize())
        RequestErrors.notFound (text "Not Found") ]
```

## Tests

We have a test project

https://github.com/jjchiw/faas-fsharp-giraffe/tree/master/template/fsharp-giraffe/function/tests

```fsharp
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
```
