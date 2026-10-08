namespace BankApi.Models;

public class Account
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Owner { get; }
    public decimal Balance { get; private set; }

    public Account(string owner)
    {
        if (string.IsNullOrWhiteSpace(owner))
        {
            throw new ArgumentException(
                "owner is required.", 
                nameof(owner)
            );
        }

        Owner = owner.Trim();
    }
}