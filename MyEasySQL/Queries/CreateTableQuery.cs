using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyEasySQL.Utils;
using static MyEasySQL.Utils.Validator;

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
    private bool _ifNotExist = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableQuery"/> class.
    /// </summary>
    /// <param name="database">The instance of the database to execute the query on.</param>
    /// <param name="table">The name of the table to be created.</param>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="table"/> name is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="database"/> is null.</exception>
    public CreateTableQuery(MySQL database, string table)
    {
        ValidateName(table, ValidateType.Table);

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table;
    }

    /// <summary>
    /// Sets whether the table should only be created if it does not already exist. (Default is true)
    /// </summary>
    /// <param name="value">A boolean value indicating whether to include the "IF NOT EXISTS" clause. 
    /// Set to <c>true</c> to include the clause, or <c>false</c> to exclude it.</param>
    /// <returns>Returns the current <see cref="CreateTableQuery"/> instance for method chaining.</returns>
    public CreateTableQuery SetIfNotExist(bool value)
    {
        _ifNotExist = value;
        return this;
    }

    /// <summary>
    /// Adds a column definition to the table creation query.
    /// This method allows the specification of column properties such as data type, constraints, and default values.
    /// </summary>
    /// <param name="name">The name of the column to be added.</param>
    /// <param name="type">The data type of the column (e.g., INT, VARCHAR).</param>
    /// <param name="typeValue">Optional size or precision for the data type (e.g., length for VARCHAR).</param>
    /// <param name="defaultValue">Specifies a default value for the column.</param>
    /// <param name="flag">Optional flags for column</param>
    /// <returns>The current <see cref="CreateTableQuery"/> instance, allowing for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if the column name or type value is invalid.</exception>
    public CreateTableQuery AddColumn(string name, DataTypes type, string? typeValue = null, string? defaultValue = null, ColumnFlags flag = ColumnFlags.None)
    {
        ValidateName(_table, ValidateType.Column);
        ValidateTypeValue(type, typeValue);

        StringBuilder builder = new();
        builder.Append($"{name} {type}");

        if (typeValue != null)
        {
            builder.Append($"({typeValue})");
        }

        if (flag.HasFlag(ColumnFlags.NotNull))
        {
            builder.Append(" NOT NULL");
        }

        if (flag.HasFlag(ColumnFlags.PrimaryKey))
        {
            builder.Append(" PRIMARY KEY");
        }

        if (flag.HasFlag(ColumnFlags.AutoIncrement))
        {
            builder.Append(" AUTO_INCREMENT");
        }

        if (flag.HasFlag(ColumnFlags.Unique))
        {
            builder.Append(" UNIQUE");
        }

        if (defaultValue != null)
        {
            builder.Append($" DEFAULT '{defaultValue.Replace("'", "''")}'");
        }

        _columns.Add(builder.ToString());

        return this;
    }

    /// <summary>
    /// Executes the table creation query asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. 
    /// The task result contains the number of rows affected by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if no columns have been defined for the table.</exception>
    public async Task<int> ExecuteAsync()
    {
        if (_columns.Count == 0)
        {
            throw new InvalidOperationException("No columns defined for table creation.");
        }

        string query = $"CREATE TABLE {(_ifNotExist ? "IF NOT EXISTS " : "")}{_table} ({string.Join(", ", _columns)});";
        return await _database.ExecuteNonQueryAsync(query);
    }
}