using System;
using System.Collections.Generic;

namespace MyEasySQL.Utils;

/// <summary>
/// Builds SQL conditions dynamically, supporting various operators and logical operators for use in queries.
/// </summary>
public class ConditionBuilder
{
    private readonly List<string> _conditions = [];
    private readonly Dictionary<string, object> _parameters = [];

    /// <summary>
    /// Adds a condition to the condition builder with an optional logical operator to chain multiple conditions.
    /// </summary>
    /// <param name="column">The name of the column to be used in the condition.</param>
    /// <param name="operator">The SQL operator to be applied in the condition (e.g., EQUAL, GREATER THAN).</param>
    /// <param name="value">The value to be compared with the column's value.</param>
    /// <param name="logicalOperator">An optional logical operator (AND, OR) to combine this condition with others.</param>
    /// <returns>The current instance of <see cref="ConditionBuilder"/> for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided operator is not recognized.</exception>
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
    /// Converts the operator enum to its SQL string representation for use in queries.
    /// </summary>
    /// <param name="operator">The operator enum to convert.</param>
    /// <returns>A string representing the SQL operator.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the provided operator is not recognized.</exception>
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
    /// Builds the complete SQL condition string by combining all added conditions.
    /// </summary>
    /// <returns>A string representing the full condition expression for the SQL query.</returns>
    public string BuildCondition() => string.Join(" ", _conditions);

    /// <summary>
    /// Retrieves the parameters for the SQL query, including column values and their associated parameter names.
    /// </summary>
    /// <returns>A dictionary where the key is the parameter name and the value is the corresponding parameter value.</returns>
    public Dictionary<string, object> GetParameters() => _parameters;
}
