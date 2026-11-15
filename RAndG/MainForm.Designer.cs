namespace RAndG
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private System.Windows.Forms.ListBox lstLibrary;
        private System.Windows.Forms.ListBox lstPlaylists;
        private System.Windows.Forms.ListBox lstPlaylistTracks;
        private System.Windows.Forms.Button btnAddTrack;
        private System.Windows.Forms.Button btnDeleteTrack;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.Button btnCreatePlaylist;
        private System.Windows.Forms.TextBox txtPlaylistName;
        private System.Windows.Forms.Button btnAddTrackToP;
        private System.Windows.Forms.Button btnRemoveTrackFromP;
        private System.Windows.Forms.Button btnDeletePlaylist;
        private System.Windows.Forms.TrackBar trackVolume;
        private System.Windows.Forms.TrackBar trackProgress;
        private System.Windows.Forms.Label lblCurrentTrack;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Timer playbackTimer;
        private System.Windows.Forms.CheckBox chkLoop;
        private System.Windows.Forms.CheckBox chkShuffle;
        private System.Windows.Forms.Label lblTitleLibrary;
        private System.Windows.Forms.Label lblTitlePlaylists;
        private System.Windows.Forms.Label lblTitleTracksInP;
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lstLibrary = new ListBox();
            lstPlaylists = new ListBox();
            lstPlaylistTracks = new ListBox();
            btnAddTrack = new Button();
            btnDeleteTrack = new Button();
            btnPlay = new Button();
            btnPause = new Button();
            btnStop = new Button();
            btnNext = new Button();
            btnPrev = new Button();
            txtSearch = new TextBox();
            btnSearch = new Button();
            btnClearSearch = new Button();
            btnCreatePlaylist = new Button();
            txtPlaylistName = new TextBox();
            btnAddTrackToP = new Button();
            btnRemoveTrackFromP = new Button();
            btnDeletePlaylist = new Button();
            trackVolume = new TrackBar();
            trackProgress = new TrackBar();
            lblCurrentTrack = new Label();
            lblTime = new Label();
            playbackTimer = new System.Windows.Forms.Timer(components);
            chkLoop = new CheckBox();
            chkShuffle = new CheckBox();
            lblTitleLibrary = new Label();
            lblTitlePlaylists = new Label();
            lblTitleTracksInP = new Label();
            ((System.ComponentModel.ISupportInitialize)trackVolume).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackProgress).BeginInit();
            SuspendLayout();
            // 
            // lstLibrary
            // 
            lstLibrary.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lstLibrary.BackColor = Color.White;
            lstLibrary.BorderStyle = BorderStyle.None;
            lstLibrary.Font = new Font("Segoe UI", 9.5F);
            lstLibrary.ForeColor = Color.FromArgb(70, 68, 81);
            lstLibrary.ItemHeight = 21;
            lstLibrary.Location = new Point(12, 80);
            lstLibrary.Name = "lstLibrary";
            lstLibrary.Size = new Size(250, 210);
            lstLibrary.TabIndex = 3;
            // 
            // lstPlaylists
            // 
            lstPlaylists.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lstPlaylists.BackColor = Color.White;
            lstPlaylists.BorderStyle = BorderStyle.None;
            lstPlaylists.Font = new Font("Segoe UI", 9.5F);
            lstPlaylists.ForeColor = Color.FromArgb(70, 68, 81);
            lstPlaylists.ItemHeight = 21;
            lstPlaylists.Location = new Point(280, 80);
            lstPlaylists.Name = "lstPlaylists";
            lstPlaylists.Size = new Size(180, 105);
            lstPlaylists.TabIndex = 4;
            lstPlaylists.SelectedIndexChanged += LstPlaylistsSelectedIndexChanged;
            // 
            // lstPlaylistTracks
            // 
            lstPlaylistTracks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstPlaylistTracks.BackColor = Color.White;
            lstPlaylistTracks.BorderStyle = BorderStyle.None;
            lstPlaylistTracks.Font = new Font("Segoe UI", 9.5F);
            lstPlaylistTracks.ForeColor = Color.FromArgb(70, 68, 81);
            lstPlaylistTracks.ItemHeight = 21;
            lstPlaylistTracks.Location = new Point(280, 250);
            lstPlaylistTracks.Name = "lstPlaylistTracks";
            lstPlaylistTracks.Size = new Size(180, 105);
            lstPlaylistTracks.TabIndex = 5;
            // 
            // btnAddTrack
            // 
            btnAddTrack.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddTrack.BackColor = Color.FromArgb(223, 219, 237);
            btnAddTrack.FlatAppearance.BorderSize = 0;
            btnAddTrack.FlatStyle = FlatStyle.Flat;
            btnAddTrack.Font = new Font("Segoe UI", 9.5F);
            btnAddTrack.ForeColor = Color.FromArgb(70, 68, 81);
            btnAddTrack.Location = new Point(12, 320);
            btnAddTrack.Name = "btnAddTrack";
            btnAddTrack.Size = new Size(120, 30);
            btnAddTrack.TabIndex = 6;
            btnAddTrack.Text = "Добавить файл";
            btnAddTrack.UseVisualStyleBackColor = false;
            btnAddTrack.Click += BtnAddTrackClick;
            // 
            // btnDeleteTrack
            // 
            btnDeleteTrack.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteTrack.BackColor = Color.FromArgb(223, 219, 237);
            btnDeleteTrack.FlatAppearance.BorderSize = 0;
            btnDeleteTrack.FlatStyle = FlatStyle.Flat;
            btnDeleteTrack.Font = new Font("Segoe UI", 9.5F);
            btnDeleteTrack.ForeColor = Color.FromArgb(70, 68, 81);
            btnDeleteTrack.Location = new Point(142, 320);
            btnDeleteTrack.Name = "btnDeleteTrack";
            btnDeleteTrack.Size = new Size(120, 30);
            btnDeleteTrack.TabIndex = 7;
            btnDeleteTrack.Text = "Удалить трек";
            btnDeleteTrack.UseVisualStyleBackColor = false;
            btnDeleteTrack.Click += BtnDeleteTrackClick;
            // 
            // btnPlay
            // 
            btnPlay.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnPlay.BackColor = Color.FromArgb(204, 226, 214);
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnPlay.ForeColor = Color.FromArgb(70, 68, 81);
            btnPlay.Location = new Point(60, 470);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(55, 32);
            btnPlay.TabIndex = 17;
            btnPlay.Text = "▶";
            btnPlay.UseVisualStyleBackColor = false;
            btnPlay.Click += BtnPlayClick;
            // 
            // btnPause
            // 
            btnPause.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnPause.BackColor = Color.FromArgb(223, 219, 237);
            btnPause.FlatAppearance.BorderSize = 0;
            btnPause.FlatStyle = FlatStyle.Flat;
            btnPause.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnPause.ForeColor = Color.FromArgb(70, 68, 81);
            btnPause.Location = new Point(121, 470);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(46, 32);
            btnPause.TabIndex = 18;
            btnPause.Text = "⏸";
            btnPause.UseVisualStyleBackColor = false;
            btnPause.Click += BtnPauseClick;
            // 
            // btnStop
            // 
            btnStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnStop.BackColor = Color.FromArgb(223, 219, 237);
            btnStop.FlatAppearance.BorderSize = 0;
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStop.ForeColor = Color.FromArgb(70, 68, 81);
            btnStop.Location = new Point(173, 470);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(46, 32);
            btnStop.TabIndex = 19;
            btnStop.Text = "⏹";
            btnStop.UseVisualStyleBackColor = false;
            btnStop.Click += BtnStopClick;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnNext.BackColor = Color.FromArgb(223, 219, 237);
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnNext.ForeColor = Color.FromArgb(70, 68, 81);
            btnNext.Location = new Point(225, 470);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(42, 32);
            btnNext.TabIndex = 20;
            btnNext.Text = "⏭";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += BtnNextClick;
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnPrev.BackColor = Color.FromArgb(223, 219, 237);
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnPrev.ForeColor = Color.FromArgb(70, 68, 81);
            btnPrev.Location = new Point(12, 470);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(42, 32);
            btnPrev.TabIndex = 16;
            btnPrev.Text = "⏮";
            btnPrev.UseVisualStyleBackColor = false;
            btnPrev.Click += BtnPrevClick;
            // 
            // txtSearch
            // 
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = new Font("Segoe UI", 9.5F);
            txtSearch.Location = new Point(12, 20);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(120, 29);
            txtSearch.TabIndex = 8;
            // 
            // btnSearch
            // 
            btnSearch.BackColor = Color.FromArgb(223, 219, 237);
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Font = new Font("Segoe UI", 9.5F);
            btnSearch.ForeColor = Color.FromArgb(70, 68, 81);
            btnSearch.Location = new Point(138, 19);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(58, 25);
            btnSearch.TabIndex = 9;
            btnSearch.Text = "Поиск";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += BtnSearchClick;
            // 
            // btnClearSearch
            // 
            btnClearSearch.BackColor = Color.FromArgb(223, 219, 237);
            btnClearSearch.FlatAppearance.BorderSize = 0;
            btnClearSearch.FlatStyle = FlatStyle.Flat;
            btnClearSearch.Font = new Font("Segoe UI", 9.5F);
            btnClearSearch.ForeColor = Color.FromArgb(70, 68, 81);
            btnClearSearch.Location = new Point(202, 19);
            btnClearSearch.Name = "btnClearSearch";
            btnClearSearch.Size = new Size(73, 25);
            btnClearSearch.TabIndex = 10;
            btnClearSearch.Text = "Сброс";
            btnClearSearch.UseVisualStyleBackColor = false;
            btnClearSearch.Click += BtnClearSearchClick;
            // 
            // btnCreatePlaylist
            // 
            btnCreatePlaylist.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCreatePlaylist.BackColor = Color.FromArgb(223, 219, 237);
            btnCreatePlaylist.FlatAppearance.BorderSize = 0;
            btnCreatePlaylist.FlatStyle = FlatStyle.Flat;
            btnCreatePlaylist.Font = new Font("Segoe UI", 9.5F);
            btnCreatePlaylist.ForeColor = Color.FromArgb(70, 68, 81);
            btnCreatePlaylist.Location = new Point(396, 19);
            btnCreatePlaylist.Name = "btnCreatePlaylist";
            btnCreatePlaylist.Size = new Size(64, 25);
            btnCreatePlaylist.TabIndex = 12;
            btnCreatePlaylist.Text = "Создать";
            btnCreatePlaylist.UseVisualStyleBackColor = false;
            btnCreatePlaylist.Click += BtnCreatePlaylistClick;
            // 
            // txtPlaylistName
            // 
            txtPlaylistName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPlaylistName.BorderStyle = BorderStyle.FixedSingle;
            txtPlaylistName.Font = new Font("Segoe UI", 9.5F);
            txtPlaylistName.Location = new Point(280, 20);
            txtPlaylistName.Name = "txtPlaylistName";
            txtPlaylistName.Size = new Size(110, 29);
            txtPlaylistName.TabIndex = 11;
            // 
            // btnAddTrackToP
            // 
            btnAddTrackToP.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddTrackToP.BackColor = Color.FromArgb(223, 219, 237);
            btnAddTrackToP.FlatAppearance.BorderSize = 0;
            btnAddTrackToP.FlatStyle = FlatStyle.Flat;
            btnAddTrackToP.Font = new Font("Segoe UI", 9.5F);
            btnAddTrackToP.ForeColor = Color.FromArgb(70, 68, 81);
            btnAddTrackToP.Location = new Point(12, 360);
            btnAddTrackToP.Name = "btnAddTrackToP";
            btnAddTrackToP.Size = new Size(250, 32);
            btnAddTrackToP.TabIndex = 14;
            btnAddTrackToP.Text = "Добавить в плейлист";
            btnAddTrackToP.UseVisualStyleBackColor = false;
            btnAddTrackToP.Click += BtnAddTrackToPClick;
            // 
            // btnRemoveTrackFromP
            // 
            btnRemoveTrackFromP.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnRemoveTrackFromP.BackColor = Color.FromArgb(223, 219, 237);
            btnRemoveTrackFromP.FlatAppearance.BorderSize = 0;
            btnRemoveTrackFromP.FlatStyle = FlatStyle.Flat;
            btnRemoveTrackFromP.Font = new Font("Segoe UI", 9.5F);
            btnRemoveTrackFromP.ForeColor = Color.FromArgb(70, 68, 81);
            btnRemoveTrackFromP.Location = new Point(280, 366);
            btnRemoveTrackFromP.Name = "btnRemoveTrackFromP";
            btnRemoveTrackFromP.Size = new Size(180, 26);
            btnRemoveTrackFromP.TabIndex = 15;
            btnRemoveTrackFromP.Text = "Исключить трек";
            btnRemoveTrackFromP.UseVisualStyleBackColor = false;
            btnRemoveTrackFromP.Click += BtnRemoveTrackFromPClick;
            // 
            // btnDeletePlaylist
            // 
            btnDeletePlaylist.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnDeletePlaylist.BackColor = Color.FromArgb(223, 219, 237);
            btnDeletePlaylist.FlatAppearance.BorderSize = 0;
            btnDeletePlaylist.FlatStyle = FlatStyle.Flat;
            btnDeletePlaylist.Font = new Font("Segoe UI", 9.5F);
            btnDeletePlaylist.ForeColor = Color.FromArgb(70, 68, 81);
            btnDeletePlaylist.Location = new Point(280, 195);
            btnDeletePlaylist.Name = "btnDeletePlaylist";
            btnDeletePlaylist.Size = new Size(180, 26);
            btnDeletePlaylist.TabIndex = 13;
            btnDeletePlaylist.Text = "🗑️ Удалить плейлист";
            btnDeletePlaylist.UseVisualStyleBackColor = false;
            btnDeletePlaylist.Click += BtnDeletePlaylistClick;
            // 
            // trackVolume
            // 
            trackVolume.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            trackVolume.BackColor = Color.FromArgb(242, 241, 246);
            trackVolume.Location = new Point(290, 470);
            trackVolume.Maximum = 100;
            trackVolume.Name = "trackVolume";
            trackVolume.Size = new Size(160, 56);
            trackVolume.TabIndex = 21;
            trackVolume.Value = 50;
            trackVolume.Scroll += TrackVolumeScroll;
            // 
            // trackProgress
            // 
            trackProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trackProgress.BackColor = Color.FromArgb(242, 241, 246);
            trackProgress.Location = new Point(12, 400);
            trackProgress.Name = "trackProgress";
            trackProgress.Size = new Size(448, 56);
            trackProgress.TabIndex = 22;
            trackProgress.TickStyle = TickStyle.None;
            trackProgress.Scroll += TrackProgressScroll;
            // 
            // lblCurrentTrack
            // 
            lblCurrentTrack.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblCurrentTrack.AutoSize = true;
            lblCurrentTrack.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCurrentTrack.ForeColor = Color.FromArgb(70, 68, 81);
            lblCurrentTrack.Location = new Point(12, 440);
            lblCurrentTrack.Name = "lblCurrentTrack";
            lblCurrentTrack.Size = new Size(145, 23);
            lblCurrentTrack.TabIndex = 23;
            lblCurrentTrack.Text = "Сейчас играет: -";
            // 
            // lblTime
            // 
            lblTime.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblTime.Font = new Font("Segoe UI", 9.5F);
            lblTime.ForeColor = Color.FromArgb(70, 68, 81);
            lblTime.Location = new Point(350, 440);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(110, 20);
            lblTime.TabIndex = 24;
            lblTime.Text = "00:00 / 00:00";
            lblTime.TextAlign = ContentAlignment.TopRight;
            // 
            // playbackTimer
            // 
            playbackTimer.Interval = 500;
            playbackTimer.Tick += PlaybackTimerTick;
            // 
            // chkLoop
            // 
            chkLoop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkLoop.Font = new Font("Segoe UI", 9.5F);
            chkLoop.ForeColor = Color.FromArgb(70, 68, 81);
            chkLoop.Location = new Point(15, 515);
            chkLoop.Name = "chkLoop";
            chkLoop.Size = new Size(120, 20);
            chkLoop.TabIndex = 25;
            chkLoop.Text = "Повтор трека";
            chkLoop.CheckedChanged += ChkLoopCheckedChanged;
            // 
            // chkShuffle
            // 
            chkShuffle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkShuffle.Font = new Font("Segoe UI", 9.5F);
            chkShuffle.ForeColor = Color.FromArgb(70, 68, 81);
            chkShuffle.Location = new Point(145, 515);
            chkShuffle.Name = "chkShuffle";
            chkShuffle.Size = new Size(150, 20);
            chkShuffle.TabIndex = 26;
            chkShuffle.Text = "Случайный порядок";
            chkShuffle.CheckedChanged += ChkShuffleCheckedChanged;
            // 
            // lblTitleLibrary
            // 
            lblTitleLibrary.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitleLibrary.ForeColor = Color.FromArgb(70, 68, 81);
            lblTitleLibrary.Location = new Point(12, 55);
            lblTitleLibrary.Name = "lblTitleLibrary";
            lblTitleLibrary.Size = new Size(250, 20);
            lblTitleLibrary.TabIndex = 0;
            lblTitleLibrary.Text = " Моя медиатека (А-Я)";
            // 
            // lblTitlePlaylists
            // 
            lblTitlePlaylists.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTitlePlaylists.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitlePlaylists.ForeColor = Color.FromArgb(70, 68, 81);
            lblTitlePlaylists.Location = new Point(280, 55);
            lblTitlePlaylists.Name = "lblTitlePlaylists";
            lblTitlePlaylists.Size = new Size(180, 20);
            lblTitlePlaylists.TabIndex = 1;
            lblTitlePlaylists.Text = " Плейлисты";
            // 
            // lblTitleTracksInP
            // 
            lblTitleTracksInP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblTitleTracksInP.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitleTracksInP.ForeColor = Color.FromArgb(70, 68, 81);
            lblTitleTracksInP.Location = new Point(280, 225);
            lblTitleTracksInP.Name = "lblTitleTracksInP";
            lblTitleTracksInP.Size = new Size(180, 29);
            lblTitleTracksInP.TabIndex = 2;
            lblTitleTracksInP.Text = " Треки в плейлисте";
            // 
            // MainForm
            // 
            BackColor = Color.FromArgb(242, 241, 246);
            ClientSize = new Size(480, 550);
            Controls.Add(lblTitleLibrary);
            Controls.Add(lblTitlePlaylists);
            Controls.Add(lblTitleTracksInP);
            Controls.Add(lstLibrary);
            Controls.Add(lstPlaylists);
            Controls.Add(lstPlaylistTracks);
            Controls.Add(btnAddTrack);
            Controls.Add(btnDeleteTrack);
            Controls.Add(txtSearch);
            Controls.Add(btnSearch);
            Controls.Add(btnClearSearch);
            Controls.Add(txtPlaylistName);
            Controls.Add(btnCreatePlaylist);
            Controls.Add(btnDeletePlaylist);
            Controls.Add(btnAddTrackToP);
            Controls.Add(btnRemoveTrackFromP);
            Controls.Add(btnPrev);
            Controls.Add(btnPlay);
            Controls.Add(btnPause);
            Controls.Add(btnStop);
            Controls.Add(btnNext);
            Controls.Add(trackVolume);
            Controls.Add(trackProgress);
            Controls.Add(lblCurrentTrack);
            Controls.Add(lblTime);
            Controls.Add(chkLoop);
            Controls.Add(chkShuffle);
            Name = "MainForm";
            Text = "R&G Музыкальный Плеер";
            ((System.ComponentModel.ISupportInitialize)trackVolume).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackProgress).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}