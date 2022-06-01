using OpenVoiceModder.Sound.Effects;
using OpenVoiceModder.Sound.Effects.Parameters;
using OpenVoiceModder.Utils;
using System.Windows;
using System.Windows.Controls;

namespace OpenVoiceModder.UI
{
    /// <summary>
    /// Interação lógica para EffectController.xam
    /// </summary>
    public partial class EffectController : UserControl
    {
        public Effect MixerEffect { get; init; }
        public bool IsEffectEnabled
        {
            get => MixerEffect.Enabled;
            set
            {
                MixerEffect.Enabled = value;
                Database.Instance.SaveEffects(MainWindow.Instance!.PART_Effects);
            }
        }

        public EffectController(Effect effect)
        {
            InitializeComponent();
            this.DataContext = this;

            this.MixerEffect = effect;
            PART_EffectName.Text = effect.Name;

            SetParameters();
        }

        public delegate void EffectRemovedEvent(EffectController sender);
        public event EffectRemovedEvent? EffectRemoved;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EffectRemoved?.Invoke(this);
        }

        void SetParameters()
        {
            foreach (IParameter p in MixerEffect.Parameters)
            {
                p.AddParameterController(this.PART_Parameters);
                p.ParameterUpdated += (snd) => Database.Instance.SaveEffects(MainWindow.Instance!.PART_Effects);
            } //this.PART_Parameters.Children.Add(new ParameterController(p));
        }
    }
}
