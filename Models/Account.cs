namespace BankApi.Models;

public class Account
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Owner { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }

    private Account() { }

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