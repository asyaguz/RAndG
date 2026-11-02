using System;
using System.Collections.Generic;
using RAndG.Models;
using RAndG.Data;

namespace RAndG.Services
{
    // Класс для управления музыкальной библиотекой
    public class MusicLibrary
    {
        private List<Track> allTracks;
        private Database db;
        // Создает музыкальную библиотеку и связывает ее с базой данных
        public MusicLibrary(Database database)
        {
            this.db = database;
            RefreshLibrary();
        }
        // Обновляет список композиций, загружая актуальные данные из базы данных
        public void RefreshLibrary()
        {
            this.allTracks = db.LoadTracks();
            this.allTracks.Sort(delegate (Track x, Track y) { return x.Title.CompareTo(y.Title); });
        }
        // Возвращает полный список музыкальных композиций
        public List<Track> GetAllTracks()
        {
            return this.allTracks;
        }
        // Добавляет новую композицию в музыкальную библиотеку
        public void AddTrack(string title, string artist, string filePath)
        {
            db.AddTrack(title, artist, filePath);
            RefreshLibrary();
        }
        // Удаляет композицию из библиотеки по ее идентификатору
        public void RemoveTrack(int trackId)
        {
            db.DeleteTrack(trackId);
            RefreshLibrary();
        }
        // Приведение строк к нижнему регистру делает поиск нечувствительным к раскладке ввода
        public List<Track> Search(string query)
        {
            List<Track> results = new List<Track>();
            string lowerQuery = query.ToLower();
            foreach (Track t in allTracks)
            {
                if (t.Title.ToLower().Contains(lowerQuery) || t.Artist.ToLower().Contains(lowerQuery))
                {
                    results.Add(t);
                }
            }
            results.Sort(delegate (Track x, Track y) { return x.Title.CompareTo(y.Title); });
            return results;
        }
    }
}
