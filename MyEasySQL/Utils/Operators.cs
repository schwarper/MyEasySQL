namespace MyEasySQL.Utils;

/// <summary>
/// Represents the operators used in SQL queries for comparison and pattern matching.
/// </summary>
public enum Operators
{
    /// <summary>
    /// Represents the equality operator (EQUAL).
    /// Checks if two values are equal.
    /// </summary>
    EQUAL,

    /// <summary>
    /// Represents the inequality operator (NOT_EQUAL).
    /// Checks if two values are not equal.
    /// </summary>
    NOT_EQUAL,

    /// <summary>
    /// Represents the greater-than operator (GREATER_THAN).
    /// Checks if a value is greater than another value.
    /// </summary>
    GREATER_THAN,

    /// <summary>
    /// Represents the less-than operator (LESS_THAN).
    /// Checks if a value is less than another value.
    /// </summary>
    LESS_THAN,

    /// <summary>
    /// Represents the greater-than-or-equal operator (GREATER_OR_EQUAL).
    /// Checks if a value is greater than or equal to another value.
    /// </summary>
    GREATER_OR_EQUAL,

    /// <summary>
    /// Represents the less-than-or-equal operator (LESS_OR_EQUAL).
    /// Checks if a value is less than or equal to another value.
    /// </summary>
    LESS_OR_EQUAL,

    /// <summary>
    /// Represents the LIKE operator.
    /// Used for pattern matching in strings.
    /// </summary>
    LIKE,

    /// <summary>
    /// Represents the IN operator.
    /// Checks if a value is in a list of values.
    /// </summary>
    IN,

    /// <summary>
    /// Represents the NOT IN operator.
    /// Checks if a value is not in a list of values.
    /// </summary>
    NOT_IN,

    /// <summary>
    /// Represents the BETWEEN operator.
    /// Used to filter results within a certain range.
    /// Checks if a value is between two other values.
    /// </summary>
    BETWEEN
}
