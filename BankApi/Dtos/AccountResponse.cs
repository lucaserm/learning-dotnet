using BankApi.Models;

namespace BankApi.Dtos;

public record AccountResponse
(
    Guid Id,
    string Owner,
    decimal Balance,
    DateTime CreatedAt
);

public static class AccountMappingExtensions
{
    public static AccountResponse ToResponse(this Account account)
    {
        return new AccountResponse(
            account.Id,
            account.Owner,
            account.Balance,
            account.CreatedAt
        );
    }
}