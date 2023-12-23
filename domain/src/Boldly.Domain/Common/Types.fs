module Boldly.Domain.Common.Types

type NonEmptySet<'a when 'a : comparison> = Set<'a>

type AsyncResult<'success, 'failure> = Async<Result<'success, 'failure>>

type Undefined = exn