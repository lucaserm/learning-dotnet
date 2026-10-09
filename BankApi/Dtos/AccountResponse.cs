namespace BankApi.Dtos;

public record AccountResponse
(
    Guid Id,
    string Owner,
    decimal Balance,
    DateTime CreatedAt
);