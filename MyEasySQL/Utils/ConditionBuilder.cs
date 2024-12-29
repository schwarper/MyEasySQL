using System;
using System.Collections.Generic;

namespace MyEasySQL.Utils;

/// <summary>
/// Builds SQL conditions dynamically with support for parameters and logical operators.
/// </summary>
public class ConditionBuilder
{
    private readonly List<string> _conditions = [];
    private readonly Dictionary<string, object> _parameters = [];

    /// <summary>
    /// Adds a condition to the condition builder.
    /// </summary>
    /// <param name="column">The name of the column.</param>
    /// <param name="operator">The operator to use for the condition.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="logicalOperator">Optional logical operator to chain conditions.</param>
    /// <returns>The current instance of <see cref="ConditionBuilder"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the operator is not recognized.</exception>
    public ConditionBuilder Add(string column, Operators @operator, object value, LogicalOperators? logicalOperator = LogicalOperators.AND)
    {
        if (_conditions.Count > 0 && logicalOperator.HasValue)
        {
            _conditions.Add(logicalOperator.Value.ToString());
        }

        string paramName = column.Replace(".", "_");
        _conditions.Add($"{column} {OperatorToString(@operator)} @{paramName}");
        _parameters[paramName] = value;
        return this;
    }

    /// <summary>
    /// Converts the operator enum to its string representation for SQL queries.
    /// </summary>
    /// <param name="operator">The operator to convert.</param>
    /// <returns>A string representation of the operator.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the operator is not recognized.</exception>
    private static string OperatorToString(Operators @operator) => @operator switch
    {
        Operators.EQUAL => "=",
        Operators.NOT_EQUAL => "!=",
        Operators.GREATER_THAN => ">",
        Operators.LESS_THAN => "<",
        Operators.GREATER_OR_EQUAL => ">=",
        Operators.LESS_OR_EQUAL => "<=",
        Operators.LIKE => "LIKE",
        Operators.IN => "IN",
        Operators.NOT_IN => "NOT IN",
        Operators.BETWEEN => "BETWEEN",
        _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null)
    };

    /// <summary>
    /// Builds the complete SQL condition string.
    /// </summary>
    /// <returns>The SQL condition string.</returns>
    public string BuildCondition() => string.Join(" ", _conditions);

    /// <summary>
    /// Retrieves the parameters for the SQL query.
    /// </summary>
    /// <returns>A dictionary of parameter names and their corresponding values.</returns>
    public Dictionary<string, object> GetParameters() => _parameters;
}
