module Boldly.Domain.Tests.Common.NameTests

open System
open System.Globalization
open System.Threading
open Boldly.Domain.Common
open Xunit
open Boldly.Domain.Common.Name

[<Fact>]
let ``A name with just one character is not a valid name, it needs at least two characters`` () =
    let name = name "a"
    let expected = Error "The length must be at least 2 characters"
    
    Assert.Equal(expected, name)

[<Fact>]
let ``A name with more than 50 characters is too long`` () =
    let tooLongName = name (String('a', 51))
    let expected = Error "The length must be 50 characters or fewer"

    Assert.Equal(expected, tooLongName)

[<Fact>]
let ``Null is not a valid value for a name`` () =
    let name = name null
    let expected = Error "Name must not be empty"
    
    Assert.Equal(expected, name)

[<Fact>]
let ``An empty name is invalid, it needs at leads 2 characters`` () =
    let name = name ""
    let expected = Error "The length must be at least 2 characters"
    
    Assert.Equal(expected, name)

[<Fact>]
let ``An invalid name in a different ui culture returns a localized error message`` () =
    Thread.CurrentThread.CurrentUICulture <- CultureInfo("de")
    let name = name ""
    let expected = Error "Die Länge muss größer oder gleich 2 sein"
    
    Assert.Equal(expected, name)

[<Fact>]
let ``Valid Name (Happy Path)`` () =
    let name = name "Stefan Jeitler"
    let expected = "Stefan Jeitler"
    
    match name with
    | Error e -> Assert.Fail($"Expected Ok, but got an error '{e}'")
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
    | Error e -> Assert.Fail($"Expected Ok, but got an error '{e}'")

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
    | Error e -> Assert.Fail($"Expected Ok, but got an error '{e}'")
