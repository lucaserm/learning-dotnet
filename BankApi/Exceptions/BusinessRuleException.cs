namespace BankApi.Exceptions;

public class BusinessRuleException(string message) : Exception(message);