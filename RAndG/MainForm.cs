using System;

using System.Collections.Generic;

using System.Drawing;

using System.IO;

using System.Linq;

using System.Windows.Forms;

using RAndG.Models;

using RAndG.Players;

using RAndG.Services;

namespace RAndG

{

    public partial class MainForm : Form

    {

        private readonly MusicLibrary musicLibrary;

        private readonly PlaylistPlayer playlistPlayer;

        private readonly Color backgroundColor =

            Color.FromArgb(220, 241, 241);

        private readonly Color panelColor =

            Color.FromArgb(231, 245, 244);

        private readonly Color brownColor =

            Color.FromArgb(119, 79, 66);

        private readonly Color textColor =

            Color.FromArgb(92, 65, 57);

        private readonly Color selectedColor =

            Color.FromArgb(205, 229, 230);

        private readonly DataGridView tracksGrid = new();

        private readonly TextBox searchBox = new();

        private readonly Label currentTitle = new();

        private readonly Label currentArtist = new();

        private readonly Label statusLabel = new();

        private readonly Label elapsedLabel = new();

        private readonly Label durationLabel = new();

        private readonly TrackBar progressBar = new();

        private readonly TrackBar volumeBar = new();

        private readonly ListBox playlistsList = new();

        private readonly ListBox playlistTracksList = new();

        private readonly System.Windows.Forms.Timer playbackTimer = new();

        private readonly List<string> playlistNames = new()

        {

            "Favorites",

            "Chill vibes",

            "Workout",

            "Study",

            "New"

        };

        private readonly Dictionary<string, List<Track>> playlists = new();

        private List<Track> allTracks = new();

        private Track? currentTrack;

        private string selectedPlaylist = "Favorites";

        private bool updatingProgress;

        private bool isClosing;

        public MainForm()

        {

            InitializeComponent();

            musicLibrary = new MusicLibrary();

            playlistPlayer = new PlaylistPlayer();

            foreach (string name in playlistNames)

            {

                playlists[name] = new List<Track>();

            }

            BuildInterface();

            LoadTracks();

            playbackTimer.Interval = 500;

            playbackTimer.Tick += PlaybackTimer_Tick;

            playbackTimer.Start();

            FormClosing += MainForm_FormClosing;

        }

        private void BuildInterface()

        {

            Text = "RAndG — Music Player";

            StartPosition = FormStartPosition.CenterScreen;

            Size = new Size(1400, 850);

            MinimumSize = new Size(1000, 650);

            BackColor = backgroundColor;

            ForeColor = textColor;

            Font = new Font("Segoe UI", 10F);

            TableLayoutPanel layout = new()

            {

                Dock = DockStyle.Fill,

                ColumnCount = 3,

                RowCount = 2,

                Padding = new Padding(12),

                BackColor = backgroundColor

            };

            layout.ColumnStyles.Add(

                new ColumnStyle(SizeType.Absolute, 210));

            layout.ColumnStyles.Add(

                new ColumnStyle(SizeType.Percent, 100));

            layout.ColumnStyles.Add(

                new ColumnStyle(SizeType.Absolute, 280));

            layout.RowStyles.Add(

                new RowStyle(SizeType.Percent, 100));

            layout.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 125));

            Controls.Add(layout);

            Panel sidebar = BuildSidebar();

            Panel libraryPanel = BuildLibraryPanel();

            Panel playlistPanel = BuildPlaylistPanel();

            Panel playerPanel = BuildPlayerPanel();

            layout.Controls.Add(sidebar, 0, 0);

            layout.Controls.Add(libraryPanel, 1, 0);

            layout.Controls.Add(playlistPanel, 2, 0);

            layout.Controls.Add(playerPanel, 0, 1);

            layout.SetColumnSpan(playerPanel, 3);

        }

        private Panel BuildSidebar()

        {

            Panel panel = new()

            {

                Dock = DockStyle.Fill,

                BackColor = backgroundColor,

                Padding = new Padding(8)

            };

            TableLayoutPanel content = new()

            {

                Dock = DockStyle.Fill,

                ColumnCount = 1,

                RowCount = 5

            };

            content.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 65));

            content.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 50));

            content.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 45));

            content.RowStyles.Add(

                new RowStyle(SizeType.Percent, 100));

            content.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 50));

            Label logo = CreateLabel(

                "♥  RAndG",

                21,

                brownColor,

                true);

            logo.Dock = DockStyle.Fill;

            Button libraryButton = CreateButton("♫  Библиотека");

            libraryButton.Click += (_, _) =>

            {

                searchBox.Text = "";

                statusLabel.Text = "Вся музыкальная библиотека";

                tracksGrid.ClearSelection();

            };

            Label playlistsHeader = CreateLabel(

                "Мои плейлисты",

                12,

                textColor,

                true);

            playlistsHeader.Dock = DockStyle.Fill;

            playlistsList.Dock = DockStyle.Fill;

            playlistsList.BorderStyle = BorderStyle.None;

            playlistsList.BackColor = backgroundColor;

            playlistsList.ForeColor = textColor;

            playlistsList.Font = new Font("Segoe UI", 11F);

            playlistsList.IntegralHeight = false;

            playlistsList.Items.AddRange(playlistNames.Cast<object>().ToArray());

            playlistsList.SelectedItem = selectedPlaylist;

            playlistsList.SelectedIndexChanged += (_, _) =>

            {

                if (playlistsList.SelectedItem is string name)

                {

                    selectedPlaylist = name;

                    RefreshPlaylistView();

                    statusLabel.Text = "Плейлист: " + name;

                }

            };

            Button addToPlaylistButton =

                CreateButton("+  Добавить выбранный трек");

            addToPlaylistButton.Click += (_, _) => AddSelectedToPlaylist();

            content.Controls.Add(logo, 0, 0);

            content.Controls.Add(libraryButton, 0, 1);

            content.Controls.Add(playlistsHeader, 0, 2);

            content.Controls.Add(playlistsList, 0, 3);

            content.Controls.Add(addToPlaylistButton, 0, 4);

            panel.Controls.Add(content);

            return panel;

        }

        private Panel BuildLibraryPanel()

        {

            Panel panel = new()

            {

                Dock = DockStyle.Fill,

                BackColor = panelColor,

                Padding = new Padding(18)

            };

            TableLayoutPanel content = new()

            {

                Dock = DockStyle.Fill,

                ColumnCount = 1,

                RowCount = 3

            };

            content.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 58));

            content.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 55));

            content.RowStyles.Add(

                new RowStyle(SizeType.Percent, 100));

            FlowLayoutPanel searchPanel = new()

            {

                Dock = DockStyle.Fill,

                FlowDirection = FlowDirection.LeftToRight,

                WrapContents = false,

                BackColor = panelColor,

                Padding = new Padding(0, 4, 0, 4)

            };

            searchBox.Width = 330;

            searchBox.Height = 36;

            searchBox.Font = new Font("Segoe UI", 11F);

            searchBox.PlaceholderText = "Поиск треков, исполнителей...";

            searchBox.BorderStyle = BorderStyle.FixedSingle;

            searchBox.TextChanged += (_, _) => ApplySearch();

            Button addButton = CreateButton("+ Добавить музыку");

            addButton.Width = 155;

            addButton.Margin = new Padding(10, 0, 0, 0);

            addButton.Click += (_, _) => AddMusicFiles();

            searchPanel.Controls.Add(searchBox);

            searchPanel.Controls.Add(addButton);

            Label heading = CreateLabel(

                "Вся библиотека",

                19,

                textColor,

                true);

            heading.Dock = DockStyle.Fill;

            ConfigureTracksGrid();

            content.Controls.Add(searchPanel, 0, 0);

            content.Controls.Add(heading, 0, 1);

            content.Controls.Add(tracksGrid, 0, 2);

            panel.Controls.Add(content);

            return panel;

        }
        private void ConfigureTracksGrid()

        {

            tracksGrid.Dock = DockStyle.Fill;

            tracksGrid.BackgroundColor = panelColor;

            tracksGrid.BorderStyle = BorderStyle.None;

            tracksGrid.GridColor = selectedColor;

            tracksGrid.AutoGenerateColumns = false;

            tracksGrid.AllowUserToAddRows = false;

            tracksGrid.AllowUserToDeleteRows = false;

            tracksGrid.AllowUserToResizeRows = false;

            tracksGrid.ReadOnly = true;

            tracksGrid.MultiSelect = false;

            tracksGrid.SelectionMode =

                DataGridViewSelectionMode.FullRowSelect;

            tracksGrid.RowHeadersVisible = false;

            tracksGrid.AutoSizeColumnsMode =

                DataGridViewAutoSizeColumnsMode.Fill;

            tracksGrid.EnableHeadersVisualStyles = false;

            tracksGrid.ColumnHeadersDefaultCellStyle.BackColor = brownColor;

            tracksGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            tracksGrid.ColumnHeadersDefaultCellStyle.Font =

                new Font("Segoe UI", 10F, FontStyle.Bold);

            tracksGrid.DefaultCellStyle.BackColor = panelColor;

            tracksGrid.DefaultCellStyle.ForeColor = textColor;

            tracksGrid.DefaultCellStyle.SelectionBackColor = selectedColor;

            tracksGrid.DefaultCellStyle.SelectionForeColor = textColor;

            tracksGrid.DefaultCellStyle.Padding = new Padding(5);

            tracksGrid.RowTemplate.Height = 38;

            tracksGrid.Columns.Add(new DataGridViewTextBoxColumn

            {

                HeaderText = "Название трека",

                DataPropertyName = nameof(Track.Title),

                FillWeight = 60

            });

            tracksGrid.Columns.Add(new DataGridViewTextBoxColumn

            {

                HeaderText = "Исполнитель",

                DataPropertyName = nameof(Track.Artist),

                FillWeight = 40

            });

            tracksGrid.CellDoubleClick += (_, e) =>

            {

                if (e.RowIndex >= 0)

                {

                    PlaySelectedTrack();

                }

            };

            tracksGrid.SelectionChanged += (_, _) =>

            {

                if (tracksGrid.CurrentRow?.DataBoundItem is Track track)

                {

                    statusLabel.Text =

                        "Выбран трек: " + track.Title;

                }

            };

        }

        private Panel BuildPlaylistPanel()

        {

            Panel panel = new()

            {

                Dock = DockStyle.Fill,

                BackColor = backgroundColor,

                Padding = new Padding(12)

            };

            TableLayoutPanel content = new()

            {

                Dock = DockStyle.Fill,

                ColumnCount = 1,

                RowCount = 4

            };

            content.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 65));

            content.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 35));

            content.RowStyles.Add(

                new RowStyle(SizeType.Percent, 100));

            content.RowStyles.Add(

                new RowStyle(SizeType.Absolute, 45));

            Label heading = CreateLabel(

                "Текущий плейлист",

                17,

                brownColor,

                true);

            heading.Dock = DockStyle.Fill;

            Label playlistName = CreateLabel(

                selectedPlaylist,

                12,

                textColor,

                false);

            playlistName.Dock = DockStyle.Fill;

            playlistsList.SelectedIndexChanged += (_, _) =>

            {

                playlistName.Text = selectedPlaylist;

            };

            playlistTracksList.Dock = DockStyle.Fill;

            playlistTracksList.BorderStyle = BorderStyle.None;

            playlistTracksList.BackColor = panelColor;

            playlistTracksList.ForeColor = textColor;

            playlistTracksList.Font = new Font("Segoe UI", 10F);

            playlistTracksList.DoubleClick += (_, _) =>

            {

                if (playlistTracksList.SelectedItem is Track track)

                {

                    PlayTrack(track);

                }

            };

            Button removeButton = CreateButton("Убрать из плейлиста");

            removeButton.Click += (_, _) => RemoveFromPlaylist();

            content.Controls.Add(heading, 0, 0);

            content.Controls.Add(playlistName, 0, 1);

            content.Controls.Add(playlistTracksList, 0, 2);

            content.Controls.Add(removeButton, 0, 3);

            panel.Controls.Add(content);

            RefreshPlaylistView();

            return panel;

        }

        private Panel BuildPlayerPanel()

        {

            Panel panel = new()

            {

                Dock = DockStyle.Fill,

                BackColor = brownColor,

                Padding = new Padding(12)

            };

            TableLayoutPanel content = new()

            {

                Dock = DockStyle.Fill,

                ColumnCount = 3,

                RowCount = 2,

                BackColor = brownColor

            };

            content.ColumnStyles.Add(

                new ColumnStyle(SizeType.Absolute, 240));

            content.ColumnStyles.Add(

                new ColumnStyle(SizeType.Percent, 100));

            content.ColumnStyles.Add(

                new ColumnStyle(SizeType.Absolute, 220));

            content.RowStyles.Add(

                new RowStyle(SizeType.Percent, 55));

            content.RowStyles.Add(

                new RowStyle(SizeType.Percent, 45));

            TableLayoutPanel trackInfo = new()

            {

                Dock = DockStyle.Fill,

                ColumnCount = 1,

                RowCount = 2,

                BackColor = brownColor,

                Padding = new Padding(5)

            };

            currentTitle.Text = "Ничего не воспроизводится";

            currentTitle.ForeColor = Color.White;

            currentTitle.Font = new Font(

                "Segoe UI", 12F, FontStyle.Bold);

            currentTitle.Dock = DockStyle.Fill;

            currentTitle.AutoEllipsis = true;

            currentArtist.Text = "Выберите трек в библиотеке";

            currentArtist.ForeColor = Color.WhiteSmoke;

            currentArtist.Dock = DockStyle.Fill;

            currentArtist.AutoEllipsis = true;

            trackInfo.Controls.Add(currentTitle, 0, 0);

            trackInfo.Controls.Add(currentArtist, 0, 1);

            FlowLayoutPanel controls = new()

            {

                Dock = DockStyle.Fill,

                FlowDirection = FlowDirection.LeftToRight,

                WrapContents = false,

                BackColor = brownColor,

                Anchor = AnchorStyles.None

            };

            Button previousButton = CreatePlayerButton("|◀");

            previousButton.Click += (_, _) => PlayPrevious();

            Button playButton = CreatePlayerButton("▶");

            playButton.Click += (_, _) => PlayOrResume();

            Button pauseButton = CreatePlayerButton("Ⅱ");

            pauseButton.Click += (_, _) => PausePlayback();

            Button stopButton = CreatePlayerButton("■");

            stopButton.Click += (_, _) => StopPlayback();

            Button nextButton = CreatePlayerButton("▶|");

            nextButton.Click += (_, _) => PlayNext();

            controls.Controls.Add(previousButton);

            controls.Controls.Add(playButton);

            controls.Controls.Add(pauseButton);

            controls.Controls.Add(stopButton);

            controls.Controls.Add(nextButton);

            FlowLayoutPanel volumePanel = new()

            {

                Dock = DockStyle.Fill,

                FlowDirection = FlowDirection.LeftToRight,

                WrapContents = false,

                BackColor = brownColor,

                Padding = new Padding(4, 5, 0, 0)

            };

            Label volumeLabel = new()

            {

                Text = "Громкость",

                ForeColor = Color.White,

                AutoSize = true,

                Margin = new Padding(0, 7, 5, 0)

            };

            volumeBar.Minimum = 0;

            volumeBar.Maximum = 100;

            volumeBar.Value = 70;

            volumeBar.TickStyle = TickStyle.None;

            volumeBar.Width = 115;

            volumeBar.Height = 35;

            volumeBar.Scroll += (_, _) =>

                playlistPlayer.SetVolume(volumeBar.Value / 100f);

            playlistPlayer.SetVolume(0.7f);

            volumePanel.Controls.Add(volumeLabel);

            volumePanel.Controls.Add(volumeBar);

            TableLayoutPanel progressPanel = new()

            {

                Dock = DockStyle.Fill,

                ColumnCount = 3,

                RowCount = 1,

                BackColor = brownColor

            };

            progressPanel.ColumnStyles.Add(

                new ColumnStyle(SizeType.Absolute, 50));

            progressPanel.ColumnStyles.Add(

                new ColumnStyle(SizeType.Percent, 100));

            progressPanel.ColumnStyles.Add(

                new ColumnStyle(SizeType.Absolute, 50));

            elapsedLabel.Text = "0:00";

            elapsedLabel.ForeColor = Color.White;

            elapsedLabel.TextAlign = ContentAlignment.MiddleCenter;

            elapsedLabel.Dock = DockStyle.Fill;

            durationLabel.Text = "0:00";

            durationLabel.ForeColor = Color.White;

            durationLabel.TextAlign = ContentAlignment.MiddleCenter;

            durationLabel.Dock = DockStyle.Fill;

            progressBar.Minimum = 0;

            progressBar.Maximum = 1000;

            progressBar.Value = 0;

            progressBar.TickStyle = TickStyle.None;

            progressBar.Dock = DockStyle.Fill;

            progressBar.Scroll += (_, _) =>

            {

                if (updatingProgress ||

                    playlistPlayer.TotalTime == TimeSpan.Zero)

                {

                    return;

                }

                double fraction =

                    progressBar.Value / (double)progressBar.Maximum;

                playlistPlayer.Seek(

                    TimeSpan.FromMilliseconds(

                        playlistPlayer.TotalTime.TotalMilliseconds *

                        fraction));

            };

            progressPanel.Controls.Add(elapsedLabel, 0, 0);

            progressPanel.Controls.Add(progressBar, 1, 0);

            progressPanel.Controls.Add(durationLabel, 2, 0);

            content.Controls.Add(trackInfo, 0, 0);

            content.Controls.Add(controls, 1, 0);

            content.Controls.Add(volumePanel, 2, 0);

            content.Controls.Add(progressPanel, 0, 1);

            content.SetColumnSpan(progressPanel, 3);

            panel.Controls.Add(content);

            return panel;

        }

        private Label CreateLabel(

            string text,

            float fontSize,

            Color color,

            bool bold)

        {

            return new Label

            {

                Text = text,

                Font = new Font(

                    "Segoe UI",

                    fontSize,

                    bold ? FontStyle.Bold : FontStyle.Regular),

                ForeColor = color,

                TextAlign = ContentAlignment.MiddleLeft,

                AutoEllipsis = true

            };

        }

        private Button CreateButton(string text)

        {

            Button button = new()

            {

                Text = text,

                Height = 38,

                Dock = DockStyle.Fill,

                BackColor = brownColor,

                ForeColor = Color.White,

                FlatStyle = FlatStyle.Flat,

                Cursor = Cursors.Hand,

                Font = new Font("Segoe UI", 9.5F),

                Margin = new Padding(2)

            };

            button.FlatAppearance.BorderSize = 0;

            return button;

        }

        private Button CreatePlayerButton(string text)

        {

            return new Button

            {

                Text = text,

                Width = 38,

                Height = 38,

                BackColor = brownColor,

                ForeColor = Color.White,

                FlatStyle = FlatStyle.Flat,

                Font = new Font("Segoe UI", 11F, FontStyle.Bold),

                Cursor = Cursors.Hand,

                Margin = new Padding(2, 4, 2, 2)

            };

        }

        private void LoadTracks()

        {

            try

            {

                allTracks = musicLibrary.GetTracks();

                ApplySearch();

            }

            catch (Exception ex)

            {

                MessageBox.Show(

                    "Не удалось загрузить музыкальную библиотеку.\n\n" +

                    ex.Message,

                    "Ошибка",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);

            }

        }

        private void ApplySearch()

        {

            string query = searchBox.Text.Trim();

            List<Track> filteredTracks = allTracks

                .Where(track =>

                    track.Title.Contains(

                        query, StringComparison.OrdinalIgnoreCase) ||

                    track.Artist.Contains(

                        query, StringComparison.OrdinalIgnoreCase))

                .ToList();

            tracksGrid.DataSource = null;

            tracksGrid.DataSource = filteredTracks;

            statusLabel.Text =

                $"Треков в библиотеке: {filteredTracks.Count}";

        }

        private void AddMusicFiles()

        {

            using OpenFileDialog dialog = new()

            {

                Title = "Выберите музыкальные файлы",

                Filter =

                    "Аудиофайлы|*.mp3;*.wav;*.aiff;*.aif;*.wma|" +

                    "Все файлы|*.*",

                Multiselect = true

            };

            if (dialog.ShowDialog() != DialogResult.OK)

                return;

            int added = 0;

            int skipped = 0;

            foreach (string filePath in dialog.FileNames)

            {

                Track track = new()

                {

                    FilePath = filePath,

                    Title = Path.GetFileNameWithoutExtension(filePath),

                    Artist = ""

                };

                try

                {

                    if (musicLibrary.AddTrack(track))

                        added++;

                    else

                        skipped++;

                }

                catch (Exception ex)

                {

                    MessageBox.Show(

                        $"Не удалось добавить файл:\n{filePath}\n\n" +

                        ex.Message,

                        "Ошибка добавления",

                        MessageBoxButtons.OK,

                        MessageBoxIcon.Warning);

                }

            }

            LoadTracks();

            statusLabel.Text =

                $"Добавлено: {added}; пропущено: {skipped}";

        }

        private Track? GetSelectedTrack()

        {

            return tracksGrid.CurrentRow?.DataBoundItem as Track;

        }

        private void PlaySelectedTrack()

        {

            Track? track = GetSelectedTrack();

            if (track == null)

            {

                MessageBox.Show(

                    "Сначала выберите трек в библиотеке.",

                    "Музыкальный плеер",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Information);

                return;

            }

            PlayTrack(track);

        }

        private void PlayTrack(Track track)

        {

            if (!File.Exists(track.FilePath))

            {

                MessageBox.Show(

                    "Файл не найден:\n" + track.FilePath,

                    "Ошибка воспроизведения",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

                return;

            }

            try

            {

                playlistPlayer.Play(track);

                currentTrack = track;

                currentTitle.Text = track.Title;

                currentArtist.Text = string.IsNullOrWhiteSpace(track.Artist)

                    ? "Неизвестный исполнитель"

                    : track.Artist;

                statusLabel.Text = "Воспроизводится: " + track.Title;

                UpdateProgress();

            }

            catch (Exception ex)

            {

                MessageBox.Show(

                    "Не удалось воспроизвести трек.\n\n" + ex.Message,

                    "Ошибка воспроизведения",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);

            }

        }

        private void PlayOrResume()

        {

            if (playlistPlayer.IsPaused)

            {

                playlistPlayer.Resume();

                statusLabel.Text = "Воспроизведение продолжается";

                return;

            }

            if (playlistPlayer.IsPlaying)

            {

                statusLabel.Text = "Трек уже воспроизводится";

                return;

            }

            if (currentTrack != null &&

                File.Exists(currentTrack.FilePath) &&

                playlistPlayer.TotalTime > TimeSpan.Zero &&

                playlistPlayer.CurrentTime < playlistPlayer.TotalTime)

            {

                playlistPlayer.Resume();

                statusLabel.Text = "Воспроизведение продолжается";

                return;

            }

            PlaySelectedTrack();

        }

        private void PausePlayback()

        {

            if (playlistPlayer.IsPlaying)

            {

                playlistPlayer.Pause();

                statusLabel.Text = "Воспроизведение на паузе";

            }

            else

            {

                statusLabel.Text = "Нет воспроизводимого трека";

            }

        }

        private void StopPlayback()

        {

            playlistPlayer.Stop();

            progressBar.Value = 0;

            elapsedLabel.Text = "0:00";

            statusLabel.Text = "Воспроизведение остановлено";

        }

        private void PlayNext()

        {

            if (allTracks.Count == 0)

                return;

            int index = currentTrack == null

                ? -1

                : allTracks.FindIndex(t => t.Id == currentTrack.Id);

            int nextIndex = (index + 1) % allTracks.Count;

            PlayTrack(allTracks[nextIndex]);

        }

        private void PlayPrevious()

        {

            if (allTracks.Count == 0)

                return;

            int index = currentTrack == null

                ? 0

                : allTracks.FindIndex(t => t.Id == currentTrack.Id);

            int previousIndex = index <= 0

                ? allTracks.Count - 1

                : index - 1;

            PlayTrack(allTracks[previousIndex]);

        }

        private void AddSelectedToPlaylist()

        {

            Track? track = GetSelectedTrack();

            if (track == null)

            {

                MessageBox.Show(

                    "Выберите трек в библиотеке.",

                    "Плейлисты",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Information);

                return;

            }

            if (!playlists.TryGetValue(

                selectedPlaylist, out List<Track>? tracks))

            {

                return;

            }

            if (tracks.Any(t => t.Id == track.Id))

            {

                statusLabel.Text = "Этот трек уже есть в плейлисте";

                return;

            }

            tracks.Add(track);

            RefreshPlaylistView();

            statusLabel.Text =

                $"Трек добавлен в плейлист «{selectedPlaylist}»";

        }

        private void RemoveFromPlaylist()

        {

            if (playlistTracksList.SelectedItem is not Track track)

            {

                statusLabel.Text = "Выберите трек в плейлисте";

                return;

            }

            if (playlists.TryGetValue(

                selectedPlaylist, out List<Track>? tracks))

            {

                tracks.RemoveAll(t => t.Id == track.Id);

                RefreshPlaylistView();

                statusLabel.Text = "Трек удалён из плейлиста";

            }

        }

        private void RefreshPlaylistView()

        {

            playlistTracksList.DataSource = null;

            if (playlists.TryGetValue(

                selectedPlaylist, out List<Track>? tracks))

            {

                playlistTracksList.DataSource = tracks.ToList();

                playlistTracksList.DisplayMember = nameof(Track.Title);

            }

        }

        private void PlaybackTimer_Tick(object? sender, EventArgs e)

        {

            if (isClosing)

                return;

            UpdateProgress();

        }

        private void UpdateProgress()

        {

            TimeSpan current = playlistPlayer.CurrentTime;

            TimeSpan total = playlistPlayer.TotalTime;

            elapsedLabel.Text = FormatTime(current);

            durationLabel.Text = FormatTime(total);

            updatingProgress = true;

            if (total > TimeSpan.Zero)

            {

                double fraction =

                    current.TotalMilliseconds / total.TotalMilliseconds;

                int value = (int)Math.Round(

                    Math.Clamp(fraction, 0, 1) * progressBar.Maximum);

                progressBar.Value = Math.Clamp(

                    value,

                    progressBar.Minimum,

                    progressBar.Maximum);

            }

            else

            {

                progressBar.Value = 0;

            }

            updatingProgress = false;

        }

        private static string FormatTime(TimeSpan time)

        {

            return time.TotalHours >= 1

                ? time.ToString(@"h\:mm\:ss")

                : time.ToString(@"m\:ss");

        }

        private void MainForm_FormClosing(

            object? sender,

            FormClosingEventArgs e)

        {

            isClosing = true;

            playbackTimer.Stop();

            playlistPlayer.Dispose();

        }

    }

}
