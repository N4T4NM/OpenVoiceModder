using OpenVoiceModder.UI.Dialogs;
using System;
using System.Windows;

namespace OpenVoiceModder.Utils
{
    public static class DialogManager
    {
        public delegate void OnDialogResult<T>(T dialog, bool result) where T : UIElement;
        public delegate void OnDialogCreated<T>(T dialog) where T : UIElement;

        public static void ShowDialog<T>(OnDialogCreated<T> onCreated, OnDialogResult<T> onResult) where T : UIElement
        {
            if (MainWindow.Instance!.PART_DialogHolder.Children.Count > 0)
            {
                DialogBase oldDialog = (DialogBase)MainWindow.Instance.PART_DialogHolder.Children[0];
                oldDialog.CloseDialog(false);
            }

            DialogBase dialog = new();

            T dialogContent = Activator.CreateInstance<T>();
            onCreated(dialogContent);

            dialog.PART_Content.Children.Add(dialogContent);
            dialog.DialogClosed += (sender, result) => onResult((T)sender, result);

            MainWindow.Instance!.PART_DialogHolder.Children.Add(dialog);
        }
        public static DialogBase GetDialogWindow()
            => (DialogBase)MainWindow.Instance!.PART_DialogHolder.Children[0];
    }
}
