using Microsoft.Data.Sqlite;
using RAndG.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace RAndG.Data
{
    // Класс для создания базы данных и выполнения операций с композициями и плейлистами
    public class Database
    {
        private string connectionString = "Data Source=music_player.db;";
        public Database()
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string createTracksTable = "CREATE TABLE IF NOT EXISTS Tracks (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT, Artist TEXT, FilePath TEXT);";
                using (SqliteCommand command = new SqliteCommand(createTracksTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                string createPlaylistsTable = "CREATE TABLE IF NOT EXISTS Playlists (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);";
                using (SqliteCommand command = new SqliteCommand(createPlaylistsTable, connection))
                {
                    command.ExecuteNonQuery();
                }
                string createPlaylistTracksTable = "CREATE TABLE IF NOT EXISTS PlaylistTracks (PlaylistId INTEGER, TrackId INTEGER);";
                using (SqliteCommand command = new SqliteCommand(createPlaylistTracksTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public List<Track> LoadTracks()
        {
            List<Track> list = new List<Track>();
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Title, Artist, FilePath FROM Tracks";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["Id"]);
                            string title = reader["Title"].ToString();
                            string artist = reader["Artist"].ToString();
                            string filePath = reader["FilePath"].ToString();
                            Track track = new Track(id, title, artist, filePath);
                            list.Add(track);
                        }
                    }
                }
            }
            return list;
        }
        public void AddTrack(string title, string artist, string filePath)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Tracks (Title, Artist, FilePath) VALUES (@Title, @Artist, @FilePath)";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Artist", artist);
                    command.Parameters.AddWithValue("@FilePath", filePath);
                    command.ExecuteNonQuery();
                }
            }
        }
        // Каскадное удаление связей предотвращает появление ссылок на несуществующие файлы
        public void DeleteTrack(int trackId)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query1 = "DELETE FROM Tracks WHERE Id = @Id";
                using (SqliteCommand command = new SqliteCommand(query1, connection))
                {
                    command.Parameters.AddWithValue("@Id", trackId);
                    command.ExecuteNonQuery();
                }
                string query2 = "DELETE FROM PlaylistTracks WHERE TrackId = @TrackId";
                using (SqliteCommand command = new SqliteCommand(query2, connection))
                {
                    command.Parameters.AddWithValue("@TrackId", trackId);
                    command.ExecuteNonQuery();
                }
            }
        }
        public List<Playlist> LoadPlaylists(List<Track> allTracks)
        {
            List<Playlist> playlists = new List<Playlist>();
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name FROM Playlists";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int pId = Convert.ToInt32(reader["Id"]);
                            string pName = reader["Name"].ToString();
                            Playlist playlist = new Playlist(pId, pName);
                            playlists.Add(playlist);
                        }
                    }
                }
                foreach (Playlist playlist in playlists)
                {
                    string trackQuery = "SELECT TrackId FROM PlaylistTracks WHERE PlaylistId = @PlaylistId";
                    using (SqliteCommand command = new SqliteCommand(trackQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PlaylistId", playlist.Id);
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int tId = Convert.ToInt32(reader["TrackId"]);
                                foreach (Track t in allTracks)
                                {
                                    if (t.Id == tId)
                                    {
                                        playlist.Tracks.Add(t);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return playlists;
        }
        public void CreatePlaylist(string name)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Playlists (Name) VALUES (@Name)";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.ExecuteNonQuery();
                }
            }
        }
        // Каскадное удаление очищает таблицу отношений во избежание сиротских записей
        public void DeletePlaylist(int playlistId)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query1 = "DELETE FROM Playlists WHERE Id = @Id";
                using (SqliteCommand command = new SqliteCommand(query1, connection))
                {
                    command.Parameters.AddWithValue("@Id", playlistId);
                    command.ExecuteNonQuery();
                }
                string query2 = "DELETE FROM PlaylistTracks WHERE PlaylistId = @PlaylistId";
                using (SqliteCommand command = new SqliteCommand(query2, connection))
                {
                    command.Parameters.AddWithValue("@PlaylistId", playlistId);
                    command.ExecuteNonQuery();
                }
            }
        }
        public void AddTrackToPlaylist(int playlistId, int trackId)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO PlaylistTracks (PlaylistId, TrackId) VALUES (@PlaylistId, @TrackId)";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlaylistId", playlistId);
                    command.Parameters.AddWithValue("@TrackId", trackId);
                    command.ExecuteNonQuery();
                }
            }
        }
        public void RemoveTrackFromPlaylist(int playlistId, int trackId)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM PlaylistTracks WHERE PlaylistId = @PlaylistId AND TrackId = @TrackId";
                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlaylistId", playlistId);
                    command.Parameters.AddWithValue("@TrackId", trackId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}