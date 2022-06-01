using NAudio.CoreAudioApi;
using OpenVoiceModder.Sound.Effects;
using OpenVoiceModder.Sound.Effects.Parameters;
using OpenVoiceModder.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace OpenVoiceModder.Utils
{
    public class Database
    {
        public bool Ready { get; set; }

        private Database() { }
        public static Database Instance { get; } = new();

        public bool SoundboardUseEffects
        {
            get => Properties.Settings.Default.SoundboardUseEffects;
            set
            {
                Properties.Settings.Default.SoundboardUseEffects = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool VoiceChangerUseEffects
        {
            get => Properties.Settings.Default.VoiceChangerUseEffects;
            set
            {
                Properties.Settings.Default.VoiceChangerUseEffects = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool VoiceMuted
        {
            get => Properties.Settings.Default.VoiceMuted;
            set
            {
                Properties.Settings.Default.VoiceMuted = value;
                Properties.Settings.Default.Save();
            }
        }

        public MMDevice InputDevice
        {
            get
            {
                string id = Properties.Settings.Default.InputUUID;

                MMDeviceEnumerator enumerator = new();

                try
                {
                    return enumerator.GetDevice(id);
                }
                catch (Exception ex)
                {
                    return enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
                }
            }
            set
            {
                Properties.Settings.Default.InputUUID = value.ID;
                Properties.Settings.Default.Save();
            }
        }
        public MMDevice OutputDevice
        {
            get
            {
                string id = Properties.Settings.Default.OutputUUID;

                MMDeviceEnumerator enumerator = new();

                try
                {
                    return enumerator.GetDevice(id);
                }
                catch (Exception ex)
                {
                    return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
                }
            }
            set
            {
                Properties.Settings.Default.OutputUUID = value.ID;
                Properties.Settings.Default.Save();
            }
        }


        public Key SoundboardHoldKey
        {
            get => (Key)Properties.Settings.Default.HoldKeyByte;
            set
            {
                Properties.Settings.Default.HoldKeyByte = (byte)value;
                Properties.Settings.Default.Save();
            }
        }

        public void LoadSounds(SoundBoard sb)
        {
            string[]? data = Properties.Settings.Default.Sounds?.Split('>');
            if (data == null || (data.Length == 1 && string.IsNullOrEmpty(data[0]))) return;

            foreach (string sound in data)
            {
                Uri uri = new(sound.Substring(0, sound.Length - 2), UriKind.Absolute);
                Key shortcut = (Key)byte.Parse(sound.Substring(sound.Length - 2), NumberStyles.HexNumber);

                sb.AddSound(uri, shortcut);
            }
        }
        public void SaveSounds(SoundBoard sb)
        {
            if (!Ready) return;

            List<string> data = new();

            foreach (UIElement control in sb.PART_Sounds.Children)
            {
                SoundInfoControl info = (SoundInfoControl)control;

                Uri uri = info.Uri;
                Key shortcut = info.Shortcut;

                data.Add($"{uri.AbsoluteUri}{((byte)shortcut).ToString("X2")}");
            }

            Properties.Settings.Default.Sounds = String.Join('>', data);
            Properties.Settings.Default.Save();
        }

        Effect? FindEffect(string name)
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type? type = asm.GetType(name);
                if (type == null) continue;

                return Activator.CreateInstance(type) as Effect;
            }

            return null;
        }

        public void LoadEffects(EffectsControl ef)
        {
            string[]? data = Properties.Settings.Default.Effects?.Split('>');
            if (data == null || (data.Length == 1 && string.IsNullOrEmpty(data[0]))) return;

            foreach (string effect in data)
            {
                string[] content = effect.Split('@');

                string typeName = content[0];
                Effect? instance = FindEffect(typeName);

                if (instance == null) continue;

                string[] parameters = content[1].Split(':');
                for (int i = 0; i < parameters.Length - 1; i++)
                {
                    string name = parameters[i++];
                    if (instance.HasParameter(name))
                        instance.Get<IParameter>(name).ParseString(parameters[i]);
                }

                string str = parameters[parameters.Length - 1];
                bool enabled = string.IsNullOrEmpty(str) ? true : str[0] == '1';

                instance.Enabled = enabled;
                ef.AddEffect(instance);
            }
        }
        public void SaveEffects(EffectsControl ef)
        {
            if (!Ready) return;

            List<string> data = new();

            foreach (UIElement element in ef.PART_ControllerRoot.Children)
            {
                EffectController controller = (EffectController)element;

                Effect effect = controller.MixerEffect;
                IEnumerable<IParameter> parameters = effect.Parameters;

                StringBuilder builder = new();

                builder.Append($"{effect.GetType().FullName!}@");
                foreach (IParameter parameter in parameters)
                {
                    builder.Append($"{parameter.Name.ToLower()}:");
                    builder.Append($"{parameter.BuildString()}:");
                }
                builder.Append(effect.Enabled ? '1' : '0');

                data.Add(builder.ToString());
            }

            Properties.Settings.Default.Effects = String.Join('>', data);
            Properties.Settings.Default.Save();
        }
    }
}
