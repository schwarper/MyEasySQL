
# MyEasySQL
**MyEasySQL** is a lightweight C# library designed to streamline interactions with MySQL databases. It leverages Dapper for efficient data access and provides an intuitive API for performing common operations such as querying, inserting, updating and deleting records. MyEasySQL supports both synchronous and asynchronous operations, providing flexible connection management and optimised query construction for small to large applications.

# Features
- **CRUD Operations**: Simplified methods for inserting, updating, selecting, and deleting records with optional conditions.
- **Database & Table Management**: Create and drop databases and tables with a fluent API.
- **Dapper Integration**: Built on Dapper for efficient data access and object mapping.
- **Asynchronous Operations**: Perform database operations asynchronously for improved performance.

# Example

```csharp
using MyEasySQL;
using MyEasySQL.Utils;

var sql = new MySQL("host", "name", "user", "password");

// 1. Create a Database
await sql.CreateDatabase("MyDatabase");

// 2. Create a Table
await sql.CreateTable("MyTable")
    .AddColumn("ID", DataTypes.INT, autoIncrement: true, primaryKey: true)  // Adding a primary key with auto-increment
    .AddColumn("Name", DataTypes.VARCHAR, typeValue: "255", notNull: true)   // Adding a non-nullable VARCHAR column
    .AddColumn("Age", DataTypes.INT)                                       // Adding an integer column for age
    .ExecuteAsync();  // Execute the table creation

// 3. Insert Data into the Table
await sql.Insert("MyTable")
    .AddColumn("Name", "John Doe")  // Inserting the name 'John Doe'
    .AddColumn("Age", 30)           // Inserting the age 30
    .ExecuteAsync();

await sql.Insert("MyTable")
    .AddColumn("Name", "Jane Smith")  // Inserting the name 'Jane Smith'
    .AddColumn("Age", 25)             // Inserting the age 25
    .ExecuteAsync();

// 4. Update Data in the Table
await sql.Update("MyTable")
    .Set("Age", 31)  // Updating the age to 31
    .Where("Name", Operators.EQUAL, "John Doe")  // Where the name is 'John Doe'
    .ExecuteAsync();

// 5. Select Data from the Table (SELECT)
var result = await sql.Select("Name", "Age")  // Selecting 'Name' and 'Age' columns
    .From("MyTable")  // From the 'MyTable' table
    .Where("Age", Operators.GREATER_THAN, 20)  // Where age is greater than 20
    .OrderBy("Age", OrderType.ASC)  // Ordering by age in ascending order
    .Limit(5)  // Limiting the results to 5
    .ReadAsync<dynamic>();  // Reading the results as dynamic objects

// Iterate through the result and print it
foreach (var row in result)
{
    Console.WriteLine($"Name: {row.Name}, Age: {row.Age}");
}

// 6. Delete Data from the Table
await sql.Delete()
    .From("MyTable")  // From the 'MyTable' table
    .Where("Age", Operators.LESS_THAN, 30)  // Where age is less than 30
    .ExecuteAsync();

// 7. Drop the Table
await sql.DropTable("MyTable");  // Dropping (deleting) the 'MyTable' table

// 8. Drop the Database
await sql.DropDatabase("MyDatabase");  // Dropping (deleting) the 'MyDatabase' database

```
