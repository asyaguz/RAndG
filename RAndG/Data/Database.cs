using Microsoft.Data.Sqlite;
using System;
using System.IO;
namespace RAndG.Data
{
    public static class Database
    {
        private static readonly string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RAndG");
        private static readonly string DatabasePath = Path.Combine(DataDirectory, "RAndG.db");
        private static readonly string ConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true,
            Pooling = true,
            DefaultTimeout = 5
        }.ToString();
        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(ConnectionString);
        }
        public static void Initialize()
        {
            Directory.CreateDirectory(DataDirectory);
            MigrateExistingDatabase();
            using var connection = GetConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Tracks
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FilePath TEXT NOT NULL UNIQUE,
                    Title TEXT NOT NULL,
                    Artist TEXT
                );
                CREATE TABLE IF NOT EXISTS Playlists
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL COLLATE NOCASE UNIQUE
                );
                CREATE TABLE IF NOT EXISTS PlaylistTracks
                (
                    PlaylistId INTEGER NOT NULL,
                    TrackId INTEGER NOT NULL,
                    Position INTEGER NOT NULL DEFAULT 0,
                    PRIMARY KEY (PlaylistId, TrackId),
                    FOREIGN KEY (PlaylistId) REFERENCES Playlists(Id) ON DELETE CASCADE,
                    FOREIGN KEY (TrackId) REFERENCES Tracks(Id) ON DELETE CASCADE
                );
                CREATE INDEX IF NOT EXISTS IX_Tracks_Title ON Tracks(Title COLLATE NOCASE);
                CREATE INDEX IF NOT EXISTS IX_Tracks_Artist ON Tracks(Artist COLLATE NOCASE);
                CREATE INDEX IF NOT EXISTS IX_PlaylistTracks_Order ON PlaylistTracks(PlaylistId, Position);
            ";
            command.ExecuteNonQuery();
        }
        private static void MigrateExistingDatabase()
        {
            if (File.Exists(DatabasePath))
                return;
            string[] oldPaths =
            {
                Path.Combine(Environment.CurrentDirectory, "RAndG.db"),
                Path.Combine(AppContext.BaseDirectory, "RAndG.db")
            };
            foreach (string oldPath in oldPaths)
            {
                if (!File.Exists(oldPath))
                    continue;
                // Копируем существующую библиотеку, не удаляя исходную базу.
                File.Copy(oldPath, DatabasePath, false);
                return;
            }
        }
    }
}