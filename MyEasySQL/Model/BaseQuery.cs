using System.Reflection;
using MyEasySQL.Attribute;

namespace MyEasySQL.Model;

/// <summary>
/// Abstract base class for constructing and managing database queries.
/// </summary>
public abstract class BaseQuery
{
    /// <summary>
    /// The database instance associated with this query.
    /// </summary>
    protected MySQL? _database;

    /// <summary>
    /// The name of the database table for the query.
    /// </summary>
    protected string? _table;

    /// <summary>
    /// Default constructor for initializing a base query without parameters.
    /// </summary>
    protected BaseQuery() { }

    /// <summary>
    /// Constructor for initializing a base query with a table name.
    /// </summary>
    /// <param name="table">The name of the database table.</param>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    protected BaseQuery(string table) => SetTable(table);

    /// <summary>
    /// Constructor for initializing a base query with a database instance.
    /// </summary>
    /// <param name="database">The database instance.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    protected BaseQuery(MySQL database) => SetDatabase(database);

    /// <summary>
    /// Constructor for initializing a base query with both a database instance and table name.
    /// </summary>
    /// <param name="database">The database instance.</param>
    /// <param name="table">The name of the database table.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    protected BaseQuery(MySQL database, string table)
    {
        SetDatabase(database);
        SetTable(table);
    }

    /// <summary>
    /// Sets the database instance for this query.
    /// </summary>
    /// <param name="database">The database instance to associate with this query.</param>
    /// <exception cref="MyEasySQLException">Thrown if the database instance is not initialized.</exception>
    protected void SetDatabase(MySQL database)
    {
        MyEasySQLException.ThrowIfDatabaseNotInitialized(database);
        _database = database;
    }

    /// <summary>
    /// Sets the table name for this query.
    /// </summary>
    /// <param name="table">The name of the database table.</param>
    /// <exception cref="MyEasySQLException">Thrown if the table name is invalid or already defined.</exception>
    protected void SetTable(string table)
    {
        MyEasySQLException.ThrowIfTableIsAlreadyDefined(_table);
        MyEasySQLException.ThrowIfInvalidName(table);
        _table = table;
    }

    /// <summary>
    /// Sets the table name for this query based on an entity type's <see cref="TableNameAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The entity type associated with the table.</typeparam>
    /// <exception cref="MyEasySQLException">Thrown if the table is not defined for the entity type.</exception>
    public virtual void SetTableFromEntity<T>()
    {
        MyEasySQLException.ThrowIfTableIsNotDefined<T>();
        SetTable(typeof(T).GetCustomAttribute<TableNameAttribute>()!.Value);
    }
}