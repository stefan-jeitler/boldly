module Boldly.Domain.Tests.Versioning.TechnicalNameTests

open System.Globalization
open System.Threading
open Boldly.Domain.Common
open Boldly.Domain.Tests
open Xunit
open Boldly.Domain.Versioning.TechnicalName
open System

[<Fact>]
let ``Valid Technical Name (Happy Path)`` () =
    let technicalName = technicalName "FooBar"
    
    match technicalName with
    | Ok tn -> Assert.Equal("FooBar", WrappedString.value tn)
    | Error e -> ResultAssert.errorNotExpected e

[<Fact>]
let ``A technical name with just one character is not a valid name, it needs at least two characters`` () =
    let name = technicalName "a"
    let expected = "The length must be at least 2 characters"
    
    match name with
    | Error e -> Assert.Equal(expected, e)
    | Ok n -> ResultAssert.okNotExpected n

[<Fact>]
let ``A technical name with two characters is Ok`` () =
    let name = technicalName "QA"
    
    match name with
    | Ok n -> Assert.Equal("QA", (WrappedString.value n))
    | Error e -> ResultAssert.errorNotExpected e

[<Fact>]
let ``A technical name with more than 50 characters is too long`` () =
    let tooLongName = technicalName (String('a', 51))
    let expected = "The length must be 50 characters or fewer"

    match tooLongName with
    | Ok n -> ResultAssert.errorNotExpected n
    | Error e -> Assert.Equal(expected, e)

[<Fact>]
let ``Null is not a valid value for a technical name`` () =
    let name = technicalName null
    let expected = "The technical name must not be empty"
    
    match name with
    | Ok n -> ResultAssert.okNotExpected n
    | Error e -> Assert.Equal(expected, e) 

[<Fact>]
let ``An empty technical name is invalid, it needs at leads 2 characters`` () =
    let name = technicalName ""
    let expected = "The length must be at least 2 characters"
    
    match name with
    | Ok n -> ResultAssert.okNotExpected n
    | Error e -> Assert.Equal(expected, e)

[<Fact>]
let ``Whitespaces in technical names are not allowed`` () = 
    let technicalName = technicalName "Bug Fix"
    let expectedResult = "Whitespaces are not allowed"

    match technicalName with
    | Ok n -> ResultAssert.okNotExpected n
    | Error e -> Assert.Equal(expectedResult, e)

[<Fact>]
let ``An invalid name in a different ui culture returns a localized error message`` () =
    Thread.CurrentThread.CurrentUICulture <- CultureInfo("de")
    let name = technicalName ""
    let expected = "Die Länge muss größer oder gleich 2 sein"
    
    match name with
    | Ok n -> ResultAssert.okNotExpected n
    | Error e -> Assert.Equal(expected, e)

[<Fact>]
let ``Technical names have structural equality`` () = 
    let tn1 = technicalName "ab"
    let tn2 = technicalName "ab"
    
    match tn1, tn2 with
    | Ok left, Ok right -> Assert.True((left = right))
    | Error error, _ | _, Error error -> ResultAssert.errorNotExpected error

[<Fact>]
let ``Leading and trailing whitespaces get removed`` () = 
    let technicalNameWithLeadingAndTrailingWhitespace = technicalName " ABC "
    
    match technicalNameWithLeadingAndTrailingWhitespace with
    | Ok tn -> Assert.Equal("ABC", WrappedString.value tn)
    | Error e -> ResultAssert.errorNotExpected e 
