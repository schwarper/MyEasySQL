using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEasySQL.Attribute;

/// <summary>
/// Attribute to designate a field or property as a database column.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class Column : ColumnAttribute { }