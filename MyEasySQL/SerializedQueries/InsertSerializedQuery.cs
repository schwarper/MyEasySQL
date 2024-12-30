using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyEasySQL.SerializedQueries;

/// <summary>
/// Provides functionality to build and execute INSERT queries on a specified table based on a class type.
/// This class dynamically builds an SQL INSERT statement using the properties of the specified class.
/// </summary>
/// <typeparam name="T">The class type representing the table schema.</typeparam>
public class InsertSerializedQuery<T> where T : class, new()
{
    private readonly MySQL _database;
    private readonly string _table;
    private readonly Dictionary<string, object> _values = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="InsertSerializedQuery{T}"/> class.
    /// </summary>
    /// <param name="database">The database instance to execute the query on.</param>
    /// <param name="table">The name of the table to insert data into.</param>
    /// <param name="instance">The object containing data to insert.</param>
    /// <exception cref="ArgumentNullException">Thrown when the database or instance is null.</exception>
    public InsertSerializedQuery(MySQL database, string table, T instance)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table ?? throw new ArgumentNullException(nameof(table));

        ArgumentNullException.ThrowIfNull(instance);

        SetValuesFromObject(instance);
    }

    /// <summary>
    /// Adds values from a given object to the INSERT query.
    /// This method extracts property values from the object and associates them with the corresponding table columns.
    /// </summary>
    /// <param name="entity">The object containing the values to insert.</param>
    private void SetValuesFromObject(T entity)
    {
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo prop in properties)
        {
            ColumnAttribute? columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
            string columnName = columnAttr?.Name ?? prop.Name;

            object value = prop.GetValue(entity) ?? DBNull.Value;
            _values[columnName] = value;
        }
    }

    /// <summary>
    /// Executes the INSERT query asynchronously.
    /// This method constructs the INSERT SQL statement and sends it to the database for execution.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. 
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

        string query = $"INSERT INTO {_table} ({columns}) VALUES ({paramNames});";
        return await _database.ExecuteNonQueryAsync(query, _values);
    }
}
