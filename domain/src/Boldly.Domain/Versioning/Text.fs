namespace Boldly.Domain.Versioning

open Boldly.Domain.Localization
open Boldly.Domain.Common.WrappedString

type Text =
    private
    | Text of string

    interface IWrappedString with
        member this.Value = let (Text t) = this in t

module Text =

    let maxLength = 500
    let private maxLengthValidator = (maxLengthValidator maxLength)

    let text = create CommonStrings.TextWithArticle id maxLengthValidator Text
    let create = text


