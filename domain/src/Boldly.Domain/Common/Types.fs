module Boldly.Domain.Common.Types

type AsyncResult<'success, 'failure> = Async<Result<'success, 'failure>>

type Undefined = exn