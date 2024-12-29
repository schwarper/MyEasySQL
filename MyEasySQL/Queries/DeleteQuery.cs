using MyEasySQL.Utils;
using System;
using System.Threading.Tasks;
using static MyEasySQL.Utils.RegexUtil;

namespace MyEasySQL.Queries;

/// <summary>
/// Provides functionality to build and execute DELETE queries on a specified table with optional conditions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DeleteQuery"/> class.
/// </remarks>
/// <param name="database">The database instance to execute the query on.</param>
/// <exception cref="ArgumentNullException">Thrown when the database is null.</exception>
public class DeleteQuery(MySQL database)
{
    private readonly MySQL _database = database ?? throw new ArgumentNullException(nameof(database));
    private string? _table;
    private readonly ConditionBuilder _conditionBuilder = new();

    /// <summary>
    /// Specifies the table from which rows will be deleted.
    /// </summary>
    /// <param name="table">The name of the table.</param>
    /// <returns>The <see cref="DeleteQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the table name is invalid.</exception>
    public DeleteQuery From(string table)
    {
        Validate(table, ValidateType.Table);

        _table = table;
        return this;
    }

    /// <summary>
    /// Adds a condition to the DELETE query.
    /// </summary>
    /// <param name="column">The name of the column to apply the condition on.</param>
    /// <param name="operator">The operator to use in the condition.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="logicalOperator">The logical operator to use if combining multiple conditions (default is AND).</param>
    /// <returns>The <see cref="DeleteQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the column name is invalid.</exception>
    public DeleteQuery Where(string column, Operators @operator, object value, LogicalOperators? logicalOperator = LogicalOperators.AND)
    {
        _conditionBuilder.Add(column, @operator, value, logicalOperator);
        return this;
    }

    /// <summary>
    /// Executes the DELETE query asynchronously.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the number of rows affected by the command.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when the table name is not specified.</exception>
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