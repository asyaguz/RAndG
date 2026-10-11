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
                PRAGMA foreign_keys = ON;
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
            ";
            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}
