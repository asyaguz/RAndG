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
        private readonly MusicLibrary musicLibrary = new();
        private readonly PlaylistPlayer playlistPlayer = new();
        private readonly PlaylistService playlistService = new();
        private readonly Color backgroundColor = Color.FromArgb(220, 241, 241);
        private readonly Color panelColor = Color.FromArgb(231, 245, 244);
        private readonly Color brownColor = Color.FromArgb(119, 79, 66);
        private readonly Color textColor = Color.FromArgb(92, 65, 57);
        private readonly Color selectedColor = Color.FromArgb(205, 229, 230);
        private readonly DataGridView tracksGrid = new();
        private readonly TextBox searchBox = new();
        private readonly ComboBox sortBox = new();
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
        private List<Track> allTracks = new();
        private List<Playlist> allPlaylists = new();
        private Track? currentTrack;
        private Playlist? selectedPlaylist;
        private bool updatingProgress;
        private bool isClosing;
        public MainForm()
        {
            InitializeComponent();
            BuildInterface();
            LoadTracks();
            LoadPlaylists();
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
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 145));
            Controls.Add(layout);
            layout.Controls.Add(BuildSidebar(), 0, 0);
            layout.Controls.Add(BuildLibraryPanel(), 1, 0);
            layout.Controls.Add(BuildPlaylistPanel(), 2, 0);
            Panel playerPanel = BuildPlayerPanel();
            layout.Controls.Add(playerPanel, 0, 1);
            layout.SetColumnSpan(playerPanel, 3);
        }
        private Panel BuildSidebar()
        {
            Panel panel = new() { Dock = DockStyle.Fill, BackColor = backgroundColor, Padding = new Padding(8) };
            TableLayoutPanel content = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5 };
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 95));
            Label logo = CreateLabel("♥  RAndG", 21, brownColor, true);
            logo.Dock = DockStyle.Fill;
            Button libraryButton = CreateButton("♫  Библиотека");
            libraryButton.Click += (_, _) =>
            {
                searchBox.Clear();
                sortBox.SelectedIndex = 0;
                RefreshTracks();
            };
            Label heading = CreateLabel("Мои плейлисты", 12, textColor, true);
            heading.Dock = DockStyle.Fill;
            playlistsList.Dock = DockStyle.Fill;
            playlistsList.BorderStyle = BorderStyle.None;
            playlistsList.BackColor = backgroundColor;
            playlistsList.ForeColor = textColor;
            playlistsList.Font = new Font("Segoe UI", 11F);
            playlistsList.IntegralHeight = false;
            playlistsList.DisplayMember = nameof(Playlist.Name);
            playlistsList.SelectedIndexChanged += (_, _) =>
            {
                selectedPlaylist = playlistsList.SelectedItem as Playlist;
                RefreshPlaylistTracks();
            };
            FlowLayoutPanel buttons = new()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            Button createButton = CreateButton("+ Создать плейлист");
            createButton.Width = 185;
            createButton.Height = 38;
            createButton.Click += (_, _) => CreatePlaylist();
            Button deleteButton = CreateButton("Удалить плейлист");
            deleteButton.Width = 185;
            deleteButton.Height = 38;
            deleteButton.Click += (_, _) => DeletePlaylist();
            buttons.Controls.Add(createButton);
            buttons.Controls.Add(deleteButton);
            content.Controls.Add(logo, 0, 0);
            content.Controls.Add(libraryButton, 0, 1);
            content.Controls.Add(heading, 0, 2);
            content.Controls.Add(playlistsList, 0, 3);
            content.Controls.Add(buttons, 0, 4);
            panel.Controls.Add(content);
            return panel;
        }
        private Panel BuildLibraryPanel()
        {
            Panel panel = new() { Dock = DockStyle.Fill, BackColor = panelColor, Padding = new Padding(16) };
            TableLayoutPanel content = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Label heading = CreateLabel("Вся библиотека", 19, textColor, true);
            heading.Dock = DockStyle.Fill;
            searchBox.Dock = DockStyle.Fill;
            searchBox.Font = new Font("Segoe UI", 11F);
            searchBox.PlaceholderText = "Поиск по названию или исполнителю...";
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            searchBox.TextChanged += (_, _) => RefreshTracks();
            FlowLayoutPanel options = new()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 3, 0, 3)
            };
            sortBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sortBox.Width = 175;
            sortBox.Items.AddRange(new object[] { "Название", "Исполнитель", "Путь к файлу", "Сначала новые" });
            sortBox.SelectedIndex = 0;
            sortBox.SelectedIndexChanged += (_, _) => RefreshTracks();
            Button clearButton = CreateButton("Очистить поиск");
            clearButton.Width = 135;
            clearButton.Click += (_, _) => searchBox.Clear();
            Button addButton = CreateButton("+ Добавить музыку");
            addButton.Width = 150;
            addButton.Click += (_, _) => AddMusicFiles();
            options.Controls.Add(sortBox);
            options.Controls.Add(clearButton);
            options.Controls.Add(addButton);
            ConfigureTracksGrid();
            content.Controls.Add(heading, 0, 0);
            content.Controls.Add(searchBox, 0, 1);
            content.Controls.Add(options, 0, 2);
            content.Controls.Add(tracksGrid, 0, 3);
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
            tracksGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tracksGrid.RowHeadersVisible = false;
            tracksGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tracksGrid.EnableHeadersVisualStyles = false;
            tracksGrid.ColumnHeadersDefaultCellStyle.BackColor = brownColor;
            tracksGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            tracksGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
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
                    PlaySelectedTrack();
            };
            tracksGrid.SelectionChanged += (_, _) =>
            {
                if (tracksGrid.CurrentRow?.DataBoundItem is Track track)
                    statusLabel.Text = "Выбран трек: " + track.Title;
            };
        }
        private Panel BuildPlaylistPanel()
        {
            Panel panel = new() { Dock = DockStyle.Fill, BackColor = backgroundColor, Padding = new Padding(12) };
            TableLayoutPanel content = new() { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5 };
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            content.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            Label heading = CreateLabel("Текущий плейлист", 16, brownColor, true);
            heading.Dock = DockStyle.Fill;
            Label hint = CreateLabel("Двойной щелчок — играть", 9, textColor, false);
            hint.Dock = DockStyle.Fill;
            playlistTracksList.Dock = DockStyle.Fill;
            playlistTracksList.BorderStyle = BorderStyle.None;
            playlistTracksList.BackColor = panelColor;
            playlistTracksList.ForeColor = textColor;
            playlistTracksList.Font = new Font("Segoe UI", 10F);
            playlistTracksList.DisplayMember = nameof(Track.Title);
            playlistTracksList.DoubleClick += (_, _) =>
            {
                if (playlistTracksList.SelectedItem is Track track)
                    PlayTrack(track);
            };
            Button addButton = CreateButton("+ Добавить выбранный трек");
            addButton.Click += (_, _) => AddTrackToPlaylist();
            Button removeButton = CreateButton("Убрать выбранный трек");
            removeButton.Click += (_, _) => RemoveTrackFromPlaylist();
            content.Controls.Add(heading, 0, 0);
            content.Controls.Add(hint, 0, 1);
            content.Controls.Add(playlistTracksList, 0, 2);
            content.Controls.Add(addButton, 0, 3);
            content.Controls.Add(removeButton, 0, 4);
            panel.Controls.Add(content);
            return panel;
        }
        private Panel BuildPlayerPanel()
        {
            Panel panel = new() { Dock = DockStyle.Fill, BackColor = brownColor, Padding = new Padding(12) };
            TableLayoutPanel content = new() { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3, BackColor = brownColor };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            content.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            currentTitle.Text = "Ничего не воспроизводится";
            currentTitle.ForeColor = Color.White;
            currentTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            currentTitle.Dock = DockStyle.Fill;
            currentTitle.AutoEllipsis = true;
            currentArtist.Text = "Выберите трек";
            currentArtist.ForeColor = Color.WhiteSmoke;
            currentArtist.Dock = DockStyle.Fill;
            currentArtist.AutoEllipsis = true;
            FlowLayoutPanel controls = new()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = brownColor
            };
            Button previous = CreatePlayerButton("|◀");
            previous.Click += (_, _) => PlayAdjacentTrack(-1);
            Button play = CreatePlayerButton("▶");
            play.Click += (_, _) => PlayOrResume();
            Button pause = CreatePlayerButton("Ⅱ");
            pause.Click += (_, _) => PausePlayback();
            Button stop = CreatePlayerButton("■");
            stop.Click += (_, _) => StopPlayback();
            Button next = CreatePlayerButton("▶|");
            next.Click += (_, _) => PlayAdjacentTrack(1);
            controls.Controls.Add(previous);
            controls.Controls.Add(play);
            controls.Controls.Add(pause);
            controls.Controls.Add(stop);
            controls.Controls.Add(next);
            FlowLayoutPanel volumePanel = new()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = brownColor,
                Padding = new Padding(0, 2, 0, 0)
            };
            Label volumeLabel = new()
            {
                Text = "Громкость",
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(0, 7, 4, 0)
            };
            volumeBar.Minimum = 0;
            volumeBar.Maximum = 100;
            volumeBar.Value = 70;
            volumeBar.TickStyle = TickStyle.None;
            volumeBar.Width = 120;
            volumeBar.Scroll += (_, _) => playlistPlayer.SetVolume(volumeBar.Value / 100f);
            playlistPlayer.SetVolume(0.7f);
            volumePanel.Controls.Add(volumeLabel);
            volumePanel.Controls.Add(volumeBar);
            elapsedLabel.Text = "0:00";
            elapsedLabel.ForeColor = Color.White;
            elapsedLabel.Dock = DockStyle.Fill;
            elapsedLabel.TextAlign = ContentAlignment.MiddleCenter;
            durationLabel.Text = "0:00";
            durationLabel.ForeColor = Color.White;
            durationLabel.Dock = DockStyle.Fill;
            durationLabel.TextAlign = ContentAlignment.MiddleCenter;
            progressBar.Minimum = 0;
            progressBar.Maximum = 1000;
            progressBar.TickStyle = TickStyle.None;
            progressBar.Dock = DockStyle.Fill;
            progressBar.Scroll += (_, _) =>
            {
                if (updatingProgress || playlistPlayer.TotalTime == TimeSpan.Zero)
                    return;
                double fraction = progressBar.Value / (double)progressBar.Maximum;
                playlistPlayer.Seek(TimeSpan.FromMilliseconds(playlistPlayer.TotalTime.TotalMilliseconds * fraction));
            };
            statusLabel.Text = "Готов к воспроизведению";
            statusLabel.ForeColor = Color.WhiteSmoke;
            statusLabel.Dock = DockStyle.Fill;
            statusLabel.AutoEllipsis = true;
            content.Controls.Add(currentTitle, 0, 0);
            content.Controls.Add(controls, 1, 0);
            content.Controls.Add(volumePanel, 2, 0);
            content.Controls.Add(currentArtist, 0, 1);
            content.SetColumnSpan(currentArtist, 3);
            content.Controls.Add(elapsedLabel, 0, 2);
            content.Controls.Add(progressBar, 1, 2);
            content.Controls.Add(durationLabel, 2, 2);
            panel.Controls.Add(content);
            statusLabel.Visible = true;
            statusLabel.Location = new Point(8, 2);
            statusLabel.Size = new Size(1, 1);
            panel.Controls.Add(statusLabel);
            return panel;
        }
        private Label CreateLabel(string text, float size, Color color, bool bold)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
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
                Font = new Font("Segoe UI", 9F),
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
                Margin = new Padding(2)
            };
        }
        private void LoadTracks()
        {
            try
            {
                allTracks = musicLibrary.GetTracks();
                RefreshTracks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить библиотеку.\n\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RefreshTracks()
        {
            if (IsDisposed)
                return;
            string sortBy = sortBox.SelectedItem?.ToString() ?? "Название";
            allTracks = musicLibrary.SearchTracks(searchBox.Text.Trim(), sortBy);
            tracksGrid.DataSource = null;
            tracksGrid.DataSource = allTracks;
            statusLabel.Text = $"Найдено треков: {allTracks.Count}";
        }
        private void AddMusicFiles()
        {
            using OpenFileDialog dialog = new()
            {
                Title = "Выберите музыкальные файлы",
                Filter = "Аудиофайлы|*.mp3;*.wav;*.aiff;*.aif;*.wma|Все файлы|*.*",
                Multiselect = true
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            int added = 0;
            int skipped = 0;
            foreach (string path in dialog.FileNames)
            {
                Track track = new()
                {
                    FilePath = path,
                    Title = Path.GetFileNameWithoutExtension(path),
                    Artist = ""
                };
                if (musicLibrary.AddTrack(track))
                    added++;
                else
                    skipped++;
            }
            RefreshTracks();
            MessageBox.Show($"Добавлено: {added}\nПропущено (уже есть в библиотеке): {skipped}", "Импорт музыки", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void LoadPlaylists(int? selectId = null)
        {
            allPlaylists = playlistService.GetPlaylists();
            playlistsList.DataSource = null;
            playlistsList.DisplayMember = nameof(Playlist.Name);
            playlistsList.DataSource = allPlaylists;
            if (allPlaylists.Count == 0)
            {
                selectedPlaylist = null;
                RefreshPlaylistTracks();
                return;
            }
            Playlist? target = selectId.HasValue
                ? allPlaylists.FirstOrDefault(p => p.Id == selectId.Value)
                : selectedPlaylist == null
                    ? null
                    : allPlaylists.FirstOrDefault(p => p.Id == selectedPlaylist.Id);
            playlistsList.SelectedItem = target ?? allPlaylists[0];
        }
        private void RefreshPlaylistTracks()
        {
            if (playlistsList.SelectedItem is Playlist playlist)
            {
                selectedPlaylist = playlist;
                playlistTracksList.DataSource = null;
                playlistTracksList.DisplayMember = nameof(Track.Title);
                playlistTracksList.DataSource = playlist.Tracks.ToList();
            }
            else
            {
                selectedPlaylist = null;
                playlistTracksList.DataSource = null;
            }
        }
        private void CreatePlaylist()
        {
            string? name = ShowTextInput("Введите название нового плейлиста:", "Создание плейлиста");
            if (name == null)
                return;
            if (!playlistService.CreatePlaylist(name))
            {
                MessageBox.Show("Плейлист с таким названием уже существует или название пустое.", "Не удалось создать плейлист", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            LoadPlaylists();
            statusLabel.Text = "Плейлист создан";
        }
        private void DeletePlaylist()
        {
            if (selectedPlaylist == null)
            {
                MessageBox.Show("Сначала выберите плейлист.", "Плейлисты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult result = MessageBox.Show($"Удалить плейлист «{selectedPlaylist.Name}»?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            int deletedId = selectedPlaylist.Id;
            playlistService.DeletePlaylist(deletedId);
            selectedPlaylist = null;
            LoadPlaylists();
            statusLabel.Text = "Плейлист удалён";
        }
        private void AddTrackToPlaylist()
        {
            Track? track = tracksGrid.CurrentRow?.DataBoundItem as Track;
            if (track == null)
            {
                MessageBox.Show("Сначала выберите трек в библиотеке.", "Плейлисты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (selectedPlaylist == null)
            {
                MessageBox.Show("Сначала создайте или выберите плейлист.", "Плейлисты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            bool added = playlistService.AddTrack(selectedPlaylist.Id, track.Id);
            LoadPlaylists(selectedPlaylist.Id);
            statusLabel.Text = added ? "Трек добавлен в плейлист" : "Этот трек уже есть в плейлисте";
        }
        private void RemoveTrackFromPlaylist()
        {
            if (selectedPlaylist == null || playlistTracksList.SelectedItem is not Track track)
            {
                statusLabel.Text = "Выберите трек в плейлисте";
                return;
            }
            playlistService.RemoveTrack(selectedPlaylist.Id, track.Id);
            LoadPlaylists(selectedPlaylist.Id);
            statusLabel.Text = "Трек удалён из плейлиста";
        }
        private static string? ShowTextInput(string prompt, string title)
        {
            using Form dialog = new()
            {
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(360, 130)
            };
            Label label = new() { Text = prompt, Left = 12, Top = 12, Width = 335 };
            TextBox input = new() { Left = 12, Top = 40, Width = 335 };
            Button ok = new() { Text = "Создать", Left = 168, Top = 82, Width = 85, DialogResult = DialogResult.OK };
            Button cancel = new() { Text = "Отмена", Left = 262, Top = 82, Width = 85, DialogResult = DialogResult.Cancel };
            dialog.Controls.Add(label);
            dialog.Controls.Add(input);
            dialog.Controls.Add(ok);
            dialog.Controls.Add(cancel);
            dialog.AcceptButton = ok;
            dialog.CancelButton = cancel;
            return dialog.ShowDialog() == DialogResult.OK ? input.Text.Trim() : null;
        }
        private void PlaySelectedTrack()
        {
            if (tracksGrid.CurrentRow?.DataBoundItem is Track track)
                PlayTrack(track);
        }
        private void PlayTrack(Track track)
        {
            if (!File.Exists(track.FilePath))
            {
                MessageBox.Show("Файл не найден:\n" + track.FilePath, "Ошибка воспроизведения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                playlistPlayer.Play(track);
                currentTrack = track;
                currentTitle.Text = track.Title;
                currentArtist.Text = string.IsNullOrWhiteSpace(track.Artist) ? "Неизвестный исполнитель" : track.Artist;
                statusLabel.Text = "Воспроизводится: " + track.Title;
                UpdateProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось воспроизвести трек.\n\n" + ex.Message, "Ошибка воспроизведения", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                return;
            if (currentTrack != null && File.Exists(currentTrack.FilePath) && playlistPlayer.TotalTime > TimeSpan.Zero && playlistPlayer.CurrentTime < playlistPlayer.TotalTime)
            {
                playlistPlayer.Resume();
                return;
            }
            PlaySelectedTrack();
        }
        private void PausePlayback()
        {
            if (playlistPlayer.IsPlaying)
            {
                playlistPlayer.Pause();
                statusLabel.Text = "Пауза";
            }
        }
        private void StopPlayback()
        {
            playlistPlayer.Stop();
            progressBar.Value = 0;
            elapsedLabel.Text = "0:00";
            statusLabel.Text = "Воспроизведение остановлено";
        }
        private void PlayAdjacentTrack(int direction)
        {
            if (allTracks.Count == 0)
                return;
            int index = currentTrack == null ? -1 : allTracks.FindIndex(t => t.Id == currentTrack.Id);
            int nextIndex = (index + direction + allTracks.Count) % allTracks.Count;
            PlayTrack(allTracks[nextIndex]);
        }
        private void PlaybackTimer_Tick(object? sender, EventArgs e)
        {
            if (!isClosing)
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
                double fraction = current.TotalMilliseconds / total.TotalMilliseconds;
                progressBar.Value = Math.Clamp((int)Math.Round(fraction * progressBar.Maximum), progressBar.Minimum, progressBar.Maximum);
            }
            else
            {
                progressBar.Value = 0;
            }
            updatingProgress = false;
        }
        private static string FormatTime(TimeSpan time)
        {
            return time.TotalHours >= 1 ? time.ToString(@"h\:mm\:ss") : time.ToString(@"m\:ss");
        }
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            isClosing = true;
            playbackTimer.Stop();
            playlistPlayer.Dispose();
        }
    }
}