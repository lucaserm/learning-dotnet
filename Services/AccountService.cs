using System.Collections.Concurrent;
using BankApi.Models;

namespace BankApi.Services;

public class AccountService : IAccountService
{
    private readonly ConcurrentDictionary<Guid, Account> _accounts = new();
    public Account Create(string owner)
    {
        var account = new Account(owner);
        _accounts[account.Id] = account;

        return account;
    }

    public Account? GetById(Guid id)
    {
        return _accounts.TryGetValue(id, out var account)
            ? account
            : null;
    }
}