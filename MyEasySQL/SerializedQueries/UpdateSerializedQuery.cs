using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyEasySQL.Utils;
using static MyEasySQL.Utils.RegexUtil;

namespace MyEasySQL.SerializedQueries;

/// <summary>
/// Provides functionality to construct and execute an SQL UPDATE query for serialized objects.
/// </summary>
/// <typeparam name="T">The type of the object to serialize and update.</typeparam>
public class SerializedUpdateQuery<T> where T : class, new()
{
    private readonly MySQL _database;
    private readonly string _table;
    private readonly Dictionary<string, object> _sets = [];
    private readonly Dictionary<string, object> _parameters = [];
    private readonly ConditionBuilder _conditionBuilder = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SerializedUpdateQuery{T}"/> class.
    /// </summary>
    /// <param name="database">The database instance to execute the query on.</param>
    /// <param name="table">The name of the table to update.</param>
    public SerializedUpdateQuery(MySQL database, string table)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        Validate(table, ValidateType.Table);
        _table = table;
    }

    /// <summary>
    /// Sets the properties of an object to be updated in the database.
    /// </summary>
    /// <param name="entity">The object containing the properties and values to update.</param>
    /// <returns>The <see cref="SerializedUpdateQuery{T}"/> instance for method chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the object has no properties to update.</exception>
    internal SerializedUpdateQuery<T> SetObject(T entity)
    {
        FieldInfo[] properties = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo prop in properties)
        {
            ColumnAttribute? columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
            bool isPrimaryKey = prop.GetCustomAttribute<ColumnPrimaryKeyAttribute>() != null;

            if (isPrimaryKey)
            {
                continue;
            }

            string columnName = columnAttr?.Name ?? prop.Name;
            object value = prop.GetValue(entity) ?? DBNull.Value;

            _sets[columnName] = value;
        }

        if (_sets.Count == 0)
        {
            throw new InvalidOperationException("No columns were set for update.");
        }

        return this;
    }

    /// <summary>
    /// Adds a WHERE condition to the UPDATE query.
    /// </summary>
    public SerializedUpdateQuery<T> Where(string column, Operators @operator, object value, LogicalOperators? logicalOperator = LogicalOperators.AND)
    {
        _conditionBuilder.Add(column, @operator, value, logicalOperator);
        return this;
    }

    /// <summary>
    /// Executes the constructed UPDATE query asynchronously.
    /// </summary>
    public async Task<int> ExecuteAsync()
    {
        if (_sets.Count == 0)
        {
            throw new InvalidOperationException("No columns were set for update.");
        }

        foreach (KeyValuePair<string, object> set in _sets)
        {
            _parameters[set.Key] = set.Value;
        }

        string setClause = string.Join(", ", _sets.Keys.Select(k => $"{k} = @{k}"));
        string whereClause = _conditionBuilder.BuildCondition();

        string query = $"UPDATE {_table} SET {setClause} {(string.IsNullOrWhiteSpace(whereClause) ? "" : $"WHERE {whereClause}")};";

        Dictionary<string, object> parameters = _conditionBuilder.GetParameters();
        foreach (KeyValuePair<string, object> param in _parameters)
        {
            parameters[param.Key] = param.Value;
        }

        return await _database.ExecuteNonQueryAsync(query, parameters);
    }
}
