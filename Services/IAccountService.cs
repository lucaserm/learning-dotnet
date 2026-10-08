using BankApi.Models;

namespace BankApi.Services;

public interface IAccountService
{
    Task<Account> CreateAsync(string owner);
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> DepositAsync(Guid id, decimal amount);
    Task<Account?> WithdrawAsync(Guid id, decimal amount);
}