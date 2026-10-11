using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace RAndG.Data
{
    public static class Database
    {
        private static readonly string ConnectionString = "Data Source=RAndG.db";
        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }
        public static void Initialize()
        {
            using var connection = GetConnection();
            connection.Open();
            string sql = @"
                CREATE TABLE IF NOT EXISTS Tracks
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FilePath TEXT NOT NULL UNIQUE,
                    Title TEXT NOT NULL,
                    Artist TEXT
                );
            ";
            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
             
        }
    }
}
