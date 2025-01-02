using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MyEasySQL.Queries;
using MyEasySQL.Utils;
using static MyEasySQL.Utils.Validator;

namespace MyEasySQL.SerializedQueries;

/// <summary>
/// A class to generate and execute a SQL CREATE TABLE query based on a given class definition.
/// The class dynamically generates the SQL schema based on the properties of the provided class type.
/// </summary>
/// <typeparam name="T">The class type representing the table structure.</typeparam>
public class CreateTableSerializedQuery<T> where T : class, new()
{
    private readonly MySQL _database;
    private readonly string _table;
    private readonly List<string> _columns = [];
    private bool _ifNotExist = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableSerializedQuery{T}"/> class.
    /// </summary>
    /// <param name="database">The database instance to execute the query on.</param>
    /// <param name="table">The name of the table to be created.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="table"/> name is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="database"/> is null.</exception>
    public CreateTableSerializedQuery(MySQL database, string table)
    {
        ValidateName(table, ValidateType.Table);

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table;

        GenerateColumnsFromType();
    }

    /// <summary>
    /// Sets whether the table should only be created if it does not already exist. (Default is true)
    /// </summary>
    /// <param name="value">A boolean value indicating whether to include the "IF NOT EXISTS" clause. 
    /// Set to <c>true</c> to include the clause, or <c>false</c> to exclude it.</param>
    /// <returns>Returns the current <see cref="CreateTableSerializedQuery{T}"/> instance for method chaining.</returns>
    public CreateTableSerializedQuery<T> SetIfNotExist(bool value)
    {
        _ifNotExist = value;
        return this;
    }

    /// <summary>
    /// Executes the generated CREATE TABLE query asynchronously.
    /// After the table structure has been dynamically created, this method sends the query to the database to create the table.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the number of rows affected by the query.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if no columns are defined for the table.</exception>
    public async Task<int> ExecuteAsync()
    {
        if (_columns.Count == 0)
        {
            throw new InvalidOperationException("No columns defined for table creation.");
        }

        string query = $"CREATE TABLE {(_ifNotExist ? "IF NOT EXISTS " : "")}{_table} ({string.Join(", ", _columns)});";
        return await _database.ExecuteNonQueryAsync(query);
    }

    private void GenerateColumnsFromType()
    {
        FieldInfo[] properties = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo prop in properties)
        {
            string columnName = prop.Name;
            string columnType = GetSqlType(prop.FieldType);

            StringBuilder builder = new();
            builder.Append($"{columnName} {columnType}");

            ColumnDefaultValueAttribute? defaultAttr = prop.GetCustomAttribute<ColumnDefaultValueAttribute>();
            if (defaultAttr?.Value != null)
            {
                builder.Append($" DEFAULT '{defaultAttr.Value.ToString()?.Replace("'", "''")}'");
            }

            if (prop.GetCustomAttribute<ColumnPrimaryKeyAttribute>() != null)
            {
                builder.Append(" PRIMARY KEY");
            }

            if (prop.GetCustomAttribute<ColumnNotNullAttribute>() != null)
            {
                builder.Append(" NOT NULL");
            }

            if (prop.GetCustomAttribute<ColumnUniqueAttribute>() != null)
            {
                builder.Append(" UNIQUE");
            }

            if (prop.GetCustomAttribute<ColumnAutoIncrementAttribute>() != null)
            {
                builder.Append(" AUTO_INCREMENT");
            }

            _columns.Add(builder.ToString());
        }
    }

    private static string GetSqlType(Type type)
    {
        return type switch
        {
            _ when type == typeof(int) => "INT",
            _ when type == typeof(string) => "VARCHAR(255)",
            _ when type == typeof(bool) => "BOOLEAN",
            _ when type == typeof(DateTime) => "DATETIME",
            _ when type == typeof(decimal) => "DECIMAL(18,2)",
            _ when type == typeof(float) => "FLOAT",
            _ when type == typeof(double) => "DOUBLE",
            _ when type == typeof(long) => "BIGINT",
            _ when type == typeof(short) => "SMALLINT",
            _ when type == typeof(byte) => "TINYINT",
            _ when type == typeof(char) => "CHAR(1)",
            _ when type == typeof(Guid) => "CHAR(36)",
            _ when type == typeof(byte[]) => "BLOB",
            _ when type == typeof(TimeSpan) => "TIME",
            _ when type == typeof(DateOnly) => "DATE",
            _ when type == typeof(TimeOnly) => "TIME",
            _ => throw new NotSupportedException($"Type '{type.Name}' is not supported for table creation.")
        };
    }
}
