using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAndG.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Track> Tracks { get; set; } = new();
        public override string ToString()
        {
            return Name;
        }
    }
}
