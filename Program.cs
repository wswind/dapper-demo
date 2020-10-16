using Dapper;
using DapperDemo.Infra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace DapperDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            DIManager dIManager = new DIManager(
                x =>
                {
                    x.AddSingletonDbConnectionFactory("Default");
                    x.AddSingleton(sp =>
                    {
                        return GetConfiguration();
                    });
                });

            var factory = dIManager.For<IDbConnectionFactory>();

            using var connection = factory.CreateConnection();

            //List Support
            Console.WriteLine("use select where in:");
            var lst1 = connection.Query("select * from Table_1 where Id in @Ids", new { Ids = new int[] { 1, 2, 3 } });
            ShowList(lst1);

            //TVP
            Console.WriteLine("use tvp:");
            var tvpTable = new DataTable();
            tvpTable.Columns.Add(new DataColumn("Id", typeof(int)));
            tvpTable.Rows.Add(1);
            tvpTable.Rows.Add(2);
            tvpTable.Rows.Add(3);
            var sql = "SELECT t1.* FROM Table_1 t1 INNER JOIN @tvp t2 ON t1.Id = t2.Id";
            var lst2 = connection.Query(sql, new { tvp = tvpTable.AsTableValuedParameter("dbo.IDListType") });
            ShowList(lst2);

            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }

        static private void ShowList<T>(IEnumerable<T> lst)
        {
            foreach(var item in lst)
            {
                Console.WriteLine(item);
            }
        }

        static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: false)
                 .Build();
        }

    }
}
