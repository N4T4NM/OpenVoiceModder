using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenVoiceModder.UI.Controls
{
    /// <summary>
    /// Interação lógica para ShortcutEditorControl.xam
    /// </summary>
    public partial class ShortcutEditorControl : UserControl
    {
        Key _shortcut = Key.None;

        public Key Shortcut
        {
            get => _shortcut;
            set
            {
                _shortcut = value;
                PART_ShortcutBox.Text = value.ToString();
            }
        }
        public bool Editing { get; protected set; }

        public delegate void StartEditingEvent(Key currentShortcut);
        public event StartEditingEvent? StartEditing;

        public delegate void EndEditingEvent(Key newShortcut);
        public event EndEditingEvent? EndEditing;

        public ShortcutEditorControl()
        {
            InitializeComponent();

            PART_ShortcutBox.PART_Text.IsReadOnly = true;
            PART_ShortcutBox.PART_Text.IsReadOnlyCaretVisible = false;

            PART_ShortcutBox.PreviewKeyDown += OnShortcutChange;
            PART_EditButton.Click += OnStartChange;

        }

        private void OnStartChange(object sender, RoutedEventArgs e)
        {
            Editing = true;
            PART_EditButton.IsEnabled = false;
            PART_EditButton.Visibility = Visibility.Hidden;

            PART_ShortcutBox.PART_Text.IsReadOnlyCaretVisible = true;
            PART_ShortcutBox.PART_Text.Focus();

            StartEditing?.Invoke(Shortcut);
        }

        private void OnShortcutChange(object sender, KeyEventArgs e)
        {
            if (!Editing) return;
            e.Handled = true;
            SetShortcut(e.Key);
        }

        void SetShortcut(Key key)
        {
            Editing = false;

            if (key == Key.Escape) key = Key.None;

            Shortcut = key;
            PART_ShortcutBox.Text = Shortcut.ToString();

            PART_ShortcutBox.PART_Text.IsReadOnlyCaretVisible = false;
            PART_EditButton.IsEnabled = true;
            PART_EditButton.Visibility = Visibility.Visible;
            PART_EditButton.Focus();

            EndEditing?.Invoke(Shortcut);
        }
    }
}
