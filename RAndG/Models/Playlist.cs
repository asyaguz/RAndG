using System;
using System.Collections.Generic;

namespace RAndG.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Track> Tracks { get; set; }
        public Playlist(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Tracks = new List<Track>();
        }
    }
}