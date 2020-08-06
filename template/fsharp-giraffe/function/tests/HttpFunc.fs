//https://github.com/samueleresca/Blog.FSharpOnWeb/blob/master/test/Blog.FSharpWebAPI.Tests/HttpFunc.fs

module HttpFunc

open System.Net
open System.Net.Http
open Xunit

let get (client: HttpClient) (path: string) = path |> client.GetAsync

let post (client: HttpClient) (path: string) (content: HttpContent) = client.PostAsync(path, content)

let put (client: HttpClient) (path: string) (content: HttpContent) = client.PutAsync(path, content)

let delete (client: HttpClient) (path: string) = client.DeleteAsync(path)

let createRequest (method: HttpMethod) (path: string) =
    let url = "http://127.0.0.1" + path
    new HttpRequestMessage(method, url)

let ensureSuccess (response: HttpResponseMessage) =
    if not response.IsSuccessStatusCode then
        response.Content.ReadAsStringAsync()

        |> failwithf "%A"
    else
        response

let isStatus (code: HttpStatusCode) (response: HttpResponseMessage) =
    Assert.Equal(code, response.StatusCode)
    response

let isOfType (response: HttpResponseMessage) (contentType: string) =
    Assert.Equal(contentType, response.Content.Headers.ContentType.MediaType)
    response

let readText (response: HttpResponseMessage) = response.Content.ReadAsStringAsync()
