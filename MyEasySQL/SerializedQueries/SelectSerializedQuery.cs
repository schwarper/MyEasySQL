using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyEasySQL.Utils;
using static MyEasySQL.Utils.Validator;

namespace MyEasySQL.SerializedQueries;

/// <summary>
/// Provides functionality to execute SELECT and UPDATE queries on serialized objects.
/// </summary>
/// <remarks>
/// This class allows building SQL SELECT queries dynamically using object properties and executing them asynchronously.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="SerializedSelectQuery{T}"/> class.
/// </remarks>
/// <param name="database">The database instance to execute queries against.</param>
/// <exception cref="ArgumentNullException">Thrown if the <paramref name="database"/> is null.</exception>
public class SerializedSelectQuery<T>(MySQL database) where T : class, new()
{
    private readonly MySQL _database = database ?? throw new ArgumentNullException(nameof(database));
    private string? _table;
    private string? _orderBy;
    private OrderType? _orderType;
    private int? _limit;
    private readonly ConditionBuilder _conditionBuilder = new();

    /// <summary>
    /// Specifies the table to select data from.
    /// </summary>
    /// <param name="table">The name of the table to select from.</param>
    /// <returns>The current <see cref="SerializedSelectQuery{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="table"/> name is invalid.</exception>
    public SerializedSelectQuery<T> From(string table)
    {
        ValidateName(table, ValidateType.Table);

        _table = table;
        return this;
    }

    /// <summary>
    /// Adds a WHERE clause to the SELECT query to filter results based on a condition.
    /// </summary>
    /// <param name="column">The column name to filter by.</param>
    /// <param name="operator">The comparison operator to use in the condition.</param>
    /// <param name="value">The value to compare the column against.</param>
    /// <param name="logicalOperator">The logical operator to chain multiple conditions (default is AND).</param>
    /// <returns>The <see cref="SerializedSelectQuery{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="column"/> name is invalid.</exception>
    public SerializedSelectQuery<T> Where(string column, Operators @operator, object value, LogicalOperators? logicalOperator = LogicalOperators.AND)
    {
        ValidateName(column, ValidateType.Column);

        _conditionBuilder.Add(column, @operator, value, logicalOperator);
        return this;
    }

    /// <summary>
    /// Adds an ORDER BY clause to the query.
    /// </summary>
    /// <param name="column">The column to order by.</param>
    /// <param name="orderType">The order type (ASC or DESC, default is ASC).</param>
    /// <returns>The current <see cref="SerializedSelectQuery{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="column"/> name is invalid.</exception>
    public SerializedSelectQuery<T> OrderBy(string column, OrderType orderType)
    {
        ValidateName(column, ValidateType.Column);

        _orderBy = column;
        _orderType = orderType;
        return this;
    }

    /// <summary>
    /// Adds a LIMIT clause to the query.
    /// </summary>
    /// <param name="limit">The maximum number of records to return.</param>
    /// <returns>The current <see cref="SerializedSelectQuery{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="limit"/> is less than or equal to zero.</exception>
    public SerializedSelectQuery<T> Limit(int limit)
    {
        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than 0.");
        }
        _limit = limit;
        return this;
    }

    /// <summary>
    /// Executes the SELECT query and deserializes the results into objects of type T.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized objects of type T.</returns>
    /// <summary>
    /// Executes the SELECT query asynchronously and retrieves the results.
    /// </summary>
    /// <returns>An enumerable collection of results of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the table name is not specified.</exception>
    public async Task<IEnumerable<T>> ReadAsync()
    {
        if (string.IsNullOrWhiteSpace(_table))
        {
            throw new InvalidOperationException("Table name is required.");
        }

        StringBuilder builder = new();
        builder.Append($"SELECT * FROM {_table}");

        string whereClause = _conditionBuilder.BuildCondition();
        if (!string.IsNullOrWhiteSpace(whereClause))
        {
            builder.Append($" WHERE {whereClause}");
        }

        if (_orderBy != null && _orderType.HasValue)
        {
            builder.Append($" ORDER BY {_orderBy} {_orderType}");
        }

        if (_limit.HasValue)
        {
            builder.Append($" LIMIT {_limit.Value}");
        }

        string query = builder.ToString().Trim();

        return await _database.ExecuteQueryAsync<T>(query, _conditionBuilder.GetParameters());
    }
}
