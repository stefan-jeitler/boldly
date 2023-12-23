namespace Boldly.Domain.AccountAdministration

open System.Text.RegularExpressions
open Boldly.Domain.Localization
open Boldly.Domain.Common.WrappedString

type Email =
    private
    | Email of string

    interface IWrappedString with
        member this.Value = let (Email e) = this in e

module Email =

    let minLength = 3
    let private minLengthValidator =
        (minLengthValidator minLength ValidationStrings.Email)

    let maxLength = 254
    let private maxLengthValidator = maxLengthValidator maxLength

    let private emailValidator (candidate: string) =
        if Regex.IsMatch(candidate, @"^\S+@\S+") then
            Ok(candidate)
        else
            Error ValidationStrings.InvalidEmailAddressError

    let private validator = emailValidator >=> maxLengthValidator

    let email = create ValidationStrings.Email singleLineTrimmed validator Email
    let create = email
