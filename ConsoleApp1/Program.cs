using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public class Repository
    {
        string sql = "insert into UserInfo (UserName,department) values (@usernName,@department)";

        public void Insert(List<UserInfo> list)
        {
            using var conn = new MySqlConnection(string.Empty);
            var insertData = list.Select(x => new
            {
                x.UserName,
                Department = x.Department.HasValue ? (int)x.Department.Value : -1
            });
            /*
             * 我的問題是Department因為某個特殊情況是null必須紀錄為-1，但在Enum不新增-1這個值
             * 以前要insert list都是直接在value傳入list
             * 但這次等於是Department為null的時候要在insert時改成-1，但又不動到原來Enum
             * 所以除了Select新的List之外有無其他方式可以達到這目的呢？
             * 我本來是想說使用Parameter的方式，但Parameter好像不能用在List？
             */
            conn.Open();
            conn.Execute(sql, insertData);
        }

        public void InsertByParameter(UserInfo userInfo)
        {
            using var conn = new MySqlConnection(string.Empty);
            conn.Open();
            var dp = new DynamicParameters();
            dp.Add("@UserName", userInfo.UserName);
            dp.Add("@Department", userInfo.Department);
            conn.Execute(sql, userInfo);
        }
    }

    public class UserInfo
    {
        public string UserName { get; set; }
        public DepartmentEnum? Department { get; set; }
    }

    public enum DepartmentEnum
    {
        TeamA = 1,
        TeamB = 2,
        TeamC = 3,
    }
}
