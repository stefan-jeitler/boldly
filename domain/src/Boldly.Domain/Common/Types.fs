module Boldly.Domain.Common.Types

type AsyncResult<'success, 'failure> = Async<Result<'success, 'failure>>
type AsyncOption<'a> = Async<Option<'a>>

type Undefined = exn