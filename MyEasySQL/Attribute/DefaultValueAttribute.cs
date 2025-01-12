using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEasySQL.Attribute;

/// <summary>
/// Attribute to specify a default value for a database column.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultValueAttribute"/> class.
/// </remarks>
/// <param name="value">The default value to set for the column.</param>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class DefaultValueAttribute(object value) : ColumnAttribute
{
    /// <summary>
    /// The default value to assign to the column.
    /// </summary>
    public object Value { get; } = value;
}