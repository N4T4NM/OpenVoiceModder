using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenVoiceModder.UI
{
    /// <summary>
    /// Interação lógica para DropdownMenu.xam
    /// </summary>
    public partial class DropdownMenu : UserControl
    {
        public DropdownMenu()
        {
            InitializeComponent();
        }

        public delegate void ItemSelectedEvent(object? item);
        public event ItemSelectedEvent? ItemSelected;

        readonly Dictionary<string, UIElement> _items = new();

        public object? Selected { get; private set; }

        public void SetText(string text)
        {
            PART_SelectionText.Text = text;
        }

        public void AddItem(string text, object? obj)
        {
            TextBlock block = new();
            block.Text = text;
            block.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            block.Tag = obj;
            block.Cursor = Cursors.Hand;
            block.MouseLeftButtonDown += OnItemSelected;

            _items.Add(text, block);
            PART_DropdownItemsRoot.Children.Add(block);
        }

        private void OnItemSelected(object sender, MouseButtonEventArgs e)
        {
            string item = ((TextBlock)sender).Text;

            PART_SelectionText.Text = item;
            Selected = ((TextBlock)sender).Tag;
            DropdownToggled = false;

            ItemSelected?.Invoke(Selected);
        }

        public void RemoveItem(string text)
        {
            PART_DropdownItemsRoot.Children.Remove(_items[text]);
            _items.Remove(text);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                ToggleDropDown();
        }

        public bool DropdownToggled
        {
            get => PART_DropdownMenu.Visibility == Visibility.Visible;
            set
            {
                PART_DropdownMenu.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                PART_SelectionTextRoot.CornerRadius = new(5, 0, 0, value ? 0 : 5);
            }
        }

        void ToggleDropDown()
        {
            DropdownToggled = !DropdownToggled;
            if (!DropdownToggled)
            {
                PART_SelectionTextRoot.CornerRadius = new(5, 0, 0, 5);
                return;
            }

            PART_SelectionTextRoot.CornerRadius = new(5, 0, 0, 0);
        }
    }
}
