namespace FishingCommunity.Domain.Exceptions;

public class BusinessRuleValidationException : Exception
{
    public BusinessRuleValidationException(string brokenRule)
        : base(brokenRule)
    {
    }
}