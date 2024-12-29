using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MyEasySQL.Utils.RegexUtil;

namespace MyEasySQL.Queries;

/// <summary>
/// Provides functionality to build and execute INSERT queries on a specified table.
/// </summary>
public class InsertQuery
{
    private readonly MySQL _database;
    private readonly string _table;
    private readonly Dictionary<string, object> _values = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="InsertQuery"/> class.
    /// </summary>
    /// <param name="database">The database instance to execute the query on.</param>
    /// <param name="table">The name of the table to insert data into.</param>
    /// <exception cref="ArgumentNullException">Thrown when the database is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the table name is invalid.</exception>
    public InsertQuery(MySQL database, string table)
    {
        Validate(table, ValidateType.Table);

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table;
    }

    /// <summary>
    /// Adds a column and its value to the INSERT query.
    /// </summary>
    /// <param name="column">The name of the column.</param>
    /// <param name="value">The value to insert into the column.</param>
    /// <returns>The <see cref="InsertQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the column name is invalid.</exception>
    public InsertQuery Value(string column, object value)
    {
        Validate(column, ValidateType.Column);

        _values[column] = value ?? DBNull.Value;
        return this;
    }

    /// <summary>
    /// Executes the INSERT query asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no values are specified for the insert operation.</exception>
    public async Task ExecuteAsync()
    {
        if (_values.Count == 0)
        {
            throw new InvalidOperationException("No values specified for insert operation.");
        }

        string columns = string.Join(", ", _values.Keys);
        string paramNames = string.Join(", ", _values.Keys.Select(k => $"@{k}"));

        string query = $"INSERT INTO {_table} ({columns}) VALUES ({paramNames});";
        await _database.ExecuteNonQueryAsync(query, _values);
    }
}