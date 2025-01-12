using MyEasySQL.Model;
using MyEasySQL.SerializedQuery;
using MySqlConnector;

namespace MyEasySQL;

/// <summary>
/// Represents a MySQL database connection and provides methods for creating and executing queries.
/// </summary>
public class MySQL : BaseDatabase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MySQL"/> class using a connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to connect to the MySQL database.</param>
    public MySQL(string connectionString) => _connectionString = connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySQL"/> class using a connection string builder.
    /// </summary>
    /// <param name="builder">The <see cref="MySqlConnectionStringBuilder"/> containing the connection details.</param>
    public MySQL(MySqlConnectionStringBuilder builder) => _connectionString = builder.ConnectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySQL"/> class using individual connection parameters.
    /// </summary>
    /// <param name="host">The hostname or IP address of the MySQL server.</param>
    /// <param name="name">The name of the database.</param>
    /// <param name="user">The username for authentication.</param>
    /// <param name="password">The password for authentication.</param>
    /// <param name="port">The port number of the MySQL server (default is 3306).</param>
    public MySQL(string host, string name, string user, string password, uint port = 3306) =>
        _connectionString = new MySqlConnectionStringBuilder()
        {
            Server = host,
            Database = name,
            UserID = user,
            Password = password,
            Port = port
        }
        .ConnectionString;

    /// <summary>
    /// Creates a new serialized query for creating a table based on a specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type to base the table structure on.</typeparam>
    /// <returns>A <see cref="CreateTableSerializedQuery{T}"/> instance.</returns>
    public CreateTableSerializedQuery<T> CreateTableSerialized<T>() where T : class, new() => new(this);

    /// <summary>
    /// Creates a new serialized query for creating a table based on a specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type to base the table structure on.</typeparam>
    /// <param name="table">The name of the table to create.</param>
    /// <returns>A <see cref="CreateTableSerializedQuery{T}"/> instance.</returns>
    public CreateTableSerializedQuery<T> CreateTableSerialized<T>(string table) where T : class, new() => new(this, table);

    /// <summary>
    /// Creates a new serialized query for inserting an entity into a table.
    /// </summary>
    /// <typeparam name="T">The entity type to insert.</typeparam>
    /// <param name="instance">The instance of the entity to insert.</param>
    /// <returns>An <see cref="InsertSerializedQuery{T}"/> instance.</returns>
    public InsertSerializedQuery<T> InsertSerialized<T>(T instance) where T : class, new() => new(this, instance);

    /// <summary>
    /// Creates a new serialized query for inserting an entity into a table.
    /// </summary>
    /// <typeparam name="T">The entity type to insert.</typeparam>
    /// <param name="table">The name of the table to insert into.</param>
    /// <param name="instance">The instance of the entity to insert.</param>
    /// <returns>An <see cref="InsertSerializedQuery{T}"/> instance.</returns>
    public InsertSerializedQuery<T> InsertSerialized<T>(string table, T instance) where T : class, new() => new(this, table, instance);

    /// <summary>
    /// Creates a new serialized query for updating entities in a table.
    /// </summary>
    /// <typeparam name="T">The entity type to update.</typeparam>
    /// <returns>An <see cref="UpdateSerializedQuery{T}"/> instance.</returns>
    public UpdateSerializedQuery<T> UpdateSerialized<T>() where T : class, new() => new(this);

    /// <summary>
    /// Creates a new serialized query for updating entities in a table.
    /// </summary>
    /// <typeparam name="T">The entity type to update.</typeparam>
    /// <param name="table">The name of the table to update.</param>
    /// <returns>An <see cref="UpdateSerializedQuery{T}"/> instance.</returns>
    public UpdateSerializedQuery<T> UpdateSerialized<T>(string table) where T : class, new() => new(this, table);

    /// <summary>
    /// Creates a new serialized query for selecting entities from a table.
    /// </summary>
    /// <typeparam name="T">The entity type to select.</typeparam>
    /// <returns>A <see cref="SelectSerializedQuery{T}"/> instance.</returns>
    public SelectSerializedQuery<T> SelectSerialized<T>() where T : class, new() => new(this);

    /// <summary>
    /// Creates a new serialized query for deleting entities from a table.
    /// </summary>
    /// <typeparam name="T">The entity type to delete.</typeparam>
    /// <returns>A <see cref="DeleteSerializedQuery{T}"/> instance.</returns>
    public DeleteSerializedQuery<T> DeleteSerialized<T>() where T : class, new() => new(this);
}