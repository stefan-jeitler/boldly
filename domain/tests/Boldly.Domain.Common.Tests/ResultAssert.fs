module Boldly.Domain.Tests.ResultAssert

open Xunit

let okNotExpected<'a>(v: 'a) = Assert.Fail($"Expected an Error, but got Ok '{v}'")
let errorNotExpected<'a>(e: 'a) = Assert.Fail($"Expected Ok, but got an error '{e}'")