module Boldly.Domain.Tests.AccountAdministration.EmailTests

open System.Globalization
open System.Threading
open Boldly.Domain.Common
open Boldly.Domain.PlatformAdministration.Email
open Boldly.Domain.Tests
open Xunit

[<Fact>]
let ``Valid email address (Happy Path)`` () =
    let email = email "stefan@boldly.com"
    
    match email with
    | Ok e -> Assert.Equal("stefan@boldly.com", WrappedString.value e)
    | Error e -> ResultAssert.errorNotExpected e

[<Theory>]
[<InlineData("stefan@boldly.com ")>]
[<InlineData(" stefan@boldly.com")>]
[<InlineData(" stefan@boldly.com ")>]
let ``Leading and trailing whitespaces in email addresses will be removed`` emailAddress =
    let email = email emailAddress
    let expected = "stefan@boldly.com"

    match email with
    | Ok n -> Assert.Equal(expected, WrappedString.value n)
    | Error e -> ResultAssert.errorNotExpected e

[<Theory>]
[<InlineData("stefanAtboldly")>]
[<InlineData("stefan_boldly")>]
[<InlineData("@boldly")>]
[<InlineData("stefan@")>]
let ``Email addresses without an at-sign surrounded by characters are invalid`` emailAddress =
    let email = email emailAddress
    let expected = "Invalid email address"

    match email with
    | Ok e -> ResultAssert.okNotExpected e
    | Error e -> Assert.Equal(expected, e)

[<Fact>]
let ``An Email address with a whitespace is invalid`` () =
    let email = email "stef an@boldly"
    let expected = "Invalid email address"

    match email with
    | Ok e -> ResultAssert.okNotExpected e
    | Error e -> Assert.Equal(expected, e)

[<Theory>]
[<InlineData("stefan\n@boldly")>]
[<InlineData("stefan\t@boldly")>]
[<InlineData("stefan\r\n@boldly")>]
let ``Email addresses with control characters are invalid`` emailAddress =
    let email = email emailAddress
    let expected = "Invalid email address"

    match email with
    | Ok e -> ResultAssert.okNotExpected e
    | Error e -> Assert.Equal(expected, e)

[<Fact>]
let ``An invalid email address in a different ui culture returns a localized error message`` () =
    Thread.CurrentThread.CurrentUICulture <- CultureInfo("de")
    let invalidEmail = email "stefanAtboldly"
    let expected = "Ungültige E-Mail-Adresse"

    match invalidEmail with
    | Ok e -> ResultAssert.okNotExpected e
    | Error e -> Assert.Equal(expected, e)

[<Fact>]
let ``An email address must not be empty`` () =
    let email = email ""
    let expected = "The email address must not be empty"
    
    match email with
    | Ok e -> ResultAssert.okNotExpected e
    | Error e -> Assert.Equal(expected, e) 
