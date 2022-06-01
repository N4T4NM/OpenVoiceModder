using Microsoft.Win32;
using OpenVoiceModder.Utils;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVoiceModder.UI.Dialogs
{
    /// <summary>
    /// Interação lógica para AddSoundDialog.xam
    /// </summary>
    public partial class AddSoundDialog : UserControl
    {
        public Uri SelectedSoundUri
        {
            get => new(PART_EditorText.Text);
            set => PART_EditorText.Text = value.IsFile ? value.LocalPath : value.AbsoluteUri;
        }
        public Key SelectedShortcut
        {
            get => PART_Shortcut.Shortcut;
            set => PART_Shortcut.Shortcut = value;
        }

        public AddSoundDialog()
        {
            InitializeComponent();

            PART_Shortcut.StartEditing += (old) =>
            {
                DialogManager.GetDialogWindow().PART_CloseButton.IsEnabled = false;
                PART_Save.IsEnabled = false;
            };
            PART_Shortcut.EndEditing += (@new) =>
            {
                DialogManager.GetDialogWindow().PART_CloseButton.IsEnabled = true;
                PART_Save.IsEnabled = true;
            };
        }

        private void PART_SelectButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            dialog.Multiselect = false;
            dialog.Filter = "Audio Files|*.mp3;*.mp4;*.wav";
            dialog.Title = "SoundBoard";

            if (dialog.ShowDialog() == false) return;
            SelectedSoundUri = new(dialog.FileName);
        }

        private void PART_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(SelectedSoundUri.IsFile)
                {
                    if (!File.Exists(SelectedSoundUri.LocalPath))
                    {
                        MessageBox.Show("Invalid path.");
                        return;
                    }
                } else
                {
                    string host = SelectedSoundUri.Host.Replace("www.", "").Replace(".com", "");
                    if(host.ToLower() != "youtube")
                    {
                        MessageBox.Show("Not a YouTube url!");
                        return;
                    }
                }

                DialogManager.GetDialogWindow().CloseDialog(true);
            } catch(Exception ex) 
            {
                MessageBox.Show("Invalid Input.");
                return;
            }
        }
    }
}
