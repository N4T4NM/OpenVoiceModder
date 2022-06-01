using OpenVoiceModder.Sound;
using OpenVoiceModder.UI.Dialogs;
using OpenVoiceModder.Utils;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VideoLibrary;

namespace OpenVoiceModder.UI
{
    /// <summary>
    /// Interação lógica para SoundInfoControl.xam
    /// </summary>
    public partial class SoundInfoControl : UserControl
    {
        public Uri Uri { get; protected set; } = new(@"C:\");
        public Uri OnlineUri { get; protected set; } = new("https://youtube.com/");

        public bool IsReady { get; protected set; }
        public bool IsCurrentlyPlaying { get; protected set; }

        public Key Shortcut
        {
            get => PART_ShortcutEditor.Shortcut;
            set => PART_ShortcutEditor.Shortcut = value;
        }
        public Key HoldKey { get; protected set; }

        public delegate void RemovedEvent();
        public event RemovedEvent? Removed;

        public SoundInfoControl()
        {
            InitializeComponent();
            this.DataContext = this;
            Utils.Keyboard.GlobalKey += OnKeyReceived;
            PART_ShortcutEditor.EndEditing += OnEndEditing;
        }

        private void OnEndEditing(Key newShortcut) => Database.Instance.SaveSounds(MainWindow.Instance!.PART_Soundboard);
        private void OnKeyReceived(Key key, bool alternate)
        {
            if (key == this.Shortcut && !PART_ShortcutEditor.Editing)
            {
                if (IsCurrentlyPlaying)
                    GlobalSound.Instance.CurrentSoundboardSource?.Dispose();
                else StartPlaying(alternate);
            }
        }

        void GetSoundName()
        {
            PART_SoundName.Text = "Loading...";
            Task.Run(async () =>
            {
                YouTube yt = YouTube.Default;
                YouTubeVideo video = await yt.GetVideoAsync(Uri.AbsoluteUri);

                OnlineUri = new(video.Uri);
                Dispatcher.Invoke(() => PART_SoundName.Text = video.Title);
                IsReady = true;
            });
        }
        public void Set(Uri uri, Key shortcut)
        {
            IsReady = false;

            Uri = uri;
            Shortcut = shortcut;

            if (!Uri.IsFile)
            {
                GetSoundName();
            }
            else
            {
                PART_SoundName.Text = new FileInfo(Uri.LocalPath).Name;
                PART_SoundName.Text = PART_SoundName.Text.Remove(PART_SoundName.Text.LastIndexOf('.'));
                IsReady = true;
            }
            Database.Instance.SaveSounds(MainWindow.Instance!.PART_Soundboard);
        }

        private void PART_SoundName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DialogManager.ShowDialog<AddSoundDialog>((dialog) =>
            {
                dialog.SelectedSoundUri = Uri;
                dialog.SelectedShortcut = Shortcut;
            }, (dialog, result) =>
            {
                if (!result) return;

                if (IsCurrentlyPlaying)
                    GlobalSound.Instance.CurrentSoundboardSource!.Dispose();

                Set(dialog.SelectedSoundUri, dialog.SelectedShortcut);
            });
        }
        private void PART_RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.Keyboard.GlobalKey -= OnKeyReceived;

            if (IsCurrentlyPlaying)
                GlobalSound.Instance.CurrentSoundboardSource?.Dispose();

            Removed?.Invoke();
        }

        private void PART_StopSound_Click(object sender, RoutedEventArgs e)
        {
            if (IsCurrentlyPlaying)
                GlobalSound.Instance.CurrentSoundboardSource!.Dispose();
            else StartPlaying(false);
        }
        public void StartPlaying(bool altMode)
        {
            if (!IsReady) return;

            if (!Uri.IsFile)
            {
                IsReady = false;
                PrepareOnlinePlayer(altMode);
                return;
            }

            DoPlay(Uri, altMode);
        }

        void PrepareOnlinePlayer(bool altMode)
        {
            Task.Run(async () =>
            {
                try
                {
                    HttpClient client = new();
                    Stream test = await client.GetStreamAsync(OnlineUri);
                    test.Dispose();
                }
                catch (Exception ex)
                {
                    YouTube yt = YouTube.Default;
                    YouTubeVideo video = await yt.GetVideoAsync(Uri.AbsoluteUri);

                    OnlineUri = new(video.Uri);
                }

                IsReady = true;
                Dispatcher.Invoke(() => DoPlay(OnlineUri, altMode));
            });
        }
        void DoPlay(Uri uri, bool altMode)
        {
            IsCurrentlyPlaying = true;
            this.PART_StopSound.Content = "Stop";

            GlobalSound.Instance.PlaySoundFile(uri, altMode);
            GlobalSound.Instance.CurrentSoundboardSource!.AudioSourceRemoved += OnStop;

            HoldKey = MainWindow.Instance!.PART_Soundboard.PART_HoldKey.Shortcut;

            if (HoldKey != Key.None)
                Utils.Keyboard.HoldKey(HoldKey);
        }

        private void OnStop(AudioSource sender)
        {
            Dispatcher.Invoke(() =>
            {
                IsCurrentlyPlaying = false;
                sender.AudioSourceRemoved -= OnStop;
                this.PART_StopSound.Content = "Play";

                if (HoldKey != Key.None)
                    Utils.Keyboard.ReleaseKey(HoldKey);
            });
        }
    }
}
