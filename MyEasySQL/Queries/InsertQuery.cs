using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MyEasySQL.Utils.RegexUtil;

namespace MyEasySQL.Queries;

/// <summary>
/// Represents an INSERT query builder for inserting data into a specified table.
/// Provides methods to specify columns and values, and execute the query asynchronously.
/// </summary>
public class InsertQuery
{
    private readonly MySQL _database;
    private readonly string _table;
    private readonly Dictionary<string, object> _values = [];
    private readonly Dictionary<string, object> _updateParameters = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="InsertQuery"/> class.
    /// </summary>
    /// <param name="database">The database instance to execute the query on.</param>
    /// <param name="table">The name of the table to insert data into.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="database"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="table"/> name is invalid.</exception>
    public InsertQuery(MySQL database, string table)
    {
        Validate(table, ValidateType.Table);

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table;
    }

    /// <summary>
    /// Adds a column and its corresponding value to the INSERT query.
    /// </summary>
    /// <param name="column">The name of the column to insert data into.</param>
    /// <param name="value">The value to insert into the specified column.</param>
    /// <returns>The current <see cref="InsertQuery"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="column"/> name is invalid.</exception>
    public InsertQuery Value(string column, object value)
    {
        Validate(column, ValidateType.Column);

        _values[column] = value ?? DBNull.Value;
        return this;
    }

    /// <summary>
    /// Specifies the columns and values to update in case of a duplicate key.
    /// </summary>
    /// <param name="column">The column to update.</param>
    /// <param name="value">The value to update the column with.</param>
    /// <returns>The current <see cref="InsertQuery"/> instance, allowing for method chaining.</returns>
    public InsertQuery OnDuplicateKeyUpdate(string column, object value)
    {
        Validate(column, ValidateType.Column);

        _updateParameters[column] = value ?? DBNull.Value;
        return this;
    }

    /// <summary>
    /// Executes the INSERT query asynchronously and inserts the specified data into the table.
    /// If duplicate keys are found, updates the specified columns.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the number of rows affected by the command.
    /// The task result contains the number of rows affected by the command.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when no values are specified for the insert operation.</exception>
    public async Task<int> ExecuteAsync()
    {
        if (_values.Count == 0)
        {
            throw new InvalidOperationException("No values specified for insert operation.");
        }

        string columns = string.Join(", ", _values.Keys);
        string paramNames = string.Join(", ", _values.Keys.Select(k => $"@{k}"));
        string query = $"INSERT INTO {_table} ({columns}) VALUES ({paramNames})";

        if (_updateParameters.Count > 0)
        {
            string updateClause = string.Join(", ", _updateParameters.Keys.Select(k => $"{k} = @{k}_update"));

            query += $" ON DUPLICATE KEY UPDATE {updateClause}";

            foreach (var key in _updateParameters.Keys)
            {
                _values[$"{key}_update"] = _updateParameters[key];
            }
        }

        query += ";";
        return await _database.ExecuteNonQueryAsync(query, _values);
    }
}