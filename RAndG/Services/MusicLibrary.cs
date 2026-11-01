using System;
using System.Collections.Generic;
using RAndG.Models;
using RAndG.Data;

namespace RAndG.Services
{
    public class MusicLibrary
    {
        private List<Track> allTracks;
        private Database db;

        public MusicLibrary(Database database)
        {
            this.db = database;
            RefreshLibrary();
        }

        public void RefreshLibrary()
        {
            this.allTracks = db.LoadTracks();
            this.allTracks.Sort(delegate (Track x, Track y) { return x.Title.CompareTo(y.Title); });
        }

        public List<Track> GetAllTracks()
        {
            return this.allTracks;
        }

        public void AddTrack(string title, string artist, string filePath)
        {
            db.AddTrack(title, artist, filePath);
            RefreshLibrary();
        }

        public void RemoveTrack(int trackId)
        {
            db.DeleteTrack(trackId);
            RefreshLibrary();
        }

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
