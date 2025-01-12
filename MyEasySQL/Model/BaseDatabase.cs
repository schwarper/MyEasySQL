using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;

namespace MyEasySQL.Model;

/// <summary>
/// Abstract base class for managing database connections and executing queries.
/// </summary>
public abstract class BaseDatabase
{
    /// <summary>
    /// The connection string used to establish a database connection.
    /// </summary>
    protected string? _connectionString;

    /// <summary>
    /// Opens a new asynchronous connection to the MySQL database.
    /// </summary>
    /// <returns>An open <see cref="MySqlConnection"/> instance.</returns>
    internal async Task<MySqlConnection> GetOpenConnectionAsync()
    {
        MySqlConnection connection = new(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    /// <summary>
    /// Executes a query that retrieves data and maps it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to map the query result to.</typeparam>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional parameters for the query.</param>
    /// <returns>An enumerable of the mapped result type <typeparamref name="T"/>.</returns>
    internal async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query, object? parameters = null)
    {
        await using MySqlConnection connection = await GetOpenConnectionAsync();
        return await connection.QueryAsync<T>(query, parameters);
    }

    /// <summary>
    /// Executes a non-query SQL command.
    /// </summary>
    /// <param name="query">The SQL command to execute.</param>
    /// <param name="parameters">Optional parameters for the command.</param>
    /// <returns>The number of rows affected by the command.</returns>
    internal async Task<int> ExecuteNonQueryAsync(string query, object? parameters = null)
    {
        await using MySqlConnection connection = await GetOpenConnectionAsync();
        return await connection.ExecuteAsync(query, parameters);
    }
}