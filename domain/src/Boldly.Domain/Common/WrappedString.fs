module Boldly.Domain.Common.WrappedString

open System
open System.Text.RegularExpressions
open Boldly.Domain.Localization

let (>=>) a b = a >> (Result.bind b)

type IWrappedString =
    abstract Value: string

let create (notEmptyErrorPrefix: string) canonicalize validate ctor (candidate: string) =
    if candidate = null then
        Error(String.Format(ValidationStrings.EmptyError, notEmptyErrorPrefix))
    else
        let c = canonicalize candidate

        match validate c with
        | Ok v -> Ok(ctor v)
        | Error e -> Error e

let apply f (s: IWrappedString) = s.Value |> f

let value s = apply id s

let equals left right = (value left) = (value right)

let compareTo left right = (value left).CompareTo(value right)

let singleLineTrimmed candidate =
    Regex.Replace(candidate, "\s+", " ").Trim()

let minLengthValidator minLength (candidate: string) =
    match candidate.Length with
    | l when l < minLength -> Error(String.Format(ValidationStrings.MinLengthNotReachedError, minLength))
    | _ -> Ok(candidate)

let maxLengthValidator maxLength (candidate: string) =
    match candidate.Length with
    | l when l > maxLength -> Error(String.Format(ValidationStrings.MaxLengthExceededError, maxLength))
    | _ -> Ok(candidate)
