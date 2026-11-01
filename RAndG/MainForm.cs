using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RAndG.Models;
using RAndG.Data;
using RAndG.Services;
using RAndG.Player;

namespace RAndG
{
    public partial class MainForm : Form
    {
        private Database db;
        private MusicLibrary library;
        private PlaylistService playlistService;
        private MusicPlayer player;
        private AudioFileValidator validator;
        private PerformanceMonitor monitor;

        private List<Track> activeLibraryList;

        public MainForm()
        {
            InitializeComponent();

            db = new Database();
            library = new MusicLibrary(db);
            playlistService = new PlaylistService(db, library.GetAllTracks());
            player = new MusicPlayer();
            validator = new AudioFileValidator();
            monitor = new PerformanceMonitor();

            UpdateLibraryUI(library.GetAllTracks());
            UpdatePlaylistsUI();
        }

        private void UpdateLibraryUI(List<Track> tracks)
        {
            activeLibraryList = tracks;
            lstLibrary.Items.Clear();
            foreach (Track t in tracks)
            {
                lstLibrary.Items.Add(t.Title);
            }
        }

        private void UpdatePlaylistsUI()
        {
            lstPlaylists.Items.Clear();
            List<Playlist> list = playlistService.GetPlaylists();
            foreach (Playlist p in list)
            {
                lstPlaylists.Items.Add(p.Name);
            }
        }
        private void btnAddTrack_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Музыкальные файлы|*.mp3;*.wav";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                if (validator.ValidateFile(path))
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

                    string artist = "Неизвестный исполнитель";
                    string title = fileName;

                    // Если в названии файла есть дефис, автоматически разделяем его
                    if (fileName.Contains(" - "))
                    {
                        string[] parts = fileName.Split(new string[] { " - " }, StringSplitOptions.None);
                        if (parts.Length >= 2)
                        {
                            artist = parts[0].Trim();
                            title = parts[1].Trim();
                        }
                    }

                    library.AddTrack(title, artist, path);
                    playlistService.RefreshPlaylists();
                    UpdateLibraryUI(library.GetAllTracks());
                }
                else
                {
                    MessageBox.Show("Указанный файл недоступен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDeleteTrack_Click(object sender, EventArgs e)
        {
            int index = lstLibrary.SelectedIndex;
            if (index >= 0 && index < activeLibraryList.Count)
            {
                Track target = activeLibraryList[index];
                library.RemoveTrack(target.Id);
                playlistService.RefreshPlaylists();
                UpdateLibraryUI(library.GetAllTracks());
                lstPlaylistTracks.Items.Clear();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            monitor.StartMeasure();
            string query = txtSearch.Text;
            if (string.IsNullOrEmpty(query) == false)
            {
                List<Track> found = library.Search(query);
                UpdateLibraryUI(found);
            }
            monitor.EndMeasure("Поиск в базе данных");
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            UpdateLibraryUI(library.GetAllTracks());
        }

        private void btnCreatePlaylist_Click(object sender, EventArgs e)
        {
            string name = txtPlaylistName.Text;
            if (string.IsNullOrEmpty(name) == false)
            {
                playlistService.CreatePlaylist(name);
                UpdatePlaylistsUI();
                txtPlaylistName.Text = "";
            }
        }

        private void btnDeletePlaylist_Click(object sender, EventArgs e)
        {
            int index = lstPlaylists.SelectedIndex;
            if (index >= 0)
            {
                List<Playlist> allP = playlistService.GetPlaylists();
                Playlist target = allP[index];
                playlistService.DeletePlaylist(target.Id);
                UpdatePlaylistsUI();
                lstPlaylistTracks.Items.Clear();
            }
        }

        private void lstPlaylists_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lstPlaylists.SelectedIndex;
            if (index >= 0)
            {
                playlistService.RefreshPlaylists();
                List<Playlist> allP = playlistService.GetPlaylists();
                Playlist selected = allP[index];

                lstPlaylistTracks.Items.Clear();
                foreach (Track t in selected.Tracks)
                {
                    lstPlaylistTracks.Items.Add(t.Title);
                }
            }
        }

        private void btnAddTrackToP_Click(object sender, EventArgs e)
        {
            int trackIndex = lstLibrary.SelectedIndex;
            int playlistIndex = lstPlaylists.SelectedIndex;

            if (trackIndex >= 0 && playlistIndex >= 0)
            {
                Track t = activeLibraryList[trackIndex];
                List<Playlist> allP = playlistService.GetPlaylists();
                Playlist p = allP[playlistIndex];

                playlistService.AddTrackToPlaylist(p.Id, t);
                lstPlaylists_SelectedIndexChanged(null, null);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите трек слева и плейлист справа!", "Подсказка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void btnRemoveTrackFromP_Click(object sender, EventArgs e)
        {
            int playlistIndex = lstPlaylists.SelectedIndex;
            int trackIndex = lstPlaylistTracks.SelectedIndex;

            if (playlistIndex >= 0 && trackIndex >= 0)
            {
                List<Playlist> allP = playlistService.GetPlaylists();
                Playlist p = allP[playlistIndex];
                Track t = p.Tracks[trackIndex];

                playlistService.RemoveTrackFromPlaylist(p.Id, t.Id);
                lstPlaylists_SelectedIndexChanged(null, null);
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            int index = lstLibrary.SelectedIndex;
            if (index >= 0)
            {
                try
                {
                    Track target = activeLibraryList[index];
                    if (validator.ValidateFile(target.FilePath))
                    {
                        List<string> paths = new List<string>();
                        foreach (Track t in activeLibraryList)
                        {
                            paths.Add(t.FilePath);
                        }

                        player.SetQueue(paths, index);
                        player.Play();
                        lblCurrentTrack.Text = "Сейчас играет: " + target.Title;
                        playbackTimer.Start();
                    }
                    else
                    {
                        MessageBox.Show("Файл недоступен по указанному пути.", "Ошибка воспроизведения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла системная ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnPause_Click(object sender, EventArgs e)
        {
            player.Pause();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            player.Stop();
            playbackTimer.Stop();
            trackProgress.Value = 0;
            lblTime.Text = "00:00 / 00:00";
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            player.Next();
            string currentPath = player.GetCurrentTrackPath();
            if (string.IsNullOrEmpty(currentPath) == false)
            {
                foreach (Track t in activeLibraryList)
                {
                    if (t.FilePath == currentPath)
                    {
                        lblCurrentTrack.Text = "Сейчас играет: " + t.Title;
                        break;
                    }
                }
            }
        }
        private void btnPrev_Click(object sender, EventArgs e)
        {
            player.Previous();
            string currentPath = player.GetCurrentTrackPath();
            if (string.IsNullOrEmpty(currentPath) == false)
            {
                foreach (Track t in activeLibraryList)
                {
                    if (t.FilePath == currentPath)
                    {
                        lblCurrentTrack.Text = "Сейчас играет: " + t.Title;
                        break;
                    }
                }
            }
        }
        private void trackVolume_Scroll(object sender, EventArgs e)
        {
            player.SetVolume(trackVolume.Value);
        }

        private void trackProgress_Scroll(object sender, EventArgs e)
        {
            double total = player.GetTotalDurationSeconds();
            if (total > 0)
            {
                player.SetPositionSeconds(trackProgress.Value);
            }
        }

        private void chkLoop_CheckedChanged(object sender, EventArgs e)
        {
            player.LoopTrack = chkLoop.Checked;
        }

        private void chkShuffle_CheckedChanged(object sender, EventArgs e)
        {
            player.Shuffle = chkShuffle.Checked;
        }

        private void playbackTimer_Tick(object sender, EventArgs e)
        {
            double current = player.GetCurrentPositionSeconds(); double total = player.GetTotalDurationSeconds();
            int curMin = (int)current / 60;
            int curSec = (int)current % 60;
            int totMin = (int)total / 60;
            int totSec = (int)total % 60;
            lblTime.Text = curMin.ToString("D2") + ":" + curSec.ToString("D2") + " / " +
            totMin.ToString("D2") + ":" + totSec.ToString("D2");
            if (total > 0)
            {
                trackProgress.Maximum = (int)total;
                if (trackProgress.Focused == false)
                {
                    trackProgress.Value = (int)current;
                }
            }
            if (current >= total && total > 0)
            {
                if (chkLoop.Checked)
                {
                    player.Play();
                }
                else
                {
                    btnNext_Click(null, null);
                }
            }
        }
    }
}