using System;

namespace RAndG.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string FilePath { get; set; }
        public Track(int id, string title, string artist, string filePath)
        {
            this.Id = id;
            this.Title = title;
            this.Artist = artist;
            this.FilePath = filePath;
        }
    }
}
