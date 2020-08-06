// https://github.com/samueleresca/Blog.FSharpOnWeb/blob/master/test/Blog.FSharpWebAPI.Tests/Fixtures.fs

module Fixtures

open Xunit
open Newtonsoft.Json
open Newtonsoft.Json.Serialization

type Person = { Name: string; Age: int }


let getPerson = { Name = "Pascual"; Age = 200 }


let shouldContains actual expected = Assert.Contains(actual, expected)
let shouldEqual expected actual = Assert.Equal(expected, actual)
let shouldNotNull expected = Assert.NotNull(expected)

let serializeObject obj =
    let settings =
        JsonSerializerSettings(ContractResolver = CamelCasePropertyNamesContractResolver())

    JsonConvert.SerializeObject(getPerson, settings)
