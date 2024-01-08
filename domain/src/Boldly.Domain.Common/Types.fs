module Boldly.Domain.Common.Types

type AsyncResult<'success, 'failure> = Async<Result<'success, 'failure>>
type AsyncOption<'a> = Async<Option<'a>>


module ResultExtensions =
    let (>>=) r f = r |> Result.bind f
    let (>=>) f1 f2 = f1 >> Result.bind f2

module OptionExtensions =
    let (>>=) r f = r |> Option.bind f
    let (>=>) f1 f2 = f1 >> Option.bind f2

type Undefined = exn