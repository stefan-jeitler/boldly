namespace Boldly.WebApi.Features.Account.V1.DTOs

open System


type AccountDto = {
    Name: string
    CreatedAt: DateTimeOffset
}