using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RAndG.Models;
using RAndG.Data;
using RAndG.Services;
using RAndG.Player;

namespace RAndG
{
    // Главная форма приложения
    public partial class MainForm : Form
    {
        private Database db;
        private MusicLibrary library;
        private PlaylistService playlistService;
        private MusicPlayer player;
        private AudioFileValidator validator;
        private PerformanceMonitor monitor;
        private List<Track> activeLibraryList;
        private bool isDraggingProgress = false;
        private bool updatingSelection = false;
        // Создает главную форму, инициализирует необходимые объекты и настраивает внешний вид приложения
        public MainForm()
        {
            InitializeComponent();

            // При выборе песни в медиатеке
            // снимаем выделение с песен плейлиста
            lstLibrary.MouseDown += (s, e) =>
            {
                lstPlaylistTracks.ClearSelected();
            };

            // При выборе песни в плейлисте
            // снимаем выделение с медиатеки
            lstPlaylistTracks.MouseDown += (s, e) =>
            {
                lstLibrary.ClearSelected();
            };
            // Разрешаем выбирать несколько песен
            lstLibrary.SelectionMode = SelectionMode.MultiExtended;
            lstPlaylistTracks.SelectionMode = SelectionMode.MultiExtended;

            // Меняем стандартный цвет выделения
            lstLibrary.DrawMode = DrawMode.OwnerDrawFixed;
            lstPlaylistTracks.DrawMode = DrawMode.OwnerDrawFixed;
            lstPlaylists.DrawMode = DrawMode.OwnerDrawFixed;

            // Подключаем отрисовку списков
            lstLibrary.DrawItem += ListBoxDrawItem;
            lstPlaylistTracks.DrawItem += ListBoxDrawItem;
            lstPlaylists.DrawItem += ListBoxDrawItem;

            trackProgress.MouseDown += (s, e) =>
            {
                isDraggingProgress = true;
            };

            trackProgress.MouseUp += (s, e) =>
            {
                isDraggingProgress = false;
            };
            db = new Database();
            library = new MusicLibrary(db);
            playlistService = new PlaylistService(db, library.GetAllTracks());
            player = new MusicPlayer();
            validator = new AudioFileValidator();
            monitor = new PerformanceMonitor();
            UpdateLibraryUI(library.GetAllTracks());
            UpdatePlaylistsUI();
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
        private void ListBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            ListBox list = (ListBox)sender;

            Color backgroundColor = Color.White;
            Color textColor = Color.FromArgb(70, 68, 81);

            // Мятное выделение вместо стандартного синего
            if ((e.State & DrawItemState.Selected) ==
                DrawItemState.Selected)
            {
                backgroundColor = Color.FromArgb(204, 226, 214);
            }

            using (SolidBrush brush = new SolidBrush(backgroundColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            using (SolidBrush brush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(
                    list.Items[e.Index].ToString(),
                    e.Font,
                    brush,
                    e.Bounds.X + 5,
                    e.Bounds.Y + 2);
            }

            e.DrawFocusRectangle();
        }
        // Обновляет отображение музыкальной библиотеки. Добавляет названия всех переданных композиций в список
        private void UpdateLibraryUI(List<Track> tracks)
        {
            activeLibraryList = tracks;
            lstLibrary.Items.Clear();
            foreach (Track t in tracks)
            {
                lstLibrary.Items.Add(t.Artist + " - " + t.Title);
            }
        }
        // Обновляет отображение плейлистов на главной форме
        private void UpdatePlaylistsUI()
        {
            lstPlaylists.Items.Clear();
            List<Playlist> list = playlistService.GetPlaylists();
            foreach (Playlist p in list)
            {
                lstPlaylists.Items.Add(p.Name);
            }
        }
        // Обрабатывает нажатие кнопки добавления музыкального файла. Открывает окно выбора файла
        private void BtnAddTrackClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Музыкальные файлы|*.mp3;*.wav";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                if (validator.ValidateFile(path))
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path).Replace("_", " ").Trim();
                    string artist = "Неизвестный исполнитель";
                    string title = fileName;
                    string[] parts = fileName.Split(new string[] { " - " }, StringSplitOptions.None );
                    if (parts.Length >= 2)
                    {
                        artist = parts[0].Trim();
                        title = string.Join(" - ", parts.Skip(1)).Trim();
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
        // Обрабатывает удаление выбранной композиции из музыкальной библиотеки
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
        // Обрабатывает нажатие кнопки поиска композиций
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
        // Обрабатывает сброс поискового запроса
        private void BtnClearSearchClick(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            UpdateLibraryUI(library.GetAllTracks());
        }
        // Обрабатывает создание нового плейлиста
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
        // Обрабатывает удаление выбранного плейлиста
        private void BtnDeletePlaylistClick(object sender, EventArgs e)
        {
            int index = lstPlaylists.SelectedIndex;
            if (index >= 0)
            {
                List<Playlist> allP = playlistService.GetPlaylists();
                if (index < allP.Count)
                {
                    Playlist target = allP[index];
                    playlistService.DeletePlaylist(target.Id);
                    UpdatePlaylistsUI();
                    lstPlaylists.ClearSelected();
                    lstPlaylistTracks.Items.Clear();
                }
            }
        }
        // Обрабатывает изменение выбранного плейлиста. Отображает композиции, которые входят в него
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
                    lstPlaylistTracks.Items.Add(t.Artist + " - " + t.Title);
                }
                UpdatePlayingSelection();
            }
        }
        // Обрабатывает добавление выбранной композиции в выбранный плейлист
        private void BtnAddTrackToPClick(object sender, EventArgs e)
        {
            int playlistIndex = lstPlaylists.SelectedIndex;

            // Проверяем, выбран ли плейлист
            if (playlistIndex < 0)
            {
                MessageBox.Show(
                    "Сначала выберите плейлист!",
                    "Подсказка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            // Проверяем, выбраны ли песни
            if (lstLibrary.SelectedIndices.Count == 0)
            {
                MessageBox.Show(
                    "Выберите одну или несколько композиций!",
                    "Подсказка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            // Сохраняем номера выбранных песен
            List<int> selectedIndexes = new List<int>();

            foreach (int index in lstLibrary.SelectedIndices)
            {
                selectedIndexes.Add(index);
            }

            List<Playlist> allP = playlistService.GetPlaylists();

            if (playlistIndex >= allP.Count)
            {
                return;
            }

            Playlist selectedPlaylist = allP[playlistIndex];

            int addedCount = 0;

            // Добавляем все выбранные композиции
            foreach (int index in selectedIndexes)
            {
                if (index >= 0 && index < activeLibraryList.Count)
                {
                    Track t = activeLibraryList[index];

                    int oldCount = selectedPlaylist.Tracks.Count;

                    playlistService.AddTrackToPlaylist(
                        selectedPlaylist.Id, t);

                    playlistService.RefreshPlaylists();
                    allP = playlistService.GetPlaylists();
                    selectedPlaylist = allP[playlistIndex];

                    if (selectedPlaylist.Tracks.Count > oldCount)
                    {
                        addedCount++;
                    }
                }
            }

            // Обновляем отображение плейлиста
            LstPlaylistsSelectedIndexChanged(null, null);


            MessageBox.Show(
                "Добавлено композиций: " + addedCount,
                "Готово",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        // Обрабатывает удаление композиции из выбранного плейлиста
        // Обрабатывает удаление одной или нескольких композиций из выбранного плейлиста
        private void BtnRemoveTrackFromPClick(object sender, EventArgs e)
        {
            int playlistIndex = lstPlaylists.SelectedIndex;

            // Проверяем, выбран ли плейлист и выделены ли песни
            if (playlistIndex < 0 || lstPlaylistTracks.SelectedIndices.Count == 0)
            {
                MessageBox.Show(
                    "Выберите плейлист и одну или несколько композиций для удаления!",
                    "Подсказка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            List<Playlist> allP = playlistService.GetPlaylists();
            if (playlistIndex >= allP.Count)
            {
                return;
            }

            Playlist p = allP[playlistIndex];

            // Сначала собираем сами объекты песен, которые нужно удалить
            List<Track> tracksToRemove = new List<Track>();
            foreach (int trackIndex in lstPlaylistTracks.SelectedIndices)
            {
                if (trackIndex >= 0 && trackIndex < p.Tracks.Count)
                {
                    tracksToRemove.Add(p.Tracks[trackIndex]);
                }
            }

            // Удаляем каждую выбранную песню из плейлиста
            foreach (Track t in tracksToRemove)
            {
                playlistService.RemoveTrackFromPlaylist(p.Id, t.Id);
            }

            // Обновляем список треков на экране
            LstPlaylistsSelectedIndexChanged(null, null);
        }
        // Обрабатывает нажатие кнопки воспроизведения. Устанавливает очередь композиций и запускает выбранный аудиофайл
        private void BtnPlayClick(object sender, EventArgs e)
        {
            try
            {
                List<Track> tracksToPlay = new List<Track>();
                int selectedIndex = -1;

                // 1. Если выбрана песня в плейлисте
                if (lstPlaylistTracks.SelectedIndex >= 0 && lstPlaylists.SelectedIndex >= 0)
                {
                    List<Playlist> allP = playlistService.GetPlaylists();

                    if (lstPlaylists.SelectedIndex < allP.Count)
                    {
                        tracksToPlay = allP[lstPlaylists.SelectedIndex].Tracks;
                        selectedIndex = lstPlaylistTracks.SelectedIndex;
                    }
                }
                // 2. Если выбрана песня в медиатеке
                else if (lstLibrary.SelectedIndex >= 0)
                {
                    tracksToPlay = activeLibraryList;
                    selectedIndex = lstLibrary.SelectedIndex;
                }
                // 3. Если песня не выбрана, но выбран плейлист (для случайного порядка)
                else if (lstPlaylists.SelectedIndex >= 0)
                {
                    List<Playlist> allP = playlistService.GetPlaylists();

                    if (lstPlaylists.SelectedIndex < allP.Count)
                    {
                        tracksToPlay = allP[lstPlaylists.SelectedIndex].Tracks;
                    }
                }
                // 4. Иначе берем всю медиатеку (для случайного порядка)
                else
                {
                    tracksToPlay = activeLibraryList;
                }

                if (tracksToPlay.Count == 0)
                {
                    MessageBox.Show(
                        "Нет композиций для воспроизведения!",
                        "Подсказка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                // Если случайный порядок включен, выбираем случайную песню
                if (chkShuffle.Checked)
                {
                    Random rand = new Random();
                    selectedIndex = rand.Next(0, tracksToPlay.Count);
                }
                else if (selectedIndex < 0)
                {
                    MessageBox.Show(
                        "Выберите композицию или включите случайный порядок!",
                        "Подсказка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                Track target = tracksToPlay[selectedIndex];

                if (validator.ValidateFile(target.FilePath))
                {
                    List<string> paths = new List<string>();

                    foreach (Track t in tracksToPlay)
                    {
                        paths.Add(t.FilePath);
                    }

                    player.Shuffle = chkShuffle.Checked;
                    player.SetQueue(paths, selectedIndex);
                    player.Play();

                    lblCurrentTrack.Text = "Сейчас играет: " + target.Title;

                    playbackTimer.Start();

                    UpdatePlayingSelection();
                }
                else
                {
                    MessageBox.Show(
                        "Файл недоступен по указанному пути!",
                        "Ошибка воспроизведения",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка воспроизведения: " + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // Обрабатывает нажатие кнопки паузы и приостанавливает воспроизведение
        private void BtnPauseClick(object sender, EventArgs e)
        {
            player.Pause();
        }
        // Обрабатывает остановку воспроизведения. Останавливает таймер и сбрасывает индикатор времени
        private void BtnStopClick(object sender, EventArgs e)
        {
            player.Stop();
            playbackTimer.Stop();
            trackProgress.Value = 0;
            lblTime.Text = "00:00 / 00:00";
        }
        // Обрабатывает переход к следующей композиции
        private void BtnNextClick(object sender, EventArgs e)
        {
            player.Next();

            string currentPath = player.GetCurrentTrackPath();

            if (string.IsNullOrEmpty(currentPath) == false)
            {
                foreach (Track t in library.GetAllTracks())
                {
                    if (t.FilePath == currentPath)
                    {
                        lblCurrentTrack.Text = "Сейчас играет: " + t.Title;
                        break;
                    }
                }

                UpdatePlayingSelection();
                playbackTimer.Start();
            }
        }
        // Обрабатывает переход к предыдущей композиции
        private void BtnPrevClick(object sender, EventArgs e)
        {
            player.Previous();

            string currentPath = player.GetCurrentTrackPath();

            if (string.IsNullOrEmpty(currentPath) == false)
            {
                foreach (Track t in library.GetAllTracks())
                {
                    if (t.FilePath == currentPath)
                    {
                        lblCurrentTrack.Text = "Сейчас играет: " + t.Title;
                        break;
                    }
                }

                UpdatePlayingSelection();
                playbackTimer.Start();
            }
        }
        // Обрабатывает изменение положения регулятора громкости
        private void TrackVolumeScroll(object sender, EventArgs e)
        {
            player.SetVolume(trackVolume.Value);
        }
        // Обрабатывает перемещение ползунка позиции воспроизведения
        private void TrackProgressScroll(object sender, EventArgs e)
        {
            double total = player.GetTotalDurationSeconds();

            if (total > 0)
            {
                player.SetPositionSeconds(trackProgress.Value);
            }
        }

        // Обрабатывает изменение состояния переключателя повтора композиции
        private void ChkLoopCheckedChanged(object sender, EventArgs e)
        {
            player.LoopTrack = chkLoop.Checked;
        }
        // Обрабатывает изменение состояния режима случайного воспроизведения
        private void ChkShuffleCheckedChanged(object sender, EventArgs e)
        {
            player.Shuffle = chkShuffle.Checked;
        }
        // Обрабатывает срабатывания таймера воспроизведения
        private void PlaybackTimerTick(object sender, EventArgs e)
        {
            double current = player.GetCurrentPositionSeconds();
            double total = player.GetTotalDurationSeconds();

            if (total <= 0)
            {
                return;
            }
            // Обновляем время
            int curMin = (int)current / 60;
            int curSec = (int)current % 60;

            int totMin = (int)total / 60;
            int totSec = (int)total % 60;

            lblTime.Text = curMin.ToString("D2") + ":" +
                           curSec.ToString("D2") + " / " +
                           totMin.ToString("D2") + ":" +
                           totSec.ToString("D2");
            // Настраиваем ползунок
            int maxValue = (int)Math.Ceiling(total);

            if (maxValue < 1)
            {
                maxValue = 1;
            }

            trackProgress.Maximum = maxValue;
            // Обновляем ползунок, если пользователь его не перемещает
            if (isDraggingProgress == false)
            {
                int position = (int)current;

                if (position < 0)
                {
                    position = 0;
                }

                if (position > trackProgress.Maximum)
                {
                    position = trackProgress.Maximum;
                }

                trackProgress.Value = position;
            }
            // Проверяем окончание композиции
            if (player.IsTrackFinished())
            {
                if (chkLoop.Checked)
                {
                    // Повторяем текущую композицию
                    player.Play();
                }
                else
                {
                    // Переходим к следующей композиции
                    BtnNextClick(null, null);
                }
            }
        }
        private void UpdatePlayingSelection()
        {
            string currentPath = player.GetCurrentTrackPath();

            if (string.IsNullOrEmpty(currentPath))
            {
                return;
            }

            // Сначала проверяем, есть ли эта песня в текущем выбранном плейлисте
            int playlistTrackIndex = -1;

            if (lstPlaylists.SelectedIndex >= 0)
            {
                List<Playlist> allP = playlistService.GetPlaylists();

                if (lstPlaylists.SelectedIndex < allP.Count)
                {
                    List<Track> tracks = allP[lstPlaylists.SelectedIndex].Tracks;

                    for (int i = 0; i < tracks.Count; i++)
                    {
                        if (tracks[i].FilePath == currentPath)
                        {
                            playlistTrackIndex = i;
                            break;
                        }
                    }
                }
            }

            // Если песня найдена в открытом плейлисте — выделяем её только там
            if (playlistTrackIndex >= 0 && lstLibrary.SelectedIndex < 0)
            {
                lstPlaylistTracks.ClearSelected();
                lstPlaylistTracks.SelectedIndex = playlistTrackIndex;
                return;
            }

            // Иначе ищем и выделяем песню в медиатеке
            int libraryIndex = -1;

            for (int i = 0; i < activeLibraryList.Count; i++)
            {
                if (activeLibraryList[i].FilePath == currentPath)
                {
                    libraryIndex = i;
                    break;
                }
            }

            lstLibrary.ClearSelected();
            lstPlaylistTracks.ClearSelected();

            if (libraryIndex >= 0)
            {
                lstLibrary.SelectedIndex = libraryIndex;
            }
        }
    }
}