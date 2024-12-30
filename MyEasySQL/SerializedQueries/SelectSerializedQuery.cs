using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MyEasySQL.Utils.RegexUtil;

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
public class SerializedSelectQuery<T>(MySQL database) where T : class, new()
{
    private readonly MySQL _database = database ?? throw new ArgumentNullException(nameof(database));
    private string? _table;
    private IEnumerable<T>? _results;
    private readonly List<string> _conditions = [];
    private string? _orderBy;
    private string? _orderType;
    private int? _limit;

    /// <summary>
    /// Specifies the table to select data from.
    /// </summary>
    /// <param name="table">The name of the table to select from.</param>
    /// <returns>The current <see cref="SerializedSelectQuery{T}"/> instance for method chaining.</returns>
    public SerializedSelectQuery<T> From(string table)
    {
        Validate(table, ValidateType.Table); // Ensures the table name is valid.
        _table = table;
        return this;
    }

    /// <summary>
    /// Adds a WHERE condition to the query.
    /// </summary>
    /// <param name="condition">The condition to apply in the WHERE clause.</param>
    /// <returns>The current <see cref="SerializedSelectQuery{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the condition is null or empty.</exception>
    public SerializedSelectQuery<T> Where(string condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
        {
            throw new ArgumentException("Condition cannot be null or whitespace.", nameof(condition));
        }
        _conditions.Add(condition);
        return this;
    }

    /// <summary>
    /// Adds an ORDER BY clause to the query.
    /// </summary>
    /// <param name="column">The column to order by.</param>
    /// <param name="orderType">The order type (ASC or DESC, default is ASC).</param>
    /// <returns>The current <see cref="SerializedSelectQuery{T}"/> instance for method chaining.</returns>
    public SerializedSelectQuery<T> OrderBy(string column, string orderType = "ASC")
    {
        _orderBy = column;
        _orderType = orderType.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? "DESC" : "ASC";
        return this;
    }

    /// <summary>
    /// Adds a LIMIT clause to the query.
    /// </summary>
    /// <param name="limit">The maximum number of records to return.</param>
    /// <returns>The current <see cref="SerializedSelectQuery{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the limit is less than or equal to zero.</exception>
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
    public async Task<IEnumerable<T>> ReadAsync()
    {
        string whereClause = _conditions.Any() ? "WHERE " + string.Join(" AND ", _conditions) : string.Empty;
        string orderByClause = _orderBy != null ? $"ORDER BY {_orderBy} {_orderType}" : string.Empty;
        string limitClause = _limit.HasValue ? $"LIMIT {_limit.Value}" : string.Empty;

        string query = $"SELECT * FROM {_table} {whereClause} {orderByClause} {limitClause}".Trim();

        _results = await _database.ExecuteQueryAsync<T>(query);
        return _results;
    }
}
