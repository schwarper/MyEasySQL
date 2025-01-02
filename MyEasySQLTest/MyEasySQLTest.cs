using System;
using System.Threading.Tasks;
using MyEasySQL;
using MyEasySQL.Utils;

namespace MyEasySQLTest;

class Program
{
    public static MySQL sql = new("host", "name", "user", "password");

    public async static Task Main()
    {
        await CreateTable();
        await Insert();
        await Update();
        await Select();
        await Delete();

        await SQLSerializer.MainSerialized();
    }

    private static async Task CreateTable()
    {
        MyEasySQL.Queries.CreateTableQuery query = sql.CreateTable("MyTable")
            .AddColumn("Account", DataTypes.VARCHAR, typeValue: "255", flag: ColumnFlags.NotNull)
            .AddColumn("Password", DataTypes.INT, flag: ColumnFlags.NotNull)
            .AddColumn("Status", DataTypes.TEXT, flag: ColumnFlags.NotNull, defaultValue: "active")
            .AddColumn("Verified", DataTypes.BOOLEAN, flag: ColumnFlags.NotNull)
            .AddColumn("UniqueId", DataTypes.INT, flag: ColumnFlags.PrimaryKey | ColumnFlags.Unique);
        await query.ExecuteAsync();

        await sql.CreateTable("NoteTable")
            .AddColumn("Surname", DataTypes.VARCHAR, typeValue: "255")
            .AddColumn("Name", DataTypes.VARCHAR, typeValue: "255")
            .AddColumn("Id", DataTypes.INT, flag: ColumnFlags.PrimaryKey | ColumnFlags.Unique)
            .AddColumn("Note", DataTypes.INT)
            .ExecuteAsync();
    }

    private static async Task Insert()
    {
        MyEasySQL.Queries.InsertQuery query = sql.Insert("MyTable")
            .Value("Account", "schwarper")
            .Value("Password", 400)
            .Value("Verified", true)
            .Value("UniqueId", 1000)
            .OnDuplicateKeyUpdate("Account", "schwarper1");
        await query.ExecuteAsync();

        await sql.Insert("NoteTable")
            .Value("SurName", "surn")
            .Value("Name", "nam")
            .Value("Id", 10000)
            .Value("Note", 10)
            .ExecuteAsync();
    }

    private static async Task Update()
    {
        MyEasySQL.Queries.UpdateQuery query = sql.Update("MyTable")
            .Set("Verified", false)
            .Set("Password", 100)
            .Where("UniqueId", Operators.EQUAL, 1000)
            .Where("Account", Operators.NOT_EQUAL, "something", LogicalOperators.AND);
        await query.ExecuteAsync();

        await sql.Update("NoteTable")
            .Set("Note", 100)
            .ExecuteAsync();
    }

    private static async Task Select()
    {
        System.Collections.Generic.IEnumerable<dynamic> query = await sql.Select("*")
            .From("MyTable")
            .Where("Verified", Operators.EQUAL, false)
            .Limit(1)
            .ReadAsync<dynamic>();

        System.Collections.Generic.IEnumerable<dynamic> query2 = await sql.Select("Name", "Note")
            .From("NoteTable")
            .OrderBy("Note", OrderType.DESC)
            .ReadAsync<dynamic>();

        foreach (dynamic q in query)
        {
            Console.WriteLine(q);
        }

        foreach (dynamic qq in query2)
        {
            Console.WriteLine(qq);
        }
    }

    private static async Task Delete()
    {
        MyEasySQL.Queries.DeleteQuery query = sql.Delete()
            .From("MyTable")
            .Where("Verified", Operators.NOT_EQUAL, true);
        await query.ExecuteAsync();

        await sql.Delete()
            .From("NoteTable")
            .ExecuteAsync();
    }
}