module Boldly.Domain.Tests.AccountAdministration.EmailTests

open System.Globalization
open System.Threading
open Boldly.Domain.Common
open Boldly.Domain.AccountAdministration.Email
open Xunit

[<Fact>]
let ``Valid email address (Happy Path)`` () =
    let email = email "stefan@change-blog.com"
    
    match email with
    | Error e -> Assert.Fail($"Expected Ok, but got an error '{e}'")
    | Ok e -> Assert.Equal("stefan@change-blog.com", WrappedString.value e)

[<Theory>]
[<InlineData("stefan@change-blog.com ")>]
[<InlineData(" stefan@change-blog.com")>]
[<InlineData(" stefan@change-blog.com ")>]
let ``Leading and trailing whitespaces in email addresses will be removed`` emailAddress =
    let email = email emailAddress
    let expected = create "stefan@change-blog.com"

    Assert.Equal(expected, email)

[<Theory>]
[<InlineData("stefanAtchange-blog")>]
[<InlineData("stefan_change-blog")>]
[<InlineData("@change-blog")>]
[<InlineData("stefan@")>]
let ``Email addresses without an at-sign surrounded by characters are invalid`` emailAddress =
    let email = email emailAddress
    let expected = Error "Invalid email address"

    Assert.Equal(expected, email)

[<Fact>]
let ``An Email address with a whitespace is invalid`` () =
    let email = email "stef an@change-blog"
    let expected = Error "Invalid email address"

    Assert.Equal(expected, email)

[<Theory>]
[<InlineData("stefan\n@change-blog")>]
[<InlineData("stefan\t@change-blog")>]
[<InlineData("stefan\r\n@change-blog")>]
let ``Email addresses with control characters are invalid`` emailAddress =
    let email = email emailAddress
    let expected = Error "Invalid email address"

    Assert.Equal(expected, email)

[<Fact>]
let ``An invalid email address in a different ui culture returns a localized error message`` () =
    Thread.CurrentThread.CurrentUICulture <- CultureInfo("de")
    let invalidEmail = email "stefanAtchange-blog"
    let expected = Error "Ungültige E-Mail-Adresse"

    Assert.Equal(expected, invalidEmail)
