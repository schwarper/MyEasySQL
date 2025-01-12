using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using MyEasySQL.Attribute;

namespace MyEasySQL.Model;

/// <summary>
/// Custom exception class for handling MyEasySQL-specific errors.
/// </summary>
public partial class MyEasySQLException : Exception
{
    /// <summary>
    /// Compiled regex for validating table or column names.
    /// </summary>
    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled)] private static partial Regex NameRegex();

    /// <summary>
    /// Exception thrown when the database instance is not initialized.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DatabaseNotInitializedException"/> class.
    /// </remarks>
    public class DatabaseNotInitializedException() : InvalidOperationException("The database instance is not initialized. Ensure that a valid connection string is provided.")
    {
    }

    /// <summary>
    /// Exception thrown when an invalid name is provided.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="InvalidNameException"/> class.
    /// </remarks>
    /// <param name="name">The invalid name that caused the exception.</param>
    public class InvalidNameException(string name) : ArgumentException($"The provided name '{name}' is invalid. Names must match the required pattern.")
    {
    }

    /// <summary>
    /// Exception thrown when a member assignment is required but not found.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NotMemberAssignmentException"/> class.
    /// </remarks>
    /// <param name="binding">The member binding that caused the exception.</param>
    public class NotMemberAssignmentException(MemberBinding binding) : NotSupportedException($"Only member assignments are supported. Issue found in: {binding.Member.Name}")
    {
    }

    /// <summary>
    /// Exception thrown when a predicate expression is null.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="PredicateNullException"/> class.
    /// </remarks>
    public class PredicateNullException() : ArgumentNullException($"The predicate is null. A valid predicate is required.")
    {
    }

    /// <summary>
    /// Exception thrown when a key selector is not a member expression.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="KeySelectorNotMemberException"/> class.
    /// </remarks>
    /// <param name="expression">The expression that caused the exception.</param>
    public class KeySelectorNotMemberException(Expression expression) : InvalidOperationException($"The key selector must be a member expression. Issue found in: {expression.Type.Name}")
    {
    }

    /// <summary>
    /// Exception thrown when a value is less than or equal to zero.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="LessOrEqualToZeroException"/> class.
    /// </remarks>
    /// <param name="value">The invalid value that caused the exception.</param>
    public class LessOrEqualToZeroException(int value) : ArgumentOutOfRangeException(nameof(value), $"The value '{value}' must be greater than 0.")
    {
    }

    /// <summary>
    /// Exception thrown when the table name is empty or null.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TableNameIsEmptyException"/> class.
    /// </remarks>
    public class TableNameIsEmptyException() : InvalidOperationException($"The table name is empty or null. Please specify a valid table name.")
    {
    }

    /// <summary>
    /// Exception thrown when a field type is not supported for table creation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="FieldNameNotSupportedException"/> class.
    /// </remarks>
    /// <param name="type">The unsupported field type that caused the exception.</param>
    public class FieldNameNotSupportedException(Type type) : NotSupportedException($"The field type '{type.Name}' is not supported for table creation.")
    {
    }

    /// <summary>
    /// Exception thrown when an expression does not initialize object properties.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NotInitializedObjectPropertiesException"/> class.
    /// </remarks>
    /// <param name="expression">The expression that caused the exception.</param>
    public class NotInitializedObjectPropertiesException(Expression expression) : InvalidOperationException($"The expression '{expression}' must initialize object properties.")
    {
    }

    /// <summary>
    /// Exception thrown for unsupported expression types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NotSupportedExpressionTypeException"/> class.
    /// </remarks>
    /// <param name="name">The unsupported expression type name.</param>
    public class NotSupportedExpressionTypeException(string name) : NotSupportedException($"The expression type '{name}' is not supported.")
    {
    }

    /// <summary>
    /// Exception thrown when a required field is null.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="FieldNullException"/> class.
    /// </remarks>
    /// <param name="name">The name of the null field that caused the exception.</param>
    public class FieldNullException(string name) : InvalidOperationException($"Field '{name}' cannot be null.")
    {
    }

    /// <summary>
    /// Exception thrown when a primary key field is null.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="FieldPrimaryKeyNullException"/> class.
    /// </remarks>
    /// <param name="name">The name of the null primary key field that caused the exception.</param>
    public class FieldPrimaryKeyNullException(string name) : InvalidOperationException($"Primary key field '{name}' must be set.")
    {
    }

    /// <summary>
    /// Exception thrown when a table is not defined for a type.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TableNotDefinedException"/> class.
    /// </remarks>
    /// <param name="name">The name of the type without a defined table.</param>
    public class TableNotDefinedException(string name) : InvalidOperationException($"Table is not defined for type '{name}'.")
    {
    }

    /// <summary>
    /// Exception thrown when a table is already defined.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TableAlreadyDefinedException"/> class.
    /// </remarks>
    public class TableAlreadyDefinedException() : ArgumentException($"The table name is already defined.")
    {
    }

    /// <summary>
    /// Exception thrown for unsupported operators in expressions.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NotSupportedOperator"/> class.
    /// </remarks>
    /// <param name="type">The unsupported operator type.</param>
    public class NotSupportedOperator(ExpressionType type) : NotSupportedException($"The operator '{type}' is not supported.")
    {
    }

    /// <summary>
    /// Exception thrown when columns are empty during query generation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ColumnIsEmptyException"/> class for an empty columns dictionary.
    /// </remarks>
    public class ColumnIsEmptyException() : InvalidOperationException("The provided columns dictionary is empty. At least one column must be specified.")
    {
    }

    /// <summary>
    /// Throws an exception if the database instance is not initialized.
    /// </summary>
    public static void ThrowIfDatabaseNotInitialized(MySQL? database)
    {
        if (database is null)
            throw new DatabaseNotInitializedException();
    }

    /// <summary>
    /// Throws an exception if the provided name is invalid.
    /// </summary>
    public static void ThrowIfInvalidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || !NameRegex().IsMatch(name))
            throw new InvalidNameException(name);
    }

    /// <summary>
    /// Throws an exception if the column name is empty.
    /// </summary>
    public static void ThrowIfColumnNameIsEmpty(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidNameException(name);
    }

    /// <summary>
    /// Throws an exception if the provided binding is not a member assignment.
    /// </summary>
    public static void ThrowIfNotMemberAssignment(MemberBinding binding)
    {
        if (binding is not MemberAssignment)
            throw new NotMemberAssignmentException(binding);
    }

    /// <summary>
    /// Throws an exception if the provided columns dictionary is empty.
    /// </summary>
    public static void ThrowIfColumnIsEmpty(Dictionary<string, string> columns)
    {
        if (columns.Count == 0)
            throw new ColumnIsEmptyException();
    }

    /// <summary>
    /// Throws an exception if the provided columns array is empty.
    /// </summary>
    public static void ThrowIfColumnIsEmpty(string[] columns)
    {
        if (columns?.Length == 0)
            throw new ColumnIsEmptyException();
    }

    /// <summary>
    /// Throws an exception if the predicate expression is null.
    /// </summary>
    public static void ThrowIfPredicateNull<T>(Expression<Func<T, bool>> predicate)
    {
        if (predicate is null)
            throw new PredicateNullException();
    }

    /// <summary>
    /// Throws an exception if the key selector is not a member expression.
    /// </summary>
    public static void ThrowIfKeySelectorNotMember(Expression expression)
    {
        if (expression is not MemberExpression)
            throw new KeySelectorNotMemberException(expression);
    }

    /// <summary>
    /// Throws an exception if the value is less than or equal to zero.
    /// </summary>
    public static void ThrowIfLessOrEqualToZero(int value)
    {
        if (value <= 0)
            throw new LessOrEqualToZeroException(value);
    }

    /// <summary>
    /// Throws an exception if the table name is empty or null.
    /// </summary>
    public static void ThrowIfTableNameIsEmpty(string? table)
    {
        if (string.IsNullOrWhiteSpace(table))
            throw new TableNameIsEmptyException();
    }

    /// <summary>
    /// Throws an exception if the field type is not supported for table creation.
    /// </summary>
    public static void ThrowIfFieldNameNotSupported(Type type)
    {
        if (!Helper.SqlTypeMap.ContainsKey(type))
            throw new FieldNameNotSupportedException(type);
    }

    /// <summary>
    /// Throws an exception if the provided expression does not initialize object properties.
    /// </summary>
    public static void ThrowIfNotInitializedObjectProperties(Expression expression)
    {
        if (expression is not MemberInitExpression)
            throw new NotInitializedObjectPropertiesException(expression);
    }

    /// <summary>
    /// Throws an exception if required fields in an entity are not set correctly.
    /// </summary>
    public static void ThrowIfFieldNotCorrect<T>(T value)
    {
        foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.GetCustomAttribute<Column>() == null) continue;
            if (field.GetCustomAttribute<AutoIncrementAttribute>() != null) continue;

            if (field.GetCustomAttribute<NotNullAttribute>() != null && field.GetValue(value) == null)
                throw new FieldNullException(field.Name);

            if (field.GetCustomAttribute<PrimaryKeyAttribute>() != null && field.GetValue(value) == null)
                throw new FieldPrimaryKeyNullException(field.Name);
        }
    }

    /// <summary>
    /// Throws an exception if the table is not defined for the specified type.
    /// </summary>
    public static void ThrowIfTableIsNotDefined<T>()
    {
        if (typeof(T).GetCustomAttribute<TableNameAttribute>() == null)
            throw new TableNotDefinedException(typeof(T).Name);
    }

    /// <summary>
    /// Throws an exception if the table is already defined.
    /// </summary>
    public static void ThrowIfTableIsAlreadyDefined(string? table)
    {
        if (!string.IsNullOrWhiteSpace(table))
            throw new TableAlreadyDefinedException();
    }
}