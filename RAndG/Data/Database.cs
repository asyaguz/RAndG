using System;
using System.Collections.Generic;
using System.Data.SQLite;
using RAndG.Models;

namespace RAndG.Data
{
    public class Database
    {
        private string connectionString = "Data Source=music_player.db;Version=3;";

        public Database()
        {
            // Создаем файл БД и таблицы, если их нет
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Таблица треков
                string createTracksTable = "CREATE TABLE IF NOT EXISTS Tracks (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT, Artist TEXT, FilePath TEXT);";
                using (SQLiteCommand command = new SQLiteCommand(createTracksTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Таблица плейлистов
                string createPlaylistsTable = "CREATE TABLE IF NOT EXISTS Playlists (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT);";
                using (SQLiteCommand command = new SQLiteCommand(createPlaylistsTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Сводная таблица связи треков и плейлистов (многие-ко-многим)
                string createPlaylistTracksTable = "CREATE TABLE IF NOT EXISTS PlaylistTracks (PlaylistId INTEGER, TrackId INTEGER);";
                using (SQLiteCommand command = new SQLiteCommand(createPlaylistTracksTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // ЗАГРУЗКА ТРЕКОВ
        public List<Track> LoadTracks()
        {
            List<Track> list = new List<Track>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Title, Artist, FilePath FROM Tracks";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
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

        // ДОБАВЛЕНИЕ ТРЕКА
        public void AddTrack(string title, string artist, string filePath)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Tracks (Title, Artist, FilePath) VALUES (@Title, @Artist, @FilePath)";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Artist", artist);
                    command.Parameters.AddWithValue("@FilePath", filePath);
                    command.ExecuteNonQuery();
                }
            }
        }

        // УДАЛЕНИЕ ТРЕКА ИЗ БИБЛИОТЕКИ
        public void DeleteTrack(int trackId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Удаляем сам трек
                string query1 = "DELETE FROM Tracks WHERE Id = @Id";
                using (SQLiteCommand command = new SQLiteCommand(query1, connection))
                {
                    command.Parameters.AddWithValue("@Id", trackId);
                    command.ExecuteNonQuery();
                }

                // И связи этого трека с плейлистами
                string query2 = "DELETE FROM PlaylistTracks WHERE TrackId = @TrackId";
                using (SQLiteCommand command = new SQLiteCommand(query2, connection))
                {
                    command.Parameters.AddWithValue("@TrackId", trackId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // ЗАГРУЗКА ПЛЕЙЛИСТОВ С ИХ ТРЕКАМИ
        public List<Playlist> LoadPlaylists(List<Track> allTracks)
        {
            List<Playlist> playlists = new List<Playlist>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name FROM Playlists";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
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

                // Теперь заполняем треки для каждого плейлиста
                foreach (Playlist playlist in playlists)
                {
                    string trackQuery = "SELECT TrackId FROM PlaylistTracks WHERE PlaylistId = @PlaylistId";
                    using (SQLiteCommand command = new SQLiteCommand(trackQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PlaylistId", playlist.Id);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int tId = Convert.ToInt32(reader["TrackId"]);
                                // Ищем трек в общей коллекции по его Id
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

        // СОЗДАТЬ ПЛЕЙЛИСТ
        public void CreatePlaylist(string name)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Playlists (Name) VALUES (@Name)";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.ExecuteNonQuery();
                }
            }
        }

        // УДАЛИТЬ ПЛЕЙЛИСТ
        public void DeletePlaylist(int playlistId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query1 = "DELETE FROM Playlists WHERE Id = @Id";
                using (SQLiteCommand command = new SQLiteCommand(query1, connection))
                {
                    command.Parameters.AddWithValue("@Id", playlistId);
                    command.ExecuteNonQuery();
                }

                string query2 = "DELETE FROM PlaylistTracks WHERE PlaylistId = @PlaylistId";
                using (SQLiteCommand command = new SQLiteCommand(query2, connection))
                {
                    command.Parameters.AddWithValue("@PlaylistId", playlistId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // ДОБАВИТЬ ТРЕК В ПЛЕЙЛИСТ
        public void AddTrackToPlaylist(int playlistId, int trackId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO PlaylistTracks (PlaylistId, TrackId) VALUES (@PlaylistId, @TrackId)";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlaylistId", playlistId);
                    command.Parameters.AddWithValue("@TrackId", trackId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // УДАЛИТЬ ТРЕК ИЗ ПЛЕЙЛИСТА
        public void RemoveTrackFromPlaylist(int playlistId, int trackId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM PlaylistTracks WHERE PlaylistId = @PlaylistId AND TrackId = @TrackId";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlaylistId", playlistId);
                    command.Parameters.AddWithValue("@TrackId", trackId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
