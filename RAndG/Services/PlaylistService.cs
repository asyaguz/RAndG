using Microsoft.Data.Sqlite;
using RAndG.Data;
using RAndG.Models;
using System;
using System.Collections.Generic;
namespace RAndG.Services
{
    public class PlaylistService
    {
        public List<Playlist> GetPlaylists()
        {
            var playlists = new List<Playlist>();
            using var connection = Database.GetConnection();
            connection.Open();
            string sql = "SELECT Id, Name FROM Playlists ORDER BY Name COLLATE NOCASE;";
            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                playlists.Add(new Playlist
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
            reader.Close();
            foreach (Playlist playlist in playlists)
                playlist.Tracks = GetTracks(playlist.Id);
            return playlists;
        }
        public Playlist? GetPlaylist(int playlistId)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            string sql = "SELECT Id, Name FROM Playlists WHERE Id = @id;";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", playlistId);
            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return null;
            var playlist = new Playlist
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            };
            reader.Close();
            playlist.Tracks = GetTracks(playlist.Id);
            return playlist;
        }
        public bool CreatePlaylist(string name)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return false;
            using var connection = Database.GetConnection();
            connection.Open();
            string sql = "INSERT INTO Playlists (Name) VALUES (@name);";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@name", name);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqliteException)
            {
                return false;
            }
        }
        public bool DeletePlaylist(int playlistId)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = new SqliteCommand("DELETE FROM PlaylistTracks WHERE PlaylistId = @id;", connection, transaction);
                command.Parameters.AddWithValue("@id", playlistId);
                command.ExecuteNonQuery();
                using var deleteCommand = new SqliteCommand("DELETE FROM Playlists WHERE Id = @id;", connection, transaction);
                deleteCommand.Parameters.AddWithValue("@id", playlistId);
                int affected = deleteCommand.ExecuteNonQuery();
                transaction.Commit();
                return affected > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public bool AddTrack(int playlistId, int trackId)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            string sql = @"
                INSERT OR IGNORE INTO PlaylistTracks (PlaylistId, TrackId, Position)
                VALUES (@playlistId, @trackId, (SELECT COALESCE(MAX(Position), -1) + 1 FROM PlaylistTracks WHERE PlaylistId = @playlistId));
            ";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@playlistId", playlistId);
            command.Parameters.AddWithValue("@trackId", trackId);
            return command.ExecuteNonQuery() > 0;
        }
        public bool RemoveTrack(int playlistId, int trackId)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            string sql = "DELETE FROM PlaylistTracks WHERE PlaylistId = @playlistId AND TrackId = @trackId;";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@playlistId", playlistId);
            command.Parameters.AddWithValue("@trackId", trackId);
            return command.ExecuteNonQuery() > 0;
        }
        private List<Track> GetTracks(int playlistId)
        {
            var tracks = new List<Track>();
            using var connection = Database.GetConnection();
            connection.Open();
            string sql = @"
                SELECT T.Id, T.FilePath, T.Title, T.Artist
                FROM PlaylistTracks PT
                INNER JOIN Tracks T ON T.Id = PT.TrackId
                WHERE PT.PlaylistId = @playlistId
                ORDER BY PT.Position, T.Title COLLATE NOCASE;
            ";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@playlistId", playlistId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tracks.Add(new Track
                {
                    Id = reader.GetInt32(0),
                    FilePath = reader.GetString(1),
                    Title = reader.GetString(2),
                    Artist = reader.IsDBNull(3) ? "" : reader.GetString(3)
                });
            }
            return tracks;
        }
    }
}
