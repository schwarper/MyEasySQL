using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MyEasySQL.Attribute;

namespace MyEasySQL.Model;

internal static class Helper
{
    internal readonly static FrozenDictionary<Type, string> SqlTypeMap = new Dictionary<Type, string>()
    {
        { typeof(int), "INT" },
        { typeof(string), "VARCHAR(255)" },
        { typeof(bool), "BOOLEAN" },
        { typeof(DateTime), "DATETIME" },
        { typeof(decimal), "DECIMAL(18,2)" },
        { typeof(float), "FLOAT" },
        { typeof(double), "DOUBLE" },
        { typeof(long), "BIGINT" },
        { typeof(short), "SMALLINT" },
        { typeof(byte), "TINYINT" },
        { typeof(char), "CHAR(1)" },
        { typeof(Guid), "CHAR(36)" },
        { typeof(byte[]), "BLOB" },
        { typeof(TimeSpan), "TIME" },
        { typeof(DateOnly), "DATE" },
        { typeof(TimeOnly), "TIME" }
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ExpressionType, string> WhereOperators = new Dictionary<ExpressionType, string>()
    {
        { ExpressionType.Equal, "=" },
        { ExpressionType.NotEqual, "!=" },
        { ExpressionType.GreaterThan, ">" },
        { ExpressionType.GreaterThanOrEqual, ">=" },
        { ExpressionType.LessThan, "<" },
        { ExpressionType.LessThanOrEqual, "<=" },
        { ExpressionType.AndAlso, "AND" },
        { ExpressionType.OrElse, "OR" }
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ExpressionType, string> InsertOperators = new Dictionary<ExpressionType, string>()
    {
        { ExpressionType.Add, "+" },
        { ExpressionType.Subtract, "-" },
        { ExpressionType.Multiply, "*" },
        { ExpressionType.Divide, "/" },
        { ExpressionType.Modulo, "%" }
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<ExpressionType, string> UpdateOperators = new Dictionary<ExpressionType, string>()
    {
        { ExpressionType.Add, "+" },
        { ExpressionType.Subtract, "-" },
        { ExpressionType.Multiply, "*" },
        { ExpressionType.Divide, "/" },
        { ExpressionType.Modulo, "%" },
        { ExpressionType.Equal, "=" }
    }.ToFrozenDictionary();

    internal static List<string> GenerateColumnsFromType<T>()
    {
        List<string> _columns = [];

        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.GetCustomAttribute<Column>() == null)
                continue;

            MyEasySQLException.ThrowIfFieldNameNotSupported(field.FieldType);

            StringBuilder builder = new();
            builder.Append($"`{field.Name}` {SqlTypeMap[field.FieldType]}");

            if (field.GetCustomAttribute<NotNullAttribute>() != null) builder.Append(" NOT NULL");
            if (field.GetCustomAttribute<AutoIncrementAttribute>() != null) builder.Append(" AUTO_INCREMENT");
            if (field.GetCustomAttribute<PrimaryKeyAttribute>() != null) builder.Append(" PRIMARY KEY");
            if (field.GetCustomAttribute<UniqueAttribute>() != null) builder.Append(" UNIQUE");

            string? defaultValue = field.GetCustomAttribute<DefaultValueAttribute>()?.Value?.ToString();
            if (!string.IsNullOrEmpty(defaultValue)) builder.Append($" DEFAULT '{defaultValue.Replace("'", "''")}'");

            _columns.Add(builder.ToString());
        }

        return _columns;
    }

    internal static Dictionary<string, object> GetValuesFromEntity<T>(T entity)
    {
        Dictionary<string, object> values = [];

        foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.GetCustomAttribute<AutoIncrementAttribute>() != null)
                continue;

            if (field.GetCustomAttribute<Column>() == null)
                continue;

            MyEasySQLException.ThrowIfFieldNotCorrect(field.FieldType);

            object value = field.GetValue(entity) ?? FormatValue(field.GetValue(entity)).ToString();
            values[field.Name] = value;
        }

        return values;
    }

    internal static string GetOperator(ExpressionType type, FrozenDictionary<ExpressionType, string> operators)
        => operators.TryGetValue(type, out string? op) ? op : throw new MyEasySQLException.NotSupportedOperator(type);
    internal static string ParseExpressionWhere(Expression expression)
        => ParseExpressionWithOperators(expression, WhereOperators);
    internal static string ParseExpressionInsert(Expression expression)
        => ParseExpressionWithOperators(expression, InsertOperators);
    internal static string ParseExpressionUpdate(Expression expression)
        => ParseExpressionWithOperators(expression, UpdateOperators);
    internal static string ParseExpressionWithOperators(Expression expression, FrozenDictionary<ExpressionType, string> operators)
        => ParseExpression(expression, type => GetOperator(type, operators));

    internal static string ParseExpression(Expression expression, Func<ExpressionType, string> getOperator)
    {
        return expression switch
        {
            ConstantExpression constant => FormatValue(constant.Value),
            MemberExpression member => $"`{member.Member.Name}`",
            BinaryExpression binary => $"{ParseExpression(binary.Left, getOperator)} {getOperator(binary.NodeType)} {ParseExpression(binary.Right, getOperator)}",
            UnaryExpression unary => $"{(unary.NodeType == ExpressionType.Not ? "NOT " : "")}{ParseExpression(unary.Operand, getOperator)}",
            _ => throw new MyEasySQLException.NotSupportedExpressionTypeException(expression.GetType().Name)
        };
    }

    internal static string FormatValue(object? value)
    {
        return value switch
        {
            string str => $"'{str.Replace("'", "''")}'",
            DateTime dateTime => $"'{dateTime:yyyy-MM-dd HH:mm:ss}'",
            bool boolean => boolean ? "1" : "0",
            _ => value?.ToString() ?? "NULL"
        };
    }
}