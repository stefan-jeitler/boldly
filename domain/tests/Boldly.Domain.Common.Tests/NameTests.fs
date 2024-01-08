module Boldly.Domain.Tests.Common.NameTests

open System
open System.Globalization
open System.Threading
open Boldly.Domain.Common
open Boldly.Domain.Tests
open Xunit
open Boldly.Domain.Common.Name

[<Fact>]
let ``A name with just one character is not a valid name, it needs at least two characters`` () =
    let name = name "a"
    let expected = "The length must be at least 2 characters"
    
    match name with
    | Error e -> Assert.Equal(expected, e)
    | Ok n -> ResultAssert.okNotExpected n

[<Fact>]
let ``A name with two characters is Ok`` () =
    let name = name "QA"
    
    match name with
    | Ok n -> Assert.Equal("QA", (WrappedString.value n))
    | Error e -> ResultAssert.errorNotExpected e

[<Fact>]
let ``A name with more than 50 characters is too long`` () =
    let tooLongName = name (String('a', 51))
    let expected = "The length must be 50 characters or fewer"

    match tooLongName with
    | Ok n -> ResultAssert.errorNotExpected n
    | Error e -> Assert.Equal(expected, e)

[<Fact>]
let ``Null is not a valid value for a name`` () =
    let name = name null
    let expected = "The name must not be empty"
    
    match name with
    | Ok n -> ResultAssert.okNotExpected n
    | Error e -> Assert.Equal(expected, e) 

[<Fact>]
let ``An empty name is invalid, it needs at leads 2 characters`` () =
    let name = name ""
    let expected = "The length must be at least 2 characters"
    
    match name with
    | Ok n -> ResultAssert.okNotExpected n
    | Error e -> Assert.Equal(expected, e)

[<Fact>]
let ``An invalid name in a different ui culture returns a localized error message`` () =
    Thread.CurrentThread.CurrentUICulture <- CultureInfo("de")
    let name = name ""
    let expected = "Die Länge muss größer oder gleich 2 sein"
    
    match name with
    | Ok n -> ResultAssert.okNotExpected n
    | Error e -> Assert.Equal(expected, e)

[<Fact>]
let ``Valid Name (Happy Path)`` () =
    let name = name "Stefan Jeitler"
    let expected = "Stefan Jeitler"
    
    match name with
    | Error e -> ResultAssert.errorNotExpected e
    | Ok n -> Assert.Equal(expected, WrappedString.value n)

[<Theory>]
[<InlineData("Stefan\nJeitler", "Stefan Jeitler")>]
[<InlineData("Stefan\rJeitler", "Stefan Jeitler")>]
[<InlineData("Stefan\r\nJeitler", "Stefan Jeitler")>]
[<InlineData("Stefan\tJeitler", "Stefan Jeitler")>]
[<InlineData("Stefan\fJeitler", "Stefan Jeitler")>]
let ``Control characters in names will be replaced with whitespaces``
    (candidate: string)
    (expectedName: string)
    =
    match name candidate with
    | Ok n -> Assert.Equal(expectedName, WrappedString.value n)
    | Error e -> ResultAssert.errorNotExpected e

[<Theory>]
[<InlineData("Stefan ", "Stefan")>]
[<InlineData(" Stefan", "Stefan")>]
[<InlineData(" Stefan ", "Stefan")>]
let ``Leading and trailing whitespaces in names will be removed``
    (candidate: string)
    (expectedName: string)
    =
    match name candidate with
    | Ok n -> Assert.Equal(expectedName, WrappedString.value n)
    | Error e -> ResultAssert.errorNotExpected e
