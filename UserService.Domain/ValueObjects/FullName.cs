namespace UserService.Domain.ValueObjects;

public record FullName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Value => $"{FirstName} {LastName}";

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public override string ToString() => Value;
}
