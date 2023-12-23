namespace Boldly.Domain.Common

open Boldly.Domain.Localization
open Boldly.Domain.Common.WrappedString

type Name =
    private
    | Name of string

    interface IWrappedString with
        member this.Value = let (Name n) = this in n

module Name =

    let minLength = 2
    let private minLengthValidator = (minLengthValidator minLength)

    let maxLength = 50
    let private maxLengthValidator = (maxLengthValidator maxLength)

    let private validator = minLengthValidator >=> maxLengthValidator

    let name = create ValidationStrings.Name singleLineTrimmed validator Name
    let create = name
