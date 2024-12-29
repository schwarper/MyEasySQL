using Dapper;
using MySqlConnector;
using MyEasySQL.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MyEasySQL.Utils.RegexUtil;
using System;

namespace MyEasySQL;

/// <summary>
/// Provides a wrapper for MySQL operations using Dapper for easy database management.
/// </summary>
public class MySQL
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySQL"/> class with a connection string.
    /// </summary>
    /// <param name="connectionString">The MySQL connection string.</param>
    public MySQL(string connectionString) => _connectionString = connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySQL"/> class with a connection string builder.
    /// </summary>
    /// <param name="builder">The MySQL connection string builder.</param>
    public MySQL(MySqlConnectionStringBuilder builder) => _connectionString = builder.ConnectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySQL"/> class with individual connection parameters.
    /// </summary>
    /// <param name="host">The MySQL server host.</param>
    /// <param name="name">The database name.</param>
    /// <param name="user">The username for authentication.</param>
    /// <param name="password">The password for authentication.</param>
    /// <param name="port">The port number (default is 3306).</param>
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
    /// Initializes a new instance of the <see cref="CreateTableQuery"/> class for creating a table.
    /// </summary>
    /// <param name="table">The name of the table to create.</param>
    /// <returns>An instance of <see cref="CreateTableQuery"/>.</returns>
    public CreateTableQuery CreateTable(string table) => new(this, table);

    /// <summary>
    /// Initializes a new instance of the <see cref="InsertQuery"/> class for inserting data.
    /// </summary>
    /// <param name="table">The name of the table to insert data into.</param>
    /// <returns>An instance of <see cref="InsertQuery"/>.</returns>
    public InsertQuery Insert(string table) => new(this, table);

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateQuery"/> class for updating data.
    /// </summary>
    /// <param name="table">The name of the table to update data in.</param>
    /// <returns>An instance of <see cref="UpdateQuery"/>.</returns>
    public UpdateQuery Update(string table) => new(this, table);

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectQuery"/> class for selecting data.
    /// </summary>
    /// <param name="columns">The columns to select.</param>
    /// <returns>An instance of <see cref="SelectQuery"/>.</returns>
    public SelectQuery Select(params string[] columns) => new(this, columns);

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteQuery"/> class for deleting data.
    /// </summary>
    /// <returns>An instance of <see cref="DeleteQuery"/>.</returns>
    public DeleteQuery Delete() => new(this);

    /// <summary>
    /// Creates a new database with the specified name.
    /// </summary>
    /// <param name="name">The name of the database to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="MySqlException">Thrown when there is an error in the database creation process.</exception>
    /// <exception cref="ArgumentException">Thrown when the database name is invalid.</exception>
    public async Task CreateDatabase(string name)
    {
        Validate(name, ValidateType.Database);
        await ExecuteNonQueryAsync($"CREATE DATABASE {name}");
    }

    /// <summary>
    /// Drops the specified database.
    /// </summary>
    /// <param name="database">The name of the database to drop.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="MySqlException">Thrown when there is an error in the database dropping process.</exception>
    /// <exception cref="ArgumentException">Thrown when the database name is invalid.</exception>
    public async Task DropDatabase(string database)
    {
        Validate(database, ValidateType.Database);
        await ExecuteNonQueryAsync($"DROP DATABASE {database}");
    }

    /// <summary>
    /// Drops the specified table.
    /// </summary>
    /// <param name="table">The name of the table to drop.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="MySqlException">Thrown when there is an error in the table dropping process.</exception>
    /// <exception cref="ArgumentException">Thrown when the table name is invalid.</exception>
    public async Task DropTable(string table)
    {
        Validate(table, ValidateType.Table);
        await ExecuteNonQueryAsync($"DROP TABLE {table}");
    }

    /// <summary>
    /// Gets an open MySQL connection asynchronously.
    /// </summary>
    /// <returns>An open <see cref="MySqlConnection"/>.</returns>
    /// <exception cref="MySqlException">Thrown when there is an error in opening the connection.</exception>
    internal async Task<MySqlConnection> GetOpenConnectionAsync()
    {
        MySqlConnection connection = new(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    /// <summary>
    /// Executes a query asynchronously and returns the result as a collection of objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the result objects.</typeparam>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">The parameters for the query (optional).</param>
    /// <returns>A collection of objects of type <typeparamref name="T"/>.</returns>
    /// <exception cref="MySqlException">Thrown when there is an error in query execution.</exception>
    internal async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query, object? parameters = null)
    {
        await using MySqlConnection connection = await GetOpenConnectionAsync();
        return await connection.QueryAsync<T>(query, parameters);
    }

    /// <summary>
    /// Executes a non-query SQL command asynchronously.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">The parameters for the query (optional).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="MySqlException">Thrown when there is an error in query execution.</exception>
    internal async Task ExecuteNonQueryAsync(string query, object? parameters = null)
    {
        await using MySqlConnection connection = await GetOpenConnectionAsync();
        await connection.ExecuteAsync(query, parameters);
    }
}
