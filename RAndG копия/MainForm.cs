using RAndG.Services;
using RAndG.Models;
using RAndG.Players;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAndG
{
    public partial class MainForm : Form
    {
        private readonly MusicLibrary musicLibrary;
        private readonly PlaylistPlayer playlistPlayer;
        public MainForm()
        {
            InitializeComponent();
            musicLibrary = new MusicLibrary();
            playlistPlayer = new PlaylistPlayer();
        }
    }
}
