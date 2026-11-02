using System;
using System.Collections.Generic;
using RAndG.Models;
using RAndG.Data;

namespace RAndG.Services
{
    public class PlaylistService
    {
        // Класс для управления плейлистами и композициями внутри них
        private List<Playlist> playlists;
        private Database db;
        private List<Track> mainLibraryTracks;
        // Создает сервис управления плейлистами и загружает существующие плейлисты
        public PlaylistService(Database database, List<Track> allTracks)
        {
            this.db = database;
            this.mainLibraryTracks = allTracks;
            RefreshPlaylists();
        }
        // Обновляет список плейлистов данными из базы данных
        public void RefreshPlaylists()
        {
            this.playlists = db.LoadPlaylists(mainLibraryTracks);
        }
        // Возвращает список всех плейлистов
        public List<Playlist> GetPlaylists()
        {
            return this.playlists;
        }
        // Создает новый плейлист с указанным названием
        public void CreatePlaylist(string name)
        {
            db.CreatePlaylist(name);
            RefreshPlaylists();
        }
        // Удаляет плейлист по его идентификатору
        public void DeletePlaylist(int playlistId)
        {
            db.DeletePlaylist(playlistId);
            RefreshPlaylists();
        }
        // Построчная проверка по числовому Id исключает дублирование композиций внутри списков
        public void AddTrackToPlaylist(int playlistId, Track track)
        {
            foreach (Playlist p in playlists)
            {
                if (p.Id == playlistId)
                {
                    bool alreadyExists = false;
                    foreach (Track t in p.Tracks)
                    {
                        if (t.Id == track.Id)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }
                    if (alreadyExists == false)
                    {
                        db.AddTrackToPlaylist(playlistId, track.Id);
                        RefreshPlaylists();
                    }
                    break;
                }
            }
        }
        // Удаляет композицию из выбранного плейлиста
        public void RemoveTrackFromPlaylist(int playlistId, int trackId)
        {
            db.RemoveTrackFromPlaylist(playlistId, trackId);
            RefreshPlaylists();
        }
    }
}