using System.Text.RegularExpressions;

namespace Domain.ValueObject;

public partial record PhoneNumber
{
    private const int DefaultLenght = 9;
    private const string Pattern = @"^\d{1,4}-\d{1,4}$";
    
    private PhoneNumber(string value) => Value = value;

    public static PhoneNumber? Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        if (PhoneNumberRegex().IsMatch(value))
            return null;
        if (value.Length != DefaultLenght)
            return null;
        return new PhoneNumber(value);
    }

    public string Value { get; init; }
    
    [GeneratedRegex(Pattern)]
    private static partial Regex PhoneNumberRegex();
}