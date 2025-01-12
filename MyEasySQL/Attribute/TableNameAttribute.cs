using System;
using System.ComponentModel.DataAnnotations.Schema;
using MyEasySQL.Model;

namespace MyEasySQL.Attribute;

/// <summary>
/// Attribute to specify the table name for a database entity.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TableNameAttribute : ColumnAttribute
{
    /// <summary>
    /// The name of the table in the database.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableNameAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <exception cref="MyEasySQLException">Thrown if the table name is empty or null.</exception>
    public TableNameAttribute(string name)
    {
        MyEasySQLException.ThrowIfTableNameIsEmpty(name);
        Value = name;
    }
}