using System;
using System.Collections.Generic;
using RAndG.Models;
using RAndG.Data;

namespace RAndG.Services
{
    public class PlaylistService
    {
        private List<Playlist> playlists;
        private Database db;
        private List<Track> mainLibraryTracks;

        public PlaylistService(Database database, List<Track> allTracks)
        {
            this.db = database;
            this.mainLibraryTracks = allTracks;
            RefreshPlaylists();
        }

        public void RefreshPlaylists()
        {
            this.playlists = db.LoadPlaylists(mainLibraryTracks);
        }

        public List<Playlist> GetPlaylists()
        {
            return this.playlists;
        }

        public void CreatePlaylist(string name)
        {
            db.CreatePlaylist(name);
            RefreshPlaylists();
        }

        public void DeletePlaylist(int playlistId)
        {
            db.DeletePlaylist(playlistId);
            RefreshPlaylists();
        }

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

        public void RemoveTrackFromPlaylist(int playlistId, int trackId)
        {
            db.RemoveTrackFromPlaylist(playlistId, trackId);
            RefreshPlaylists();
        }
    }
}
