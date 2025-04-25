using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class IsGuidAttribute : ValidationAttribute
{
    private static readonly string _pattern = @"^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$";

    public IsGuidAttribute() : base("The field {0} must be a valid GUID.")
    {
    }

    public override bool IsValid(object value)
    {
        return value is string stringValue &&
               Regex.IsMatch(stringValue, _pattern);
    }
}