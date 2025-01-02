using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyEasySQL.Utils;
using static MyEasySQL.Utils.Validator;

namespace MyEasySQL.Queries;

/// <summary>
/// Provides functionality to build and execute SELECT queries on a specified table.
/// Allows the user to specify columns, conditions, sorting, and limits for the query.
/// </summary>
public class SelectQuery
{
    private readonly MySQL _database;
    private readonly string _columns;
    private string? _table;
    private string? _orderBy;
    private OrderType? _orderType;
    private int? _limit;
    private readonly ConditionBuilder _conditionBuilder = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectQuery"/> class.
    /// </summary>
    /// <param name="database">The database instance to execute the query on.</param>
    /// <param name="columns">The columns to select in the query.</param>
    /// <exception cref="ArgumentException">Thrown if no <paramref name="columns"/> are specified or column names are invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="database"/> is null.</exception>
    public SelectQuery(MySQL database, params string[] columns)
    {
        if (columns.Length == 0)
        {
            throw new ArgumentException("Columns cannot be empty.");
        }

        foreach (string column in columns)
        {
            if (column == "*")
            {
                continue;
            }

            ValidateName(column, ValidateType.Column);
        }

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _columns = string.Join(", ", columns);
    }

    /// <summary>
    /// Specifies the table to select data from.
    /// </summary>
    /// <param name="table">The name of the table.</param>
    /// <returns>The <see cref="SelectQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="table"/> name is invalid.</exception>
    public SelectQuery From(string table)
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
    /// <returns>The <see cref="SelectQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="column"/> name is invalid.</exception>
    public SelectQuery Where(string column, Operators @operator, object value, LogicalOperators? logicalOperator = LogicalOperators.AND)
    {
        ValidateName(column, ValidateType.Column);

        _conditionBuilder.Add(column, @operator, value, logicalOperator);
        return this;
    }

    /// <summary>
    /// Adds an ORDER BY clause to the SELECT query to sort the results.
    /// </summary>
    /// <param name="column">The column to sort the results by.</param>
    /// <param name="orderType">The sorting order (ASC for ascending or DESC for descending).</param>
    /// <returns>The <see cref="SelectQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the column name is invalid.</exception>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="column"/> name is invalid.</exception>
    public SelectQuery OrderBy(string column, OrderType orderType)
    {
        ValidateName(column, ValidateType.Column);

        _orderBy = column;
        _orderType = orderType;
        return this;
    }

    /// <summary>
    /// Adds a LIMIT clause to the SELECT query to restrict the number of rows returned.
    /// </summary>
    /// <param name="limit">The maximum number of rows to return.</param>
    /// <returns>The <see cref="SelectQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified <paramref name="limit"/> is less than or equal to zero.</exception>
    public SelectQuery Limit(int limit)
    {
        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be a positive integer.");
        }

        _limit = limit;
        return this;
    }

    /// <summary>
    /// Executes the SELECT query asynchronously and retrieves the results.
    /// </summary>
    /// <typeparam name="T">The type of the result objects to return.</typeparam>
    /// <returns>An enumerable collection of results of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the table name is not specified.</exception>
    public async Task<IEnumerable<T>> ReadAsync<T>()
    {
        if (string.IsNullOrWhiteSpace(_table))
        {
            throw new InvalidOperationException("Table name is required.");
        }

        StringBuilder builder = new();
        builder.Append($"SELECT {_columns} FROM {_table}");

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
