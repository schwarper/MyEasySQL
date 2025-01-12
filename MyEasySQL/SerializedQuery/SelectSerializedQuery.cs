using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyEasySQL.Attribute;
using MyEasySQL.Model;

namespace MyEasySQL.SerializedQuery;

/// <summary>
/// Represents a serialized query for selecting entities from a table.
/// </summary>
/// <typeparam name="T">The entity type to select.</typeparam>
public class SelectSerializedQuery<T> : BaseQuery
{
    /// <summary>
    /// The column by which the results will be ordered.
    /// </summary>
    private string? _orderBy;

    /// <summary>
    /// The order direction (ascending or descending) for the results.
    /// </summary>
    private OrderType? _orderType;

    /// <summary>
    /// The maximum number of results to return.
    /// </summary>
    private int? _limit;

    /// <summary>
    /// List of conditions to filter rows for the select operation.
    /// </summary>
    private readonly List<string> _conditions = [];

    /// <summary>
    /// List of columns to include in the result set.
    /// </summary>
    private readonly List<string> _columns = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectSerializedQuery{T}"/> class.
    /// </summary>
    public SelectSerializedQuery() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectSerializedQuery{T}"/> class with a database instance.
    /// </summary>
    /// <param name="database">The database instance to associate with the query.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public SelectSerializedQuery(MySQL database) : base(database) { }

    /// <summary>
    /// Sets the table name for the select operation.
    /// </summary>
    /// <param name="table">The name of the table to select from.</param>
    /// <returns>The current <see cref="SelectSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public SelectSerializedQuery<T> From(string table)
    {
        SetTable(table);
        return this;
    }

    /// <summary>
    /// Sets the table name based on the entity type's <see cref="TableNameAttribute"/>.
    /// </summary>
    /// <returns>The current <see cref="SelectSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table is not defined for the entity type.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public SelectSerializedQuery<T> FromEntityTable()
    {
        base.SetTableFromEntity<T>();
        return this;
    }

    /// <summary>
    /// Adds a condition to filter rows for the select operation.
    /// </summary>
    /// <param name="predicate">An expression representing the condition to apply.</param>
    /// <returns>The current <see cref="SelectSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the predicate expression is null.</exception>
    public SelectSerializedQuery<T> Where(Expression<Func<T, bool>> predicate)
    {
        MyEasySQLException.ThrowIfPredicateNull(predicate);

        string condition = Helper.ParseExpressionWhere(predicate.Body);
        _conditions.Add(condition);
        return this;
    }

    /// <summary>
    /// Specifies the columns to include in the result set.
    /// </summary>
    /// <param name="columns">The column names to include.</param>
    /// <returns>The current <see cref="SelectSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if no columns are specified or columns names are empty.</exception>
    public SelectSerializedQuery<T> Select(params string[] columns)
    {
        MyEasySQLException.ThrowIfColumnIsEmpty(columns);

        foreach (string column in columns)
        {
            MyEasySQLException.ThrowIfColumnNameIsEmpty(column);
            _columns.Add(column);
        }

        return this;
    }

    /// <summary>
    /// Specifies the column and order direction for sorting the results.
    /// </summary>
    /// <typeparam name="TKey">The type of the key to order by.</typeparam>
    /// <param name="keySelector">An expression selecting the column to order by.</param>
    /// <param name="orderType">The order direction (ascending or descending).</param>
    /// <returns>The current <see cref="SelectSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the key selector is not a member expression.</exception>
    public SelectSerializedQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, OrderType orderType)
    {
        MyEasySQLException.ThrowIfKeySelectorNotMember(keySelector.Body);

        _orderBy = ((MemberExpression)keySelector.Body).Member.Name;
        _orderType = orderType;

        return this;
    }

    /// <summary>
    /// Limits the number of results to return.
    /// </summary>
    /// <param name="limit">The maximum number of results.</param>
    /// <returns>The current <see cref="SelectSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the limit is less than or equal to zero.</exception>
    public SelectSerializedQuery<T> Limit(int limit)
    {
        MyEasySQLException.ThrowIfLessOrEqualToZero(limit);

        _limit = limit;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously to retrieve entities from the database.
    /// </summary>
    /// <returns>An enumerable of the selected entities.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public async Task<IEnumerable<T>> ReadAsync()
    {
        MyEasySQLException.ThrowIfDatabaseNotInitialized(_database);

        string query = ToString();
        IEnumerable<T> result = await _database!.ExecuteQueryAsync<T>(query);
        return result;
    }

    /// <summary>
    /// Generates the SQL query string for the select operation.
    /// </summary>
    /// <returns>The SQL query string.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table name is empty.</exception>
    public override string ToString()
    {
        MyEasySQLException.ThrowIfTableNameIsEmpty(_table);

        StringBuilder builder = new();
        builder.Append("SELECT ");

        builder.Append(_columns.Count > 0 ? string.Join(", ", _columns) : "*");
        builder.Append($" FROM `{_table}`");

        if (_conditions.Count > 0)
        {
            builder.Append(" WHERE ");
            builder.Append(string.Join(" AND ", _conditions));
        }

        if (!string.IsNullOrWhiteSpace(_orderBy) && _orderType.HasValue)
        {
            builder.Append($" ORDER BY `{_orderBy}` {_orderType.Value}");
        }

        if (_limit.HasValue)
        {
            builder.Append($" LIMIT {_limit.Value}");
        }

        return builder.Append(';').ToString().Trim();
    }
}