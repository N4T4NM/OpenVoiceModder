using System.Windows;
using System.Windows.Controls;

namespace OpenVoiceModder.UI.Dialogs
{
    /// <summary>
    /// Interação lógica para DialogBase.xam
    /// </summary>
    public partial class DialogBase : UserControl
    {
        public DialogBase()
        {
            InitializeComponent();
        }

        public delegate void DialogClosedEvent(UIElement source, bool result);
        public event DialogClosedEvent? DialogClosed;

        private void PART_CloseButton_Click(object sender, RoutedEventArgs e)
            => CloseDialog(false);
        public void CloseDialog(bool result)
        {
            MainWindow.Instance!.PART_DialogHolder.Children.Remove(this);
            DialogClosed?.Invoke(PART_Content.Children[0], result);
        }
    }
}
