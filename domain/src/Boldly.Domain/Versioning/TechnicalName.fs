namespace Boldly.Domain.Versioning

open Boldly.Domain.Common.WrappedString
open Boldly.Domain.Localization

type TechnicalName =
    private | TechnicalName of string
    
    interface IWrappedString with
        member this.Value = let (TechnicalName n) = this in n

module TechnicalName =
    let minLength = 2
    let private minLengthValidator = (minLengthValidator minLength)

    let maxLength = 50
    let private maxLengthValidator = (maxLengthValidator maxLength)

    let private noWhitespacesValidator (candidate: string) =
        match candidate.Contains(" ") with
        | true -> Error ValidationStrings.NoWhitespacesAllowed
        | false -> Ok candidate
        
    let private validators = minLengthValidator >=> maxLengthValidator >=> noWhitespacesValidator
    
    let technicalName = create CommonStrings.TechnicalNameWithArticle singleLineTrimmed validators TechnicalName
    let create = technicalName
