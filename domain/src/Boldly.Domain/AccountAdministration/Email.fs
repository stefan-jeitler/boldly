namespace Boldly.Domain.AccountAdministration

open System.Text.RegularExpressions
open Boldly.Domain.Localization
open Boldly.Domain.Common.WrappedString
open System

type Email =
    private
    | Email of string

    interface IWrappedString with
        member this.Value = let (Email e) = this in e

module Email =

    let maxLength = 254
    let private maxLengthValidator = maxLengthValidator maxLength

    let private emailValidator (candidate: string) =
        if Regex.IsMatch(candidate, @"^\S+@\S+") then
            Ok(candidate)
        else
            Error ValidationStrings.InvalidEmailAddressError

    let notEmptyValidator (candidate: string) =
        match String.IsNullOrWhiteSpace(candidate) with
        | true -> Error (String.Format(ValidationStrings.EmptyError, CommonStrings.EmailAddressWithArticle))
        | false -> Ok candidate
    
    let private validator = notEmptyValidator >=> emailValidator >=> maxLengthValidator

    let email = create CommonStrings.EmailAddressWithArticle singleLineTrimmed validator Email
    let create = email
