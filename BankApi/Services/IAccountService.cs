using BankApi.Models;
using BankApi.Dtos;

namespace BankApi.Services;

public interface IAccountService
{
    Task<PagedResponse<AccountResponse>> ListAsync(int page, int pageSize);
    Task<Account> CreateAsync(string owner);
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> DepositAsync(Guid id, decimal amount);
    Task<Account?> WithdrawAsync(Guid id, decimal amount);
}