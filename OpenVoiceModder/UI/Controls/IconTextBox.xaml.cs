using System.Windows;
using System.Windows.Controls;

namespace OpenVoiceModder.UI.Controls
{
    /// <summary>
    /// Interação lógica para IconTextBox.xam
    /// </summary>
    public partial class IconTextBox : UserControl
    {
        public string Hint
        {
            get => PART_Hint.Text;
            set => PART_Hint.Text = value;
        }
        public string Text { get => PART_Text.Text; set => PART_Text.Text = value; }
        public string Icon { get => PART_Icon.Text; set => PART_Icon.Text = value; }

        public IconTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void PART_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            PART_Hint.Visibility = Text.Length == 0 ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
