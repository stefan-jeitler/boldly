module Boldly.Domain.AccountAdministration.User

open System
open Boldly.Domain.Common

type User =
    { Id: Guid
      FirstName: Name
      LastName: Name }
