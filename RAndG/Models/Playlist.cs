using System;
using System.Collections.Generic;

namespace RAndG.Models
{
    // Класс, описывающий плейлист и содержащиеся в нем композиции
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Track> Tracks { get; set; }
        // Создает новый плейлист с указанным идентификатором и названием
        public Playlist(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Tracks = new List<Track>();
        }
    }
}