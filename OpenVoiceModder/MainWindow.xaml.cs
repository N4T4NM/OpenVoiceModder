using NAudio.CoreAudioApi;
using OpenVoiceModder.Sound;
using OpenVoiceModder.UI;
using OpenVoiceModder.UI.Dialogs;
using OpenVoiceModder.Utils;
using System;
using System.Windows;

namespace OpenVoiceModder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }
        public MainWindow()
        {
            Instance = this;

            InitializeComponent();
            MMDeviceEnumerator en = new();

            GlobalSound.Instance.Initialize(Database.Instance.InputDevice, Database.Instance.OutputDevice);
            LoadSettings();
        }

        void LoadSettings()
        {
            PART_Effects.IsMuted = Database.Instance.VoiceMuted;
            PART_Effects.ApplyEffects = Database.Instance.VoiceChangerUseEffects;

            PART_Soundboard.PART_HoldKey.Shortcut = Database.Instance.SoundboardHoldKey;

            MMDeviceEnumerator enumerator = new();
            
            foreach(MMDevice input in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                PART_InputDevice.AddItem(input.FriendlyName, input);
                if (input.ID == Database.Instance.InputDevice.ID)
                    PART_InputDevice.PART_SelectionText.Text = input.FriendlyName ;
            }

            foreach (MMDevice output in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                PART_OutputDevice.AddItem(output.FriendlyName, output);
                if (output.ID == Database.Instance.OutputDevice.ID)
                    PART_OutputDevice.PART_SelectionText.Text = output.FriendlyName;
            }

            PART_InputDevice.ItemSelected += OnInputSet;
            PART_OutputDevice.ItemSelected += OnOutputSet;

            Database.Instance.LoadSounds(PART_Soundboard);
            Database.Instance.LoadEffects(PART_Effects);

            Database.Instance.Ready = true;
            Keyboard.HookKeyboard();
        }

        private void OnInputSet(object? item)
        {
            MMDevice input = (MMDevice)item!;

            Database.Instance.InputDevice = input;
            GlobalSound.Instance.SetRecordingDevice(input);
        }

        private void OnOutputSet(object? item)
        {
            MMDevice output = (MMDevice)item!;

            Database.Instance.OutputDevice = output;
            GlobalSound.Instance.SetPlayerDevice(output);
        }

        protected override void OnClosed(EventArgs e)
        {
            Keyboard.RemoveHook();
            AudioPlayer.Instance.Destroy();
            base.OnClosed(e);
        }

        private void PART_ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            bool toggled = PART_Menu.Height > 100;
            if(toggled)
            {
                PART_Menu.Height = 12;
                PART_ToggleMenu.Content = "\uf106";
                return;
            }

            PART_Menu.Height = 500;
            PART_ToggleMenu.Content = "\uf107";
        }
    }
}
