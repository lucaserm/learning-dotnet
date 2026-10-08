using BankApi.Models;

namespace BankApi.Services;

public interface IAccountService
{
    Account Create(string owner);
    Account? GetById(Guid id);
}