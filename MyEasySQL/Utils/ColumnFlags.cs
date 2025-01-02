using System;
using System.Collections.Generic;

namespace MyEasySQL.Utils;

/// <summary>
/// Defines flags for column attributes in a database table.
/// </summary>
[Flags]
public enum ColumnFlags
{
    /// <summary>
    /// Specifies nothing.
    /// </summary>
    None = 0,

    /// <summary>
    /// Specifies that the column must have a value and cannot be null.
    /// </summary>
    NotNull = 1 << 0,

    /// <summary>
    /// Specifies that the column value is automatically incremented for each new row.
    /// </summary>
    AutoIncrement = 1 << 1,

    /// <summary>
    /// Specifies that the column must contain unique values.
    /// </summary>
    Unique = 1 << 2,

    /// <summary>
    /// Specifies that the column is a primary key.
    /// </summary>
    PrimaryKey = 1 << 3
}