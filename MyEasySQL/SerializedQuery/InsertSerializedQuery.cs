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
/// Represents a serialized query for inserting an entity into a table.
/// </summary>
/// <typeparam name="T">The entity type to insert.</typeparam>
/// <param name="instance">The instance of the entity to insert.</param>
public class InsertSerializedQuery<T>(T instance) : BaseQuery
{
    /// <summary>
    /// Dictionary containing column names and their corresponding values for the insert operation.
    /// </summary>
    private readonly Dictionary<string, object> _values = Helper.GetValuesFromEntity(instance);

    /// <summary>
    /// Dictionary containing column names and their corresponding values for the ON DUPLICATE KEY UPDATE clause.
    /// </summary>
    private readonly Dictionary<string, string> _updateValues = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="InsertSerializedQuery{T}"/> class with a specified table name and entity instance.
    /// </summary>
    /// <param name="database">The database instance to associate with the query.</param>
    /// <param name="instance">The instance of the entity to insert.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public InsertSerializedQuery(MySQL database, T instance) : this(instance)
    {
        SetDatabase(database);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InsertSerializedQuery{T}"/> class with a specified table name and entity instance.
    /// </summary>
    /// <param name="table">The name of the table to insert into.</param>
    /// <param name="instance">The instance of the entity to insert.</param>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public InsertSerializedQuery(string table, T instance) : this(instance)
    {
        SetTable(table);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InsertSerializedQuery{T}"/> class with a database, table name, and entity instance.
    /// </summary>
    /// <param name="database">The database instance to associate with the query.</param>
    /// <param name="table">The name of the table to insert into.</param>
    /// <param name="instance">The instance of the entity to insert.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public InsertSerializedQuery(MySQL database, string table, T instance) : this(instance)
    {
        SetDatabase(database);
        SetTable(table);
    }

    /// <summary>
    /// Sets the table name based on the entity type's <see cref="TableNameAttribute"/>.
    /// </summary>
    /// <returns>The current <see cref="InsertSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table is not defined for the entity type.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public InsertSerializedQuery<T> SetTableFromEntity()
    {
        base.SetTableFromEntity<T>();
        return this;
    }

    /// <summary>
    /// Configures the query to update specified columns if a duplicate key is encountered during the insert operation.
    /// </summary>
    /// <param name="expression">An expression that specifies the columns to update and their new values.</param>
    /// <returns>The current <see cref="InsertSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the expression does not initialize object properties.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the binding is not a member assaignment.</exception>
    public InsertSerializedQuery<T> OnDuplicateKeyUpdate(Expression<Func<T, T>> expression)
    {
        MyEasySQLException.ThrowIfNotInitializedObjectProperties(expression.Body);

        MemberInitExpression body = (MemberInitExpression)expression.Body;
        foreach (MemberBinding binding in body.Bindings)
        {
            MyEasySQLException.ThrowIfNotMemberAssignment(binding);

            MemberAssignment memberAssignment = ((MemberAssignment)binding);
            string memberName = memberAssignment.Member.Name;
            string value = Helper.ParseExpressionInsert(memberAssignment.Expression);
            _updateValues[memberName] = value;
        }

        return this;
    }

    /// <summary>
    /// Executes the query asynchronously to insert the entity into the database.
    /// </summary>
    /// <returns>The number of rows affected by the query.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public async Task<int> ExecuteAsync()
    {
        MyEasySQLException.ThrowIfDatabaseNotInitialized(_database);

        return await _database!.ExecuteNonQueryAsync(ToString(), _values);
    }

    /// <summary>
    /// Generates the SQL query string for the insert operation, including the ON DUPLICATE KEY UPDATE clause if specified.
    /// </summary>
    /// <returns>The SQL query string.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the fields for the insert operation are not valid.</exception>
    public override string ToString()
    {
        MyEasySQLException.ThrowIfFieldNotCorrect(_values);

        StringBuilder query = new();

        query.Append($"INSERT INTO `{_table}` (\n");
        query.Append(string.Join(", ", _values.Keys.Select(key => $"`{key}`")));
        query.Append(") VALUES (\n");
        query.Append(string.Join(", ", _values.Values.Select(Helper.FormatValue)));
        query.Append(')');

        if (_updateValues.Count > 0)
        {
            query.Append(" ON DUPLICATE KEY UPDATE ");
            query.Append(string.Join(", ", _updateValues.Select(kv => $"`{kv.Key}` = {kv.Value}")));
        }

        query.Append(';');
        return query.ToString();
    }
}