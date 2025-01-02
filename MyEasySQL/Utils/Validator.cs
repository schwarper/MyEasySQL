using System;
using System.Text.RegularExpressions;

namespace MyEasySQL.Utils;

internal static partial class Validator
{
    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled)] internal static partial Regex NameRegex();
    [GeneratedRegex(@"^\d+$", RegexOptions.Compiled)] internal static partial Regex SingleValueRegex();
    [GeneratedRegex(@"^\d+,\d+$", RegexOptions.Compiled)] internal static partial Regex MultiValueRegex();
    [GeneratedRegex(@"^[a-zA-Z0-9_\+\-\*/\s]+$", RegexOptions.Compiled)] internal static partial Regex UpdateKeyRegex();

    internal enum ValidateType
    {
        Database,
        Table,
        Column,
        TypeValueSingle,
        TypeValueMulti,
        UpdateKey
    }

    internal static void ValidateName(string name, ValidateType type)
    {
        if (string.IsNullOrWhiteSpace(name) || !NameRegex().IsMatch(name))
        {
            throw new ArgumentException($"Invalid {type} name.", name);
        }
    }

    internal static void ValidateUpdateKey(string value)
    {
        if (string.IsNullOrEmpty(value) || !UpdateKeyRegex().IsMatch(value))
        {
            throw new ArgumentException($"Invalid value", value);
        }
    }

    internal static void ValidateTypeValue(DataTypes type, string? typeValue)
    {
        switch (type)
        {
            case DataTypes.VARCHAR:
            case DataTypes.CHAR:
                {
                    if (string.IsNullOrWhiteSpace(typeValue) || !SingleValueRegex().IsMatch(typeValue))
                    {
                        throw new ArgumentException($"Invalid value for type {type}: '{typeValue}'. Expected a single integer value.");
                    }
                    break;
                }
            case DataTypes.DECIMAL:
                {
                    if (string.IsNullOrWhiteSpace(typeValue) || !MultiValueRegex().IsMatch(typeValue))
                    {
                        throw new ArgumentException($"Invalid value for type {type}: '{typeValue}'. Expected format is 'precision,scale' (e.g., FLOAT(2,3)).");
                    }
                    break;
                }
            default:
                {
                    if (typeValue != null)
                    {
                        throw new NotSupportedException($"Type '{type}' is not supported or does not require a type value.");
                    }
                    break;
                }
        }
    }
}
