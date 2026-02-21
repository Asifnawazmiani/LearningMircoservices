namespace UserService.Domain.ValueObjects;

public record MobileNo
{
    public string Value { get; }

    public MobileNo(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Mobile number cannot be empty");

        // Remove spaces and dashes
        Value = value.Replace(" ", "").Replace("-", "").Trim();
    }

    public override string ToString() => Value;
}
