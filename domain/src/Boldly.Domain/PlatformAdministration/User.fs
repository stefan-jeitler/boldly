module Boldly.Domain.PlatformAdministration.User

open System
open Boldly.Domain.Common

type User = {
      Id: Guid
      FirstName: Name
      LastName: Name
      Timezone: Name
      Culture: Name
      DateFormat: Name option
      TimeFormat: Name option
}
