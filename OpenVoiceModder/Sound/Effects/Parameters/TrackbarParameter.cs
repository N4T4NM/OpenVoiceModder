using OpenVoiceModder.UI.ParameterUI;
using System.Windows;
using System.Windows.Controls;

namespace OpenVoiceModder.Sound.Effects.Parameters
{
    public class TrackbarParameter : IParameter
    {
        float _value;

        public string Name { get; init; }
        public string Suffix { get; init; }
        public string TextFormat { get; init; }

        public float Min { get; init; }
        public float Max { get; init; }
        public float Jump { get; init; }
        public float Value
        {
            get => _value;
            set => _control.PART_Trackbar.Value = value;
        }

        TrackbarParameterControl _control = new();
        public event IParameter.ParameterUpdatedEvent? ParameterUpdated;

        public TrackbarParameter(string name, string suffix, float min, float max, float jump, float @default, string format = "0.00")
        {
            Name = name;
            Suffix = suffix;
            TextFormat = format;

            Min = min;
            Max = max;
            Jump = jump;
            _value = @default; //Thread-Safe
        }
        public void AddParameterController(Panel target)
        {
            _control.PART_MinValueText.Text = $"{Min.ToString(TextFormat)}{Suffix}";
            _control.PART_MaxValueText.Text = $"{Max.ToString(TextFormat)}{Suffix}";

            _control.PART_ParameterName.Text = Name;

            _control.PART_Trackbar.TickFrequency = Jump;
            _control.PART_Trackbar.Minimum = Min;
            _control.PART_Trackbar.Maximum = Max;
            _control.PART_Trackbar.Value = Value;
            _control.PART_Trackbar.ValueChanged += OnValueChanged;

            _control.PART_ValueText.Text = $"{Value.ToString(TextFormat)}{Suffix}";

            target.Children.Add(_control);
        }
        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _value = (float)((Slider)sender).Value;
            _control.PART_ValueText.Text = $"{Value.ToString(TextFormat)}{Suffix}";

            ParameterUpdated?.Invoke(this);
        }

        public string BuildString() => Value.ToString();
        public void ParseString(string str)
        {
            float value = float.Parse(str);
            _value = value;
        }

        public static implicit operator float(TrackbarParameter parameter) => parameter.Value;
    }
}
