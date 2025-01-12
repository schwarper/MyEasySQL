using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyEasySQL.Attribute;

/// <summary>
/// Attribute to mark a field or property as auto-incrementing in the database.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class AutoIncrementAttribute : ColumnAttribute { }