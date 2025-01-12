using System;
using System.Threading.Tasks;
using MyEasySQL;
using MyEasySQL.Attribute;
using MyEasySQL.Model;
using MyEasySQL.SerializedQuery;

namespace MyEasySQLTest;

class Program
{
    #region Table Class
    // This is not necessary. You can use another option if you want.
    // This allows you not to use table name for all queries.
    [TableName("MyTableSerialized")]
    public class MyTableSerialized
    {
        [Column, PrimaryKey, AutoIncrement] public int UniqueId;
        [Column, NotNull] public string? Account;
        [Column, NotNull] public string? Password;
        [Column, NotNull] public int Age;
        [Column, NotNull, Unique] public string? Email;
        [Column, NotNull, DefaultValue(0)] public bool Verified;
        [Column, NotNull, DefaultValue("active")] public string? Status;
    }
    #endregion
    public static async Task Main()
    {
        #region Test SQL Query Strings
        CreateTableTest();
        InsertTest();
        UpdateTest();
        SelectTest();
        DeleteTest();
        #endregion

        #region Test SQL Query ASYNC
        await CreateTableAsyncTest();
        await InsertAsyncTest();
        await UpdateAsyncTest();
        await SelectAsyncTest();
        await DeleteAsyncTest();
        #endregion
    }

    #region Colorful Console Message
    private static void SendConsoleMessage(string query, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(query);
        Console.ResetColor();
    }
    #endregion

    #region Test SQL Query Strings
    public static void CreateTableTest()
    {
        // SetTableFromEntity will get table name from [TableName(...)] in class.
        // SetIfNotExist is not necessary, it is default true.
        string query = new CreateTableSerializedQuery<MyTableSerialized>()
            .SetTableFromEntity()
            .SetIfNotExist(true)
            .ToString();

        // We can use ("tablename") if we want to use another table.
        // If we set SetIfNotExist false, it won't be added.
        string query2 = new CreateTableSerializedQuery<MyTableSerialized>("MyTableSerializedOther")
            .SetIfNotExist(false)
            .ToString();

        SendConsoleMessage(query, ConsoleColor.Green);
        SendConsoleMessage(query2, ConsoleColor.DarkGreen);
    }
    public static void InsertTest()
    {
        // We create a row to insert.
        MyTableSerialized row = new()
        {
            Account = "schwarper",
            Password = "1234567890",
            Age = 0,
            Email = "schwarper@schwarper.com",
            Verified = false,
            Status = "active"
        };

        // As we do in creating table, we use MyTableSerialized class table name.
        string query = new InsertSerializedQuery<MyTableSerialized>(row)
            .SetTableFromEntity()
            .ToString();

        // As we do in creating table, we use ("tablename") for another table name.
        // If we want to add OnDuplicateKeyUpdate, we use key => new Class
        // Age = key.Age + 1 => Age = Age + 1
        // Password = "123456789"
        // Allowed expression types: +-/*%
        string query2 = new InsertSerializedQuery<MyTableSerialized>("MyTableSerializedOther", row)
            .OnDuplicateKeyUpdate(key => new MyTableSerialized
            {
                Age = key.Age + 1,
                Password = "123456789"
            })
            .ToString();

        SendConsoleMessage(query, ConsoleColor.Blue);
        SendConsoleMessage(query2, ConsoleColor.DarkBlue);
    }
    public static void UpdateTest()
    {
        // Set does the same thing like OnDuplicateKeyUpdate.
        // Age = Age + 1
        // Where sets a condition.
        // WHERE Age < 5
        string query = new UpdateSerializedQuery<MyTableSerialized>()
            .SetTableFromEntity()
            .Set(key => new MyTableSerialized
            {
                Age = key.Age + 1
            })
            .Where(key => key.Age < 5)
            .ToString();

        // !key.Verified does `Verified` = NOT `Verified`
        // If verified is true, makes false and opposite.
        // Where conditions can be concatted with || (OR) and && (AND). 
        string query2 = new UpdateSerializedQuery<MyTableSerialized>("MyTableSerializedOther")
            .Set(key => new MyTableSerialized
            {
                Status = "not active",
                Verified = !key.Verified
            })
            .Where(key => key.Verified == false || key.Password != "1" && key.Verified == true)
            .ToString();

        SendConsoleMessage(query, ConsoleColor.Cyan);
        SendConsoleMessage(query2, ConsoleColor.DarkCyan);
    }
    public static void SelectTest()
    {
        // You must set Select.
        // FromEntityTable does the same thing like SetTableFromEntity does.
        // Where (not necessary)
        // OrderBy => ORDER BY `Age` ASC (not necessary)
        // Limit => LIMIT 5 (not necessary)
        string query = new SelectSerializedQuery<MyTableSerialized>()
            .Select("Age")
            .FromEntityTable()
            .Where(key => key.Age < 100)
            .OrderBy(key => key.Age, OrderType.ASC)
            .Limit(5)
            .ToString();

        // * shows everything in table with 
        string query2 = new SelectSerializedQuery<MyTableSerialized>()
            .Select("*")
            .From("MyTableSerializedOther")
            .ToString();

        SendConsoleMessage(query, ConsoleColor.Red);
        SendConsoleMessage(query2, ConsoleColor.DarkRed);
    }
    public static void DeleteTest()
    {
        string query = new DeleteSerializedQuery<MyTableSerialized>()
            .FromEntityTable()
            .Where(key => key.Age < 100)
            .ToString();

        string query2 = new DeleteSerializedQuery<MyTableSerialized>()
            .From("MyTableSerializedOther")
            .ToString();

        SendConsoleMessage(query, ConsoleColor.Magenta);
        SendConsoleMessage(query2, ConsoleColor.DarkMagenta);
    }
    #endregion

    #region Test SQL Query ASYNC
    public static readonly MySQL _sql = new("host", "name", "user", "password");

    public static async Task CreateTableAsyncTest()
    {
        int query = await _sql.CreateTableSerialized<MyTableSerialized>()
            .SetTableFromEntity()
            .SetIfNotExist(true)
            .ExecuteAsync();

        int query2 = await _sql.CreateTableSerialized<MyTableSerialized>("MyTableSerializedOther")
            .SetIfNotExist(true)
            .ExecuteAsync();

        SendConsoleMessage($"Create table: {query} row(s)", ConsoleColor.Green);
        SendConsoleMessage($"Create table: {query2} row(s)", ConsoleColor.DarkGreen);
    }
    public static async Task InsertAsyncTest()
    {
        MyTableSerialized row = new()
        {
            Account = "schwarper",
            Password = "1234567890",
            Age = 0,
            Email = "schwarper@schwarper.com",
            Verified = false,
            Status = "active"
        };

        int query = await _sql.InsertSerialized<MyTableSerialized>(row)
            .SetTableFromEntity()
            .OnDuplicateKeyUpdate(key => new MyTableSerialized
            {
                Age = key.Age - 1
            })
            .ExecuteAsync();

        int query2 = await _sql.InsertSerialized<MyTableSerialized>("MyTableSerializedOther", row)
            .OnDuplicateKeyUpdate(key => new MyTableSerialized
            {
                Age = key.Age + 1,
                Password = "123456789"
            })
            .ExecuteAsync();

        SendConsoleMessage($"Insert: {query} row(s)", ConsoleColor.Blue);
        SendConsoleMessage($"Insert: {query2} row(s)", ConsoleColor.DarkBlue);
    }
    public static async Task UpdateAsyncTest()
    {
        int query = await _sql.UpdateSerialized<MyTableSerialized>()
            .SetTableFromEntity()
            .Set(key => new MyTableSerialized
            {
                Age = key.Age + 1
            })
            .Where(key => key.Age < 5)
            .ExecuteAsync();

        int query2 = await _sql.UpdateSerialized<MyTableSerialized>("MyTableSerializedOther")
            .Set(key => new MyTableSerialized
            {
                Status = "not active",
                Verified = !key.Verified
            })
            .Where(key => key.Verified == false || key.Password != "1" && key.Verified == true)
            .ExecuteAsync();

        SendConsoleMessage($"Update: {query} row(s)", ConsoleColor.Cyan);
        SendConsoleMessage($"Update: {query2} row(s)", ConsoleColor.DarkCyan);
    }
    public static async Task SelectAsyncTest()
    {
        System.Collections.Generic.IEnumerable<MyTableSerialized> query = await _sql.SelectSerialized<MyTableSerialized>()
            .Select("Age")
            .FromEntityTable()
            .Where(key => key.Age < 100)
            .OrderBy(key => key.Age, OrderType.ASC)
            .Limit(5)
            .ExecuteAsync();

        System.Collections.Generic.IEnumerable<MyTableSerialized> query2 = await _sql.SelectSerialized<MyTableSerialized>()
            .Select("*")
            .From("MyTableSerializedOther")
            .ExecuteAsync();

        SendConsoleMessage($"Select 1:", ConsoleColor.Red);
        foreach (MyTableSerialized q in query)
            Console.WriteLine($"Age => {q.Age},{q.Status}");

        SendConsoleMessage($"Select 2:", ConsoleColor.DarkRed);
        foreach (MyTableSerialized q in query2)
            Console.WriteLine($"{q.Account} {q.Email} {q.Status}...");
    }
    public static async Task DeleteAsyncTest()
    {
        int query = await _sql.DeleteSerialized<MyTableSerialized>()
            .FromEntityTable()
            .Where(key => key.Age < 100)
            .ExecuteAsync();

        int query2 = await _sql.DeleteSerialized<MyTableSerialized>()
            .From("MyTableSerializedOther")
            .ExecuteAsync();

        SendConsoleMessage($"Delete: {query} row(s)", ConsoleColor.Magenta);
        SendConsoleMessage($"Delete: {query2} row(s)", ConsoleColor.DarkMagenta);
    }
    #endregion
}