using System;
using System.Text.RegularExpressions;

namespace MyEasySQL.Utils;

internal static partial class RegexUtil
{
    [GeneratedRegex(@"^[a-zA-Z_*]\w*$")] private static partial Regex NameRegex();
    [GeneratedRegex(@"^\d+$")] private static partial Regex ValueRegex();
    [GeneratedRegex(@"^[a-zA-Z_]\w*( (ASC|DESC))?$")] private static partial Regex OrderRegex();

    internal enum ValidateType
    {
        Database,
        Table,
        Column,
        TypeValue,
        OrderBy
    };

    internal static void Validate(string name, ValidateType type)
    {
        switch (type)
        {
            case ValidateType.Database:
                if (string.IsNullOrWhiteSpace(name) || !NameRegex().IsMatch(name))
                {
                    throw new ArgumentException("Invalid database name.", nameof(name));
                }
                break;

            case ValidateType.Table:
                if (string.IsNullOrWhiteSpace(name) || !NameRegex().IsMatch(name))
                {
                    throw new ArgumentException("Invalid table name.", nameof(name));
                }
                break;

            case ValidateType.Column:
                if (string.IsNullOrWhiteSpace(name) || !NameRegex().IsMatch(name))
                {
                    throw new ArgumentException("Invalid column name.", nameof(name));
                }
                break;

            case ValidateType.TypeValue:
                if (string.IsNullOrWhiteSpace(name) || !ValueRegex().IsMatch(name))
                {
                    throw new ArgumentException("Invalid type value.", nameof(name));
                }
                break;

            case ValidateType.OrderBy:
                if (string.IsNullOrWhiteSpace(name) || !OrderRegex().IsMatch(name))
                {
                    throw new ArgumentException("Invalid order by clause.", nameof(name));
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
