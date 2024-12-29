using MyEasySQL.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MyEasySQL.Utils.RegexUtil;

namespace MyEasySQL.Queries;

/// <summary>
/// Provides functionality to build and execute SELECT queries on a specified table.
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
    /// <exception cref="ArgumentNullException">Thrown when the database is null.</exception>
    /// <exception cref="ArgumentException">Thrown when no columns are specified or column names are invalid.</exception>
    public SelectQuery(MySQL database, params string[] columns)
    {
        if (columns.Length == 0)
        {
            throw new ArgumentException("Columns cannot be empty.");
        }

        foreach (string column in columns)
        {
            Validate(column, ValidateType.Column);
        }

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _columns = string.Join(", ", columns);
    }

    /// <summary>
    /// Specifies the table to select data from.
    /// </summary>
    /// <param name="table">The name of the table.</param>
    /// <returns>The <see cref="SelectQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the table name is invalid.</exception>
    public SelectQuery From(string table)
    {
        Validate(table, ValidateType.Table);

        _table = table;
        return this;
    }

    /// <summary>
    /// Adds a WHERE clause to the SELECT query.
    /// </summary>
    /// <param name="column">The column name to filter by.</param>
    /// <param name="operator">The comparison operator to use.</param>
    /// <param name="value">The value to compare the column to.</param>
    /// <param name="logicalOperator">The logical operator to chain conditions (default is AND).</param>
    /// <returns>The <see cref="SelectQuery"/> instance for method chaining.</returns>
    public SelectQuery Where(string column, Operators @operator, object value, LogicalOperators? logicalOperator = LogicalOperators.AND)
    {
        _conditionBuilder.Add(column, @operator, value, logicalOperator);
        return this;
    }

    /// <summary>
    /// Adds an ORDER BY clause to the SELECT query.
    /// </summary>
    /// <param name="column">The column to order by.</param>
    /// <param name="orderType">The type of ordering (ASC or DESC).</param>
    /// <returns>The <see cref="SelectQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the column name is invalid.</exception>
    public SelectQuery OrderBy(string column, OrderType orderType)
    {
        Validate(column, ValidateType.Column);

        _orderBy = column;
        _orderType = orderType;
        return this;
    }

    /// <summary>
    /// Adds a LIMIT clause to the SELECT query.
    /// </summary>
    /// <param name="limit">The maximum number of rows to return.</param>
    /// <returns>The <see cref="SelectQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the limit is less than or equal to zero.</exception>
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
    /// <typeparam name="T">The type of the result objects.</typeparam>
    /// <returns>An enumerable collection of results.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the table name is not specified.</exception>
    public async Task<IEnumerable<T>> ReadAsync<T>()
    {
        if (string.IsNullOrWhiteSpace(_table))
        {
            throw new InvalidOperationException("Table name is required.");
        }

        string whereClause = _conditionBuilder.BuildCondition();
        string orderByClause = _orderBy != null && _orderType.HasValue
            ? $"ORDER BY {_orderBy} {_orderType}"
            : string.Empty;

        string limitClause = _limit.HasValue ? $"LIMIT {_limit.Value}" : string.Empty;

        string query = $"SELECT {_columns} FROM {_table} " +
                       $"{(string.IsNullOrWhiteSpace(whereClause) ? string.Empty : $"WHERE {whereClause}")} " +
                       $"{orderByClause} {limitClause}".Trim();

        return await _database.ExecuteQueryAsync<T>(query, _conditionBuilder.GetParameters());
    }
}
