using MyEasySQL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MyEasySQL.Utils.RegexUtil;

namespace MyEasySQL.Queries;

/// <summary>
/// Provides functionality to construct and execute an SQL UPDATE query.
/// </summary>
public class UpdateQuery
{
    private readonly MySQL _database;
    private readonly string _table;
    private readonly Dictionary<string, object> _sets = [];
    private readonly Dictionary<string, object> _parameters = [];
    private readonly ConditionBuilder _conditionBuilder = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateQuery"/> class.
    /// </summary>
    /// <param name="database">The database instance to execute the query on.</param>
    /// <param name="table">The name of the table to update.</param>
    /// <exception cref="ArgumentNullException">Thrown when the database instance is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the table name is invalid.</exception>
    public UpdateQuery(MySQL database, string table)
    {
        Validate(table, ValidateType.Table);

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table;
    }

    /// <summary>
    /// Specifies a column and value to set in the UPDATE query.
    /// </summary>
    /// <param name="column">The name of the column to update.</param>
    /// <param name="value">The value to set the column to.</param>
    /// <returns>The <see cref="UpdateQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the column name is invalid.</exception>
    public UpdateQuery Set(string column, object value)
    {
        Validate(column, ValidateType.Column);

        _sets[column] = value ?? DBNull.Value;
        return this;
    }

    /// <summary>
    /// Adds a WHERE condition to the UPDATE query.
    /// </summary>
    /// <param name="column">The column to filter on.</param>
    /// <param name="operator">The operator to use in the condition (e.g., EQUAL, NOT_EQUAL).</param>
    /// <param name="value">The value to compare the column against.</param>
    /// <param name="logicalOperator">The logical operator to chain the condition with (default is AND).</param>
    /// <returns>The <see cref="UpdateQuery"/> instance for method chaining.</returns>
    public UpdateQuery Where(string column, Operators @operator, object value, LogicalOperators? logicalOperator = LogicalOperators.AND)
    {
        _conditionBuilder.Add(column, @operator, value, logicalOperator);
        return this;
    }

    /// <summary>
    /// Executes the constructed UPDATE query asynchronously.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the number of rows affected by the command.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when no columns are set for update.</exception>
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
