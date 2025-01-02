using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyEasySQL.Queries;
using static MyEasySQL.Utils.Validator;

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
    private readonly Dictionary<string, object> _updateParameters = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="InsertSerializedQuery{T}"/> class.
    /// </summary>
    /// <param name="database">The database instance to execute the query on.</param>
    /// <param name="table">The name of the table to insert data into.</param>
    /// <param name="instance">The object containing data to insert.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="table"/> name is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="database"/> or <paramref name="instance"/> is null.</exception>
    public InsertSerializedQuery(MySQL database, string table, T instance)
    {
        ValidateName(table, ValidateType.Table);
        ArgumentNullException.ThrowIfNull(instance);

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table;

        SetValuesFromObject(instance);
    }

    /// <summary>
    /// Specifies the columns and values to update in case of a duplicate key.
    /// </summary>
    /// <param name="column">The column to update.</param>
    /// <param name="value">The value to update the column with.</param>
    /// <returns>The current <see cref="InsertSerializedQuery{T}"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="column"/> name or <paramref name="value"/> is invalid.</exception>
    public InsertSerializedQuery<T> OnDuplicateKeyUpdate(string column, object value)
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
    /// A task that represents the asynchronous operation. 
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

    private void SetValuesFromObject(T entity)
    {
        FieldInfo[] properties = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo prop in properties)
        {
            ColumnAttribute? columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
            string columnName = columnAttr?.Name ?? prop.Name;

            object value = prop.GetValue(entity) ?? DBNull.Value;
            _values[columnName] = value;
        }
    }
}
