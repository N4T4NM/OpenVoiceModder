using OpenVoiceModder.UI.Dialogs;
using OpenVoiceModder.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVoiceModder.UI
{
    /// <summary>
    /// Interação lógica para SoundBoard.xam
    /// </summary>
    public partial class SoundBoard : UserControl
    {
        public bool ApplyEffects
        {
            get => Database.Instance.SoundboardUseEffects;
            set => Database.Instance.SoundboardUseEffects = value;
        }

        public SoundBoard()
        {
            InitializeComponent();
            this.DataContext = this;

            PART_HoldKey.EndEditing += OnEndEditing;
        }

        private void OnEndEditing(Key newShortcut) => Database.Instance.SoundboardHoldKey = newShortcut;
        private void PART_AddSound_Click(object sender, RoutedEventArgs e)
        {
            //string[]? files = SelectFiles();
            //if (files == null) return;

            //foreach (string file in files)
            //    AddSound(new(file), Key.None);

            DialogManager.ShowDialog<AddSoundDialog>((dialog) => { },
            (dialog, result) =>
            {
                if (!result) return;
                AddSound(dialog.SelectedSoundUri, dialog.SelectedShortcut);
            });
        }
        public void AddSound(Uri uri, Key shortcut)
        {
            SoundInfoControl control = new();

            control.Removed += () =>
            {
                PART_Sounds.Children.Remove(control);
                Database.Instance.SaveSounds(this);
            };
            PART_Sounds.Children.Add(control);

            control.Set(uri, shortcut);
        }
    }
}
