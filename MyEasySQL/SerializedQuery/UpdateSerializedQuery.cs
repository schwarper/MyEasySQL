using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyEasySQL.Attribute;
using MyEasySQL.Model;

namespace MyEasySQL.SerializedQuery;

/// <summary>
/// Represents a serialized query for updating entities in a table.
/// </summary>
/// <typeparam name="T">The entity type to update.</typeparam>
public class UpdateSerializedQuery<T> : BaseQuery
{
    /// <summary>
    /// Dictionary containing column names and their corresponding values for the update operation.
    /// </summary>
    private readonly Dictionary<string, string> _sets = [];

    /// <summary>
    /// List of conditions to filter rows for the update operation.
    /// </summary>
    private readonly List<string> _conditions = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSerializedQuery{T}"/> class with a specified table name.
    /// </summary>
    public UpdateSerializedQuery()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSerializedQuery{T}"/> class with a specified table name.
    /// </summary>
    /// <param name="table">The name of the table to update.</param>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public UpdateSerializedQuery(string table) : base(table)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSerializedQuery{T}"/> class with a database instance.
    /// </summary>
    /// <param name="database">The database instance to associate with the query.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public UpdateSerializedQuery(MySQL database) : base(database)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSerializedQuery{T}"/> class with a database instance and table name.
    /// </summary>
    /// <param name="database">The database instance to associate with the query.</param>
    /// <param name="table">The name of the table to update.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public UpdateSerializedQuery(MySQL database, string table) : base(database, table)
    {
    }

    /// <summary>
    /// Sets the table name based on the entity type's <see cref="TableNameAttribute"/>.
    /// </summary>
    /// <returns>The current <see cref="UpdateSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table is not defined for the entity type.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public UpdateSerializedQuery<T> SetTableFromEntity()
    {
        base.SetTableFromEntity<T>();
        return this;
    }

    /// <summary>
    /// Configures the columns and values to be updated.
    /// </summary>
    /// <param name="expression">An expression that specifies the columns to update and their new values.</param>
    /// <returns>The current <see cref="UpdateSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the expression does not initialize object properties.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the binding is not a member assignment.</exception>
    public UpdateSerializedQuery<T> Set(Expression<Func<T, T>> expression)
    {
        MyEasySQLException.ThrowIfNotInitializedObjectProperties(expression.Body);

        MemberInitExpression body = (MemberInitExpression)expression.Body;
        foreach (MemberBinding binding in body.Bindings)
        {
            MyEasySQLException.ThrowIfNotMemberAssignment(binding);

            MemberAssignment memberAssignment = (MemberAssignment)binding;
            string columnName = memberAssignment.Member.Name;
            string sqlValue = Helper.ParseExpressionUpdate(memberAssignment.Expression);
            _sets.Add(columnName, sqlValue);
        }

        return this;
    }

    /// <summary>
    /// Adds a condition to filter rows for the update operation.
    /// </summary>
    /// <param name="predicate">An expression representing the condition to apply.</param>
    /// <returns>The current <see cref="UpdateSerializedQuery{T}"/> instance.</returns>
    public UpdateSerializedQuery<T> Where(Expression<Func<T, bool>> predicate)
    {
        string condition = Helper.ParseExpressionWhere(predicate.Body);
        _conditions.Add(condition);
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously to update the entities in the database.
    /// </summary>
    /// <returns>The number of rows affected by the query.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public async Task<int> ExecuteAsync()
    {
        MyEasySQLException.ThrowIfDatabaseNotInitialized(_database);

        return await _database!.ExecuteNonQueryAsync(ToString());
    }

    /// <summary>
    /// Generates the SQL query string for the update operation.
    /// </summary>
    /// <returns>The SQL query string.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table name is empty or no columns are specified for the update.</exception>
    public override string ToString()
    {
        MyEasySQLException.ThrowIfTableNameIsEmpty(_table);
        MyEasySQLException.ThrowIfColumnIsEmpty(_sets);
        MyEasySQLException.ThrowIfFieldNotCorrect(_sets);

        StringBuilder query = new();
        query.Append($"UPDATE `{_table}` SET\n");
        query.Append(string.Join(", ", _sets.Select(kv => $"`{kv.Key}` = {kv.Value}")));

        if (_conditions.Count > 0)
        {
            query.Append(" WHERE ");
            query.Append(string.Join(" AND ", _conditions));
        }

        query.Append(';');
        return query.ToString();
    }
}