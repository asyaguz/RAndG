using Microsoft.Data.Sqlite;
using RAndG.Data;
using RAndG.Models;
using RAndG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
            string orderBy = sortBy switch
            {
                "Исполнитель" => "Artist COLLATE NOCASE, Title COLLATE NOCASE",
                "Путь к файлу" => "FilePath COLLATE NOCASE",
                "Сначала новые" => "Id DESC",
                _ => "Title COLLATE NOCASE"
            };
            using var connection = Database.GetConnection();
            connection.Open();
            string sql = $@"
                SELECT Id, FilePath, Title, Artist
                FROM Tracks
                WHERE @search = ''
                    OR Title LIKE @pattern ESCAPE '\'
                    OR COALESCE(Artist, '') LIKE @pattern ESCAPE '\'
                ORDER BY {orderBy};
            ";
            string escapedSearch = searchText.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
            using var command = new SqliteCommand(sql, connection);
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
            using var connection = Database.GetConnection();
            connection.Open();
            string sql = "INSERT INTO Tracks (FilePath, Title, Artist) VALUES (@filePath, @title, @artist);";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@filePath", track.FilePath);
            command.Parameters.AddWithValue("@title", track.Title);
            command.Parameters.AddWithValue("@artist", track.Artist);
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
        public void DeleteTrack(int id)
        {
            using var connection = Database.GetConnection();
            connection.Open();
            string sql = "DELETE FROM Tracks WHERE Id = @id;";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }
    }
}