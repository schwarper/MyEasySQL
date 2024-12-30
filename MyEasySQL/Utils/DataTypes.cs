namespace MyEasySQL.Utils;

/// <summary>
/// Represents the various data types used in a MySQL database.
/// </summary>
public enum DataTypes
{
    /// <summary>
    /// Represents a small integer data type, typically used for storing small numerical values.
    /// </summary>
    TINYINT,

    /// <summary>
    /// Represents a small integer data type with a larger range than TINYINT.
    /// </summary>
    SMALLINT,

    /// <summary>
    /// Represents a standard integer data type, used for storing typical integer values.
    /// </summary>
    INT,

    /// <summary>
    /// Represents a large integer data type, suitable for storing large numerical values.
    /// </summary>
    BIGINT,

    /// <summary>
    /// Represents a fixed-length character string data type, typically used for short, fixed-size text fields.
    /// </summary>
    CHAR,

    /// <summary>
    /// Represents a variable-length character string data type, commonly used for strings that vary in length.
    /// </summary>
    VARCHAR,

    /// <summary>
    /// Represents a large variable-length text data type, used for storing large blocks of text.
    /// </summary>
    TEXT,

    /// <summary>
    /// Represents a small string data type, allowing up to 255 characters.
    /// </summary>
    TINYTEXT,

    /// <summary>
    /// Represents a medium-length string data type, allowing up to 16,777,215 characters.
    /// </summary>
    MEDIUMTEXT,

    /// <summary>
    /// Represents a large string data type, allowing up to 4GB of text.
    /// </summary>
    LONGTEXT,

    /// <summary>
    /// Represents a fixed-length binary data type, used for storing binary data.
    /// </summary>
    BINARY,

    /// <summary>
    /// Represents a variable-length binary data type, suitable for storing variable-length binary data.
    /// </summary>
    VARBINARY,

    /// <summary>
    /// Represents a boolean data type, used for storing true/false values.
    /// </summary>
    BOOLEAN,

    /// <summary>
    /// Represents a floating-point number data type, used for storing decimal numbers with single precision.
    /// </summary>
    FLOAT,

    /// <summary>
    /// Represents a double-precision floating-point number data type, used for storing decimal numbers with double precision.
    /// </summary>
    DOUBLE,

    /// <summary>
    /// Represents a fixed-point number data type, with a specified scale and precision, used for exact numeric values.
    /// </summary>
    DECIMAL,

    /// <summary>
    /// Represents a date value, used for storing calendar dates.
    /// </summary>
    DATE,

    /// <summary>
    /// Represents a date and time value (without time zone), used for storing timestamp-like values.
    /// </summary>
    DATETIME,

    /// <summary>
    /// Represents a time value, used for storing time-of-day information.
    /// </summary>
    TIME,

    /// <summary>
    /// Represents a timestamp value, used for storing date and time information with automatic updating.
    /// </summary>
    TIMESTAMP,

    /// <summary>
    /// Represents a year value, stored in a four-digit format (e.g., 2024).
    /// </summary>
    YEAR,

    /// <summary>
    /// Represents a binary large object data type (BLOB), used for storing binary data such as images or files.
    /// </summary>
    BLOB,

    /// <summary>
    /// Represents a tiny binary large object data type (TINYBLOB), used for storing small binary data.
    /// </summary>
    TINYBLOB,

    /// <summary>
    /// Represents a medium binary large object data type (MEDIUMBLOB), used for storing medium-sized binary data.
    /// </summary>
    MEDIUMBLOB,

    /// <summary>
    /// Represents a large binary large object data type (LONGBLOB), used for storing large binary data.
    /// </summary>
    LONGBLOB,

    /// <summary>
    /// Represents a spatial data type, used for storing geographical or spatial data.
    /// </summary>
    GEOMETRY,

    /// <summary>
    /// Represents a point data type, used for spatial data to store a single point.
    /// </summary>
    POINT,

    /// <summary>
    /// Represents a line string data type, used for storing a series of connected points in spatial data.
    /// </summary>
    LINESTRING,

    /// <summary>
    /// Represents a polygon data type, used for storing polygons in spatial data.
    /// </summary>
    POLYGON,

    /// <summary>
    /// Represents a geometry collection data type, used for storing multiple geometrical objects in spatial data.
    /// </summary>
    GEOMETRYCOLLECTION,

    /// <summary>
    /// Represents a multi-point data type, used for storing multiple points in spatial data.
    /// </summary>
    MULTIPOINT,

    /// <summary>
    /// Represents a multi-line string data type, used for storing multiple line strings in spatial data.
    /// </summary>
    MULTILINESTRING,

    /// <summary>
    /// Represents a multi-polygon data type, used for storing multiple polygons in spatial data.
    /// </summary>
    MULTIPOLYGON
}
