using Microsoft.Data.Sqlite;
using RAndG.Data;
using RAndG.Models;
using System;
using System.Collections.Generic;
namespace RAndG.Services
{
    public class MusicLibrary
    {
        public List<Track> GetTracks()
        {
            return SearchTracks("", "Название");
        }
        public List<Track> SearchTracks(string searchText, string sortBy)
        {
            var tracks = new List<Track>();
            searchText = searchText?.Trim() ?? "";
            string orderBy = sortBy switch
            {
                "Исполнитель" => "Artist COLLATE NOCASE, Title COLLATE NOCASE",
                "Путь к файлу" => "FilePath COLLATE NOCASE",
                "Сначала новые" => "Id DESC",
                _ => "Title COLLATE NOCASE"
            };
            using var connection = Database.GetConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            // Название сортировки выбирается только из заранее заданных вариантов.
            command.CommandText = $@"
                SELECT Id, FilePath, Title, Artist
                FROM Tracks
                WHERE @search = ''
                OR Title LIKE @pattern ESCAPE '\'
                OR COALESCE(Artist, '') LIKE @pattern ESCAPE '\'
                ORDER BY {orderBy};";
            string escapedSearch = searchText.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
            command.Parameters.AddWithValue("@search", searchText);
            command.Parameters.AddWithValue("@pattern", "%" + escapedSearch + "%");
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
        public bool AddTrack(Track track)
        {
            if (track == null || string.IsNullOrWhiteSpace(track.FilePath))
                return false;
            // В библиотеку добавляется ссылка на файл, а не его копия.
            if (!System.IO.File.Exists(track.FilePath))
                return false;
            using var connection = Database.GetConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT OR IGNORE INTO Tracks (FilePath, Title, Artist) VALUES (@filePath, @title, @artist);";
            command.Parameters.AddWithValue("@filePath", track.FilePath);
            command.Parameters.AddWithValue("@title", track.Title ?? "");
            command.Parameters.AddWithValue("@artist", track.Artist ?? "");
            return command.ExecuteNonQuery() > 0;
        }
        public void DeleteTrack(int id)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            // Удаляется только запись из базы, исходный аудиофайл сохраняется.
            command.CommandText = "DELETE FROM Tracks WHERE Id = @id;";
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }
    }
}