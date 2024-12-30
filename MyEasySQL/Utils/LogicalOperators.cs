namespace MyEasySQL.Utils;

/// <summary>
/// Represents the logical operators used in SQL queries.
/// </summary>
public enum LogicalOperators
{
    /// <summary>
    /// Represents the logical AND operator.
    /// Used to combine multiple conditions in a SQL query, where all conditions must be true.
    /// </summary>
    AND,

    /// <summary>
    /// Represents the logical OR operator.
    /// Used to combine multiple conditions in a SQL query, where at least one condition must be true.
    /// </summary>
    OR
}