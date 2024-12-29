namespace MyEasySQL.Utils;

/// <summary>
/// Represents the various data types used in a MySQL database.
/// </summary>
public enum DataTypes
{
    /// <summary>
    /// Represents a small integer data type.
    /// </summary>
    TINYINT,

    /// <summary>
    /// Represents a small integer data type with a larger range.
    /// </summary>
    SMALLINT,

    /// <summary>
    /// Represents a normal integer data type.
    /// </summary>
    INT,

    /// <summary>
    /// Represents a large integer data type.
    /// </summary>
    BIGINT,

    /// <summary>
    /// Represents a fixed-length character string data type.
    /// </summary>
    CHAR,

    /// <summary>
    /// Represents a variable-length character string data type.
    /// </summary>
    VARCHAR,

    /// <summary>
    /// Represents a large variable-length text data type.
    /// </summary>
    TEXT,

    /// <summary>
    /// Represents a small string data type (up to 255 characters).
    /// </summary>
    TINYTEXT,

    /// <summary>
    /// Represents a medium-length string data type (up to 16,777,215 characters).
    /// </summary>
    MEDIUMTEXT,

    /// <summary>
    /// Represents a large string data type (up to 4GB).
    /// </summary>
    LONGTEXT,

    /// <summary>
    /// Represents a fixed-length binary data type.
    /// </summary>
    BINARY,

    /// <summary>
    /// Represents a variable-length binary data type.
    /// </summary>
    VARBINARY,

    /// <summary>
    /// Represents a boolean data type.
    /// </summary>
    BOOLEAN,

    /// <summary>
    /// Represents a floating-point number data type.
    /// </summary>
    FLOAT,

    /// <summary>
    /// Represents a double-precision floating-point number data type.
    /// </summary>
    DOUBLE,

    /// <summary>
    /// Represents a fixed-point number data type with a specified scale and precision.
    /// </summary>
    DECIMAL,

    /// <summary>
    /// Represents a date value.
    /// </summary>
    DATE,

    /// <summary>
    /// Represents a date and time value (without time zone).
    /// </summary>
    DATETIME,

    /// <summary>
    /// Represents a time value.
    /// </summary>
    TIME,

    /// <summary>
    /// Represents a timestamp value.
    /// </summary>
    TIMESTAMP,

    /// <summary>
    /// Represents a year value in a four-digit format.
    /// </summary>
    YEAR,

    /// <summary>
    /// Represents a binary large object data type (used for storing binary data).
    /// </summary>
    BLOB,

    /// <summary>
    /// Represents a tiny binary large object data type.
    /// </summary>
    TINYBLOB,

    /// <summary>
    /// Represents a medium binary large object data type.
    /// </summary>
    MEDIUMBLOB,

    /// <summary>
    /// Represents a large binary large object data type.
    /// </summary>
    LONGBLOB,

    /// <summary>
    /// Represents a spatial data type used for geographical data.
    /// </summary>
    GEOMETRY,

    /// <summary>
    /// Represents a point data type used for spatial data.
    /// </summary>
    POINT,

    /// <summary>
    /// Represents a line string data type used for spatial data.
    /// </summary>
    LINESTRING,

    /// <summary>
    /// Represents a polygon data type used for spatial data.
    /// </summary>
    POLYGON,

    /// <summary>
    /// Represents a geometry collection data type used for spatial data.
    /// </summary>
    GEOMETRYCOLLECTION,

    /// <summary>
    /// Represents a multi-point data type used for spatial data.
    /// </summary>
    MULTIPOINT,

    /// <summary>
    /// Represents a multi-line string data type used for spatial data.
    /// </summary>
    MULTILINESTRING,

    /// <summary>
    /// Represents a multi-polygon data type used for spatial data.
    /// </summary>
    MULTIPOLYGON
}