using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyEasySQL.SerializedQueries;
using static MyEasySQL.Utils.Validator;

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
    /// <exception cref="ArgumentException">Thrown if the <paramref name="table"/> name is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="database"/> is null.</exception>
    public InsertQuery(MySQL database, string table)
    {
        ValidateName(table, ValidateType.Table);

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table;
    }

    /// <summary>
    /// Adds a column and its corresponding value to the INSERT query.
    /// </summary>
    /// <param name="column">The name of the column to insert data into.</param>
    /// <param name="value">The value to insert into the specified column.</param>
    /// <returns>The current <see cref="InsertQuery"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="column"/> name is invalid.</exception>
    public InsertQuery Value(string column, object value)
    {
        ValidateName(column, ValidateType.Column);

        _values[column] = value ?? DBNull.Value;
        return this;
    }

    /// <summary>
    /// Specifies the columns and values to update in case of a duplicate key.
    /// </summary>
    /// <param name="column">The column to update.</param>
    /// <param name="value">The value to update the column with.</param>
    /// <returns>The current <see cref="InsertQuery"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="column"/> name or <paramref name="value"/> is invalid.</exception>
    public InsertQuery OnDuplicateKeyUpdate(string column, object value)
    {
        ValidateName(column, ValidateType.Column);

        if (value is string valueStr)
        {
            ValidateUpdateKey(valueStr);
            _updateParameters[column] = valueStr;
        }
        else
        {
            _updateParameters[column] = value ?? DBNull.Value;
        }

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
    /// <exception cref="InvalidOperationException">Thrown if no values are specified for the insert operation.</exception>
    public async Task<int> ExecuteAsync()
    {
        if (_values.Count == 0)
        {
            throw new InvalidOperationException("No values specified for insert operation.");
        }

        StringBuilder builder = new();
        builder.Append($"INSERT INTO {_table} ");

        string columns = string.Join(", ", _values.Keys);
        string paramNames = string.Join(", ", _values.Keys.Select(k => $"@{k}"));
        builder.Append($"({columns}) VALUES ({paramNames})");

        if (_updateParameters.Count > 0)
        {
            builder.Append(" ON DUPLICATE KEY UPDATE ");
            builder.Append(string.Join(", ", _updateParameters.Keys.Select(k => $"{k} = {_updateParameters[k]}")));
        }

        builder.Append(';');

        string query = builder.ToString();

        return await _database.ExecuteNonQueryAsync(query);
    }
}