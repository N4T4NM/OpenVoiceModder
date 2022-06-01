using OpenVoiceModder.Sound;
using OpenVoiceModder.Sound.Effects;
using OpenVoiceModder.Sound.Utils;
using OpenVoiceModder.Utils;
using System;
using System.Windows.Controls;

namespace OpenVoiceModder.UI
{
    /// <summary>
    /// Interação lógica para EffectsControl.xam
    /// </summary>
    public partial class EffectsControl : UserControl
    {
        public bool IsMuted
        {
            get => !GlobalSound.Instance.CurrentMicrophoneSource!.Enabled;
            set
            {
                GlobalSound.Instance.CurrentMicrophoneSource!.Enabled = !value;
                Database.Instance.VoiceMuted = value;
            }
        }
        public bool ApplyEffects
        {
            get => GlobalSound.Instance.CurrentAudioSource!.ApplyEffects;
            set
            {
                GlobalSound.Instance.CurrentAudioSource!.ApplyEffects = value;
                Database.Instance.VoiceChangerUseEffects = value;
            }
        }

        public EffectsControl()
        {
            InitializeComponent();
            this.DataContext = this;

            this.PART_EffectsSelector.SetText("Add Effect");

            foreach (Type effect in Reflection.PublicEffects)
                this.PART_EffectsSelector.AddItem(((Effect)Activator.CreateInstance(effect)!).Name, effect);

            this.PART_EffectsSelector.ItemSelected += OnEffectAdd;
        }

        private void OnEffectAdd(object? item)
        {
            Effect effect = (Effect)Activator.CreateInstance((Type)item!)!;
            this.PART_EffectsSelector.SetText("Add Effect");

            AddEffect(effect);
        }

        private void OnEffectRemoved(EffectController sender)
        {
            this.PART_ControllerRoot.Children.Remove(sender);
            GlobalSound.Instance.CurrentAudioSource!.Effects.Remove(sender.MixerEffect);

            Database.Instance.SaveEffects(this);
        }

        public void AddEffect(Effect effect)
        {
            EffectController controller = new(effect);
            controller.EffectRemoved += OnEffectRemoved;

            this.PART_ControllerRoot.Children.Add(controller);
            GlobalSound.Instance.CurrentAudioSource!.Effects.Add(effect);

            Database.Instance.SaveEffects(this);
        }
    }
}
