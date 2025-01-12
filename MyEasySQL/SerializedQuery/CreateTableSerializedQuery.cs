using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyEasySQL.Attribute;
using MyEasySQL.Model;

namespace MyEasySQL.SerializedQuery;

/// <summary>
/// Represents a serialized query for creating a table in the database based on an entity type.
/// </summary>
/// <typeparam name="T">The entity type to define the table structure.</typeparam>
public class CreateTableSerializedQuery<T> : BaseQuery
{
    /// <summary>
    /// List of column definitions generated from the entity type.
    /// </summary>
    /// <exception cref="MyEasySQLException">Thrown if fields name are not supported.</exception>
    private readonly List<string> _columns = Helper.GenerateColumnsFromType<T>();

    /// <summary>
    /// Indicates whether the table should be created only if it does not already exist.
    /// </summary>
    private bool _ifNotExist = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableSerializedQuery{T}"/> class.
    /// </summary>
    public CreateTableSerializedQuery() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableSerializedQuery{T}"/> class with a specified table name.
    /// </summary>
    /// <param name="table">The name of the table to create.</param>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public CreateTableSerializedQuery(string table) : base(table) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableSerializedQuery{T}"/> class with a database.
    /// </summary>
    /// <param name="database">The database instance to associate with the query.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public CreateTableSerializedQuery(MySQL database) : base(database) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableSerializedQuery{T}"/> class with a database and table name.
    /// </summary>
    /// <param name="database">The database instance to associate with the query.</param>
    /// <param name="table">The name of the table to create.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public CreateTableSerializedQuery(MySQL database, string table) : base(database, table) { }

    /// <summary>
    /// Sets the table name based on the entity type's <see cref="TableNameAttribute"/>.
    /// </summary>
    /// <returns>The current <see cref="CreateTableSerializedQuery{T}"/> instance.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table is not defined for the entity type.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    public CreateTableSerializedQuery<T> SetTableFromEntity()
    {
        base.SetTableFromEntity<T>();
        return this;
    }

    /// <summary>
    /// Configures the query to create the table only if it does not already exist.
    /// </summary>
    /// <param name="value">If <c>true</c>, adds "IF NOT EXISTS" to the query.</param>
    /// <returns>The current <see cref="CreateTableSerializedQuery{T}"/> instance.</returns>
    public CreateTableSerializedQuery<T> SetIfNotExist(bool value)
    {
        _ifNotExist = value;
        return this;
    }

    /// <summary>
    /// Executes the query asynchronously to create the table in the database.
    /// </summary>
    /// <returns>The number of rows affected by the query.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    public async Task<int> ExecuteAsync()
    {
        MyEasySQLException.ThrowIfDatabaseNotInitialized(_database);

        return await _database!.ExecuteNonQueryAsync(ToString());
    }

    /// <summary>
    /// Generates the SQL query string for creating the table.
    /// </summary>
    /// <returns>The SQL query string.</returns>
    /// <exception cref="MyEasySQLException">Thrown if the table name is empty or null.</exception>
    public override string ToString()
    {
        MyEasySQLException.ThrowIfTableNameIsEmpty(_table);

        StringBuilder query = new();
        query.Append($"CREATE TABLE {(_ifNotExist ? "IF NOT EXISTS " : "")}`{_table}` (\n");
        query.Append(string.Join(",\n", _columns));
        query.Append("\n);");
        return query.ToString();
    }
}