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
            // Устанавливаем лавандовую палитру при запуске
            this.BackColor = Color.FromArgb(242, 241, 246);
            Color buttonColor = Color.FromArgb(223, 219, 237);
            Color textColor = Color.FromArgb(70, 68, 81);
            Font mainFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            Button[] allButtons = new Button[] {
               btnSearch, btnClearSearch, btnCreatePlaylist, btnAddTrack,
               btnDeleteTrack, btnAddTrackToP, btnDeletePlaylist,
               btnRemoveTrackFromP, btnPrev, btnPause, btnStop, btnNext
           };
            foreach (Button btn in allButtons)
            {
                btn.BackColor = buttonColor;
                btn.ForeColor = textColor;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = mainFont;
            }
            btnPlay.BackColor = Color.FromArgb(204, 226, 214);
            btnPlay.ForeColor = textColor;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
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
        private void BtnAddTrackClick(object sender, EventArgs e)
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
        private void BtnDeleteTrackClick(object sender, EventArgs e)
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
        private void BtnSearchClick(object sender, EventArgs e)
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
        private void BtnClearSearchClick(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            UpdateLibraryUI(library.GetAllTracks());
        }
        private void BtnCreatePlaylistClick(object sender, EventArgs e)
        {
            string name = txtPlaylistName.Text;
            if (string.IsNullOrEmpty(name) == false)
            {
                playlistService.CreatePlaylist(name);
                UpdatePlaylistsUI();
                txtPlaylistName.Text = "";
            }
        }
        private void BtnDeletePlaylistClick(object sender, EventArgs e)
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
        private void LstPlaylistsSelectedIndexChanged(object sender, EventArgs e)
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
        private void BtnAddTrackToPClick(object sender, EventArgs e)
        {
            int trackIndex = lstLibrary.SelectedIndex;
            int playlistIndex = lstPlaylists.SelectedIndex;
            if (trackIndex >= 0 && playlistIndex >= 0)
            {
                Track t = activeLibraryList[trackIndex];
                List<Playlist> allP = playlistService.GetPlaylists();
                Playlist p = allP[playlistIndex];
                playlistService.AddTrackToPlaylist(p.Id, t);
                LstPlaylistsSelectedIndexChanged(null, null);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите трек слева и плейлист справа!", "Подсказка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void BtnRemoveTrackFromPClick(object sender, EventArgs e)
        {
            int playlistIndex = lstPlaylists.SelectedIndex;
            int trackIndex = lstPlaylistTracks.SelectedIndex;
            if (playlistIndex >= 0 && trackIndex >= 0)
            {
                List<Playlist> allP = playlistService.GetPlaylists();
                Playlist p = allP[playlistIndex];
                Track t = p.Tracks[trackIndex];
                playlistService.RemoveTrackFromPlaylist(p.Id, t.Id);
                LstPlaylistsSelectedIndexChanged(null, null);
            }
        }
        private void BtnPlayClick(object sender, EventArgs e)
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
        private void BtnPauseClick(object sender, EventArgs e)
        {
            player.Pause();
        }
        private void BtnStopClick(object sender, EventArgs e)
        {
            player.Stop();
            playbackTimer.Stop();
            trackProgress.Value = 0;
            lblTime.Text = "00:00 / 00:00";
        }
        private void BtnNextClick(object sender, EventArgs e)
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
        private void BtnPrevClick(object sender, EventArgs e)
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
        private void TrackVolumeScroll(object sender, EventArgs e)
        {
            player.SetVolume(trackVolume.Value);
        }
        private void TrackProgressScroll(object sender, EventArgs e)
        {
            double total = player.GetTotalDurationSeconds();
            if (total > 0)
            {
                player.SetPositionSeconds(trackProgress.Value);
            }
        }
        private void ChkLoopCheckedChanged(object sender, EventArgs e)
        {
            player.LoopTrack = chkLoop.Checked;
        }
        private void ChkShuffleCheckedChanged(object sender, EventArgs e)
        {
            player.Shuffle = chkShuffle.Checked;
        }
        private void PlaybackTimerTick(object sender, EventArgs e)
        {
            double current = player.GetCurrentPositionSeconds();
            double total = player.GetTotalDurationSeconds();
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
                    BtnNextClick(null, null);
                }
            }
        }
    }
}