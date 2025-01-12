using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyEasySQL.Attribute;
using MyEasySQL.Model;

namespace MyEasySQL.SerializedQuery;

/// <summary>
/// Represents a serialized query for deleting entities from a table.
/// </summary>
/// <typeparam name="T">The entity type to delete.</typeparam>
public class DeleteSerializedQuery<T> : BaseQuery
{
    /// <summary>
    /// List of conditions to filter rows for the delete operation.
    /// </summary>
    private readonly List<string> _conditions = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSerializedQuery{T}"/> class.
    /// </summary>
    public DeleteSerializedQuery() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSerializedQuery{T}"/> class with a database instance.
    /// </summary>
    /// <param name="database">The database instance to associate with the query.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public DeleteSerializedQuery(MySQL database) : base(database) { }

    /// <summary>
    /// Sets the table name for the delete operation.
    /// </summary>
    /// <param name="table">The name of the table to delete from.</param>
    /// <returns>The current <see cref="DeleteSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public DeleteSerializedQuery<T> From(string table)
    {
        SetTable(table);
        return this;
    }

    /// <summary>
    /// Sets the table name based on the entity type's <see cref="TableNameAttribute"/>.
    /// </summary>
    /// <returns>The current <see cref="DeleteSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table is not defined for the entity type.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public DeleteSerializedQuery<T> FromEntityTable()
    {
        base.SetTableFromEntity<T>();
        return this;
    }

    /// <summary>
    /// Adds a condition to filter rows for the delete operation.
    /// </summary>
    /// <param name="predicate">An expression representing the condition to apply.</param>
    /// <returns>The current <see cref="DeleteSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the predicate expression is null.</exception>
    public DeleteSerializedQuery<T> Where(Expression<Func<T, bool>> predicate)
    {
        MyEasySQLException.ThrowIfPredicateNull(predicate);

        string condition = Helper.ParseExpressionWhere(predicate.Body);
        _conditions.Add(condition);
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously to delete entities from the database.
    /// </summary>
    /// <returns>The number of rows affected by the query.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public async Task<int> ExecuteAsync()
    {
        MyEasySQLException.ThrowIfDatabaseNotInitialized(_database);

        string query = ToString();
        int result = await _database!.ExecuteNonQueryAsync(query);
        return result;
    }

    /// <summary>
    /// Generates the SQL query string for the delete operation.
    /// </summary>
    /// <returns>The SQL query string.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table name is empty.</exception>
    public override string ToString()
    {
        MyEasySQLException.ThrowIfTableNameIsEmpty(_table);

        StringBuilder builder = new();
        builder.Append($"DELETE FROM `{_table}`\n");

        if (_conditions.Count > 0)
        {
            builder.Append(" WHERE ");
            builder.Append(string.Join(" AND ", _conditions));
        }

        return builder.Append(';').ToString().Trim();
    }
}