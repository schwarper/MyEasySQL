using MyEasySQL;
using MyEasySQL.Utils;
using System;
using System.Threading.Tasks;

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
    }

    private static async Task CreateTable()
    {
        var query = sql.CreateTable("MyTable")
            .AddColumn("Account", DataTypes.VARCHAR, typeValue: "255", notNull: true)
            .AddColumn("Password", DataTypes.INT, notNull: true)
            .AddColumn("Status", DataTypes.TEXT, notNull: true, defaultValue: "active")
            .AddColumn("Verified", DataTypes.BOOLEAN, notNull: true)
            .AddColumn("UniqueId", DataTypes.INT, primaryKey: true, unique: true);
        await query.ExecuteAsync();

        await sql.CreateTable("NoteTable")
            .AddColumn("Surname", DataTypes.VARCHAR, typeValue: "255")
            .AddColumn("Name", DataTypes.VARCHAR, typeValue: "255")
            .AddColumn("Id", DataTypes.INT, primaryKey: true, unique: true)
            .AddColumn("Note", DataTypes.INT)
            .ExecuteAsync();
    }

    private static async Task Insert()
    {
        var query = sql.Insert("MyTable")
            .Value("Account", "schwarper")
            .Value("Password", 400)
            .Value("Verified", true)
            .Value("UniqueId", 1000);
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
        var query = sql.Update("MyTable")
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
        Console.WriteLine("READING");

        var query = await sql.Select("*")
            .From("MyTable")
            .Where("Verified", Operators.EQUAL, false)
            .Limit(1)
            .ReadAsync<dynamic>();

        var query2 = await sql.Select("Name", "Note")
            .From("NoteTable")
            .OrderBy("Note", OrderType.DESC)
            .ReadAsync<dynamic>();

        foreach (var q in query)
        {
            Console.WriteLine(q);
        }

        foreach (var qq in query2)
        {
            Console.WriteLine(qq);
        }
    }

    private static async Task Delete()
    {
        var query = sql.Delete()
            .From("MyTable")
            .Where("Verified", Operators.NOT_EQUAL, true);
        await query.ExecuteAsync();

        await sql.Delete()
            .From("NoteTable")
            .ExecuteAsync();
    }
}