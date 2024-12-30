using System;
using System.Threading.Tasks;
using MyEasySQL.Utils;
using static MyEasySQL.Utils.RegexUtil;

namespace MyEasySQL.Queries;

/// <summary>
/// Represents a DELETE query builder for deleting rows from a specified table with optional conditions.
/// Provides methods to specify the table, add conditions, and execute the query asynchronously.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DeleteQuery"/> class.
/// </remarks>
/// <param name="database">The database instance to execute the query on.</param>
/// <exception cref="ArgumentNullException">Thrown when the <paramref name="database"/> is null.</exception>
public class DeleteQuery(MySQL database)
{
    private readonly MySQL _database = database ?? throw new ArgumentNullException(nameof(database));
    private string? _table;
    private readonly ConditionBuilder _conditionBuilder = new();

    /// <summary>
    /// Specifies the table from which rows will be deleted.
    /// </summary>
    /// <param name="table">The name of the table to delete from.</param>
    /// <returns>The <see cref="DeleteQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="table"/> name is invalid.</exception>
    public DeleteQuery From(string table)
    {
        Validate(table, ValidateType.Table);

        _table = table;
        return this;
    }

    /// <summary>
    /// Adds a condition to the DELETE query. Conditions are used to filter which rows should be deleted.
    /// </summary>
    /// <param name="column">The name of the column to apply the condition on.</param>
    /// <param name="operator">The operator to use in the condition (e.g., equals, greater than).</param>
    /// <param name="value">The value to compare the column against.</param>
    /// <param name="logicalOperator">The logical operator used to combine multiple conditions (default is AND).</param>
    /// <returns>The <see cref="DeleteQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="column"/> name is invalid.</exception>
    public DeleteQuery Where(string column, Operators @operator, object value, LogicalOperators? logicalOperator = LogicalOperators.AND)
    {
        _conditionBuilder.Add(column, @operator, value, logicalOperator);
        return this;
    }

    /// <summary>
    /// Executes the DELETE query asynchronously, removing rows that match the specified conditions.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the number of rows affected by the command.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when the table name is not specified before execution.</exception>
    public async Task<int> ExecuteAsync()
    {
        if (string.IsNullOrWhiteSpace(_table))
        {
            throw new InvalidOperationException("Table name is required.");
        }

        string whereClause = _conditionBuilder.BuildCondition();
        string query = $"DELETE FROM {_table} {(string.IsNullOrWhiteSpace(whereClause) ? "" : $"WHERE {whereClause}")};";

        return await _database.ExecuteNonQueryAsync(query, _conditionBuilder.GetParameters());
    }
}
