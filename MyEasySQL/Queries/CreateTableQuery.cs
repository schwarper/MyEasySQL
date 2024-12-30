using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyEasySQL.Utils;
using static MyEasySQL.Utils.RegexUtil;

namespace MyEasySQL.Queries;

/// <summary>
/// Represents a query for creating a table in the database with specified columns and constraints.
/// Provides methods for adding columns and constraints, and executing the creation query.
/// </summary>
public class CreateTableQuery
{
    private readonly MySQL _database;
    private readonly string _table;
    private readonly List<string> _columns = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableQuery"/> class.
    /// </summary>
    /// <param name="database">The instance of the database to execute the query on.</param>
    /// <param name="table">The name of the table to be created.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="database"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="table"/> name is invalid.</exception>
    public CreateTableQuery(MySQL database, string table)
    {
        Validate(table, ValidateType.Table);

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table;
    }

    /// <summary>
    /// Adds a column definition to the table creation query.
    /// This method allows the specification of column properties such as data type, constraints, and default values.
    /// </summary>
    /// <param name="name">The name of the column to be added.</param>
    /// <param name="type">The data type of the column (e.g., INT, VARCHAR).</param>
    /// <param name="typeValue">Optional size or precision for the data type (e.g., length for VARCHAR).</param>
    /// <param name="notNull">Indicates whether the column should be defined as NOT NULL.</param>
    /// <param name="autoIncrement">Indicates whether the column should be set as AUTO_INCREMENT.</param>
    /// <param name="unique">Indicates whether the column should be unique.</param>
    /// <param name="primaryKey">Indicates whether the column is a primary key.</param>
    /// <param name="defaultValue">Specifies a default value for the column.</param>
    /// <returns>The current <see cref="CreateTableQuery"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the column name or type value is invalid.</exception>
    public CreateTableQuery AddColumn(
        string name,
        DataTypes type,
        string? typeValue = null,
        bool notNull = false,
        bool autoIncrement = false,
        bool unique = false,
        bool primaryKey = false,
        string? defaultValue = null)
    {
        Validate(name, ValidateType.Column);

        if (typeValue != null)
        {
            Validate(typeValue, ValidateType.TypeValue);
        }

        StringBuilder columnDefBuilder = new();
        columnDefBuilder.Append($"{name} {type}");

        if (!string.IsNullOrEmpty(typeValue))
            columnDefBuilder.Append($"({typeValue})");

        if (notNull)
            columnDefBuilder.Append(" NOT NULL");

        if (primaryKey)
            columnDefBuilder.Append(" PRIMARY KEY");

        if (autoIncrement)
            columnDefBuilder.Append(" AUTO_INCREMENT");

        if (unique)
            columnDefBuilder.Append(" UNIQUE");

        if (defaultValue != null)
            columnDefBuilder.Append($" DEFAULT '{defaultValue.Replace("'", "''")}'");

        _columns.Add(columnDefBuilder.ToString());

        return this;
    }

    /// <summary>
    /// Executes the table creation query asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. 
    /// The task result contains the number of rows affected by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when no columns have been defined for the table.</exception>
    public async Task<int> ExecuteAsync()
    {
        if (_columns.Count == 0)
        {
            throw new InvalidOperationException("No columns defined for table creation.");
        }

        string query = $"CREATE TABLE IF NOT EXISTS {_table} ({string.Join(", ", _columns)});";
        return await _database.ExecuteNonQueryAsync(query);
    }
}
