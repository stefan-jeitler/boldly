module Boldly.Domain.Tests.Versioning.TextTests

open System
open Boldly.Domain.Common
open Boldly.Domain.Tests
open Xunit
open Boldly.Domain.Versioning.Text

[<Fact>]
let ``A text with more than 4000 characters is not allowed`` () = 
    let tooLongText = text (String('a', 4001))
    let expectedResult = "The length must be 4000 characters or fewer"
    
    match tooLongText with
    | Ok t -> ResultAssert.okNotExpected t
    | Error e -> Assert.Equal(expectedResult, e)
    
[<Fact>]
let ``Valid Text (Happy Path)`` () = 
    let text = text "Lorem Ipsum"
    
    match text with
    | Ok t -> Assert.Equal("Lorem Ipsum", WrappedString.value t)
    | Error e -> ResultAssert.errorNotExpected e

[<Fact>]
let ``An empty string is allowed`` () = 
    let text = text ""
    
    match text with
    | Ok t -> Assert.Equal("", WrappedString.value t)
    | Error e -> ResultAssert.errorNotExpected e
    
[<Fact>]
let ``Control characters are also allowed`` () = 
    let text = text "Lorem\tIpsum"
    let expected = "Lorem\tIpsum"
    
    match text with
    | Ok t -> Assert.Equal(expected, WrappedString.value t)
    | Error e -> ResultAssert.errorNotExpected e