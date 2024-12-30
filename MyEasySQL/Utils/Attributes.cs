using System;

namespace MyEasySQL.Utils;

/// <summary>
/// Specifies that the property represents the primary key of the table.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnPrimaryKeyAttribute : Attribute { }

/// <summary>
/// Specifies that the property must not allow NULL values in the table.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnNotNullAttribute : Attribute { }

/// <summary>
/// Specifies a default value for the column in the table.
/// </summary>
/// <param name="value">The default value to assign to the column.</param>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnDefaultValueAttribute(object value) : Attribute
{
    /// <summary>
    /// Gets the default value to be assigned to the column.
    /// </summary>
    public object Value { get; } = value;
}

/// <summary>
/// Specifies that the property should have a UNIQUE constraint in the table.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnUniqueAttribute : Attribute { }

/// <summary>
/// Specifies that the property should have an AUTO_INCREMENT constraint in the table.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnAutoIncrementAttribute : Attribute { }
