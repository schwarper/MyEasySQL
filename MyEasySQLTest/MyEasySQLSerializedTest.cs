﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MyEasySQL.Utils;
using static MyEasySQLTest.Program;

namespace MyEasySQLTest;

public class SQLSerializer
{
    public class MyTableSerialized
    {
        [ColumnNotNull]
        public string Account = string.Empty;

        public int Password;

        [ColumnDefaultValue("active")]
        public string? Status;

        [ColumnDefaultValue(0)]
        public bool Verified;

        [ColumnUnique]
        public string? Email;

        [ColumnPrimaryKey, ColumnAutoIncrement, ColumnNotNull]
        public int UniqueId;
    }

    public async static Task MainSerialized()
    {
        await CreateTable();
        await Insert();
        await Update();
        await Select();
    }

    private static async Task CreateTable()
    {
        await sql.CreateTableSerialized<MyTableSerialized>("MyTableSerialized")
            // This is default true
            .SetIfNotExist(true)
            .ExecuteAsync();
    }

    private static async Task Insert()
    {
        MyTableSerialized column = new()
        {
            Account = "Mert",
            Password = 200,
            Status = "deactive",
            Email = "test@test.com"
        };

        await sql.InsertSerialized("MyTableSerialized", column)
            .OnDuplicateKeyUpdate("Password", "Password + 1")
            .OnDuplicateKeyUpdate("Status", "active")
            .ExecuteAsync();
    }

    private static async Task Update()
    {
        System.Collections.Generic.IEnumerable<MyTableSerialized> column = await sql.SelectSerialized<MyTableSerialized>()
            .From("MyTableSerialized").ReadAsync();

        MyTableSerialized? firstColumn = column.FirstOrDefault();

        if (firstColumn == null)
        {
            return;
        }

        firstColumn.Password = 10421041;
        firstColumn.Status = "active";
        firstColumn.Verified = true;

        await sql.UpdateSerialized("MyTableSerialized", firstColumn)
            .ExecuteAsync();
    }

    private static async Task Select()
    {
        System.Collections.Generic.IEnumerable<MyTableSerialized> query = await sql.SelectSerialized<MyTableSerialized>()
            .From("MyTableSerialized")
            .ReadAsync();

        foreach (MyTableSerialized q in query)
        {
            Console.WriteLine($"{q.Account}, {q.Password}, {q.Email}"); //...
        }
    }
}