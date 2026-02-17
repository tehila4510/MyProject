using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Text;

namespace Services.Services
{
    public class Speech
    {
        private readonly SpeechSynthesizer _synthesizer;

        public Speech()
        {
            _synthesizer = new SpeechSynthesizer();
            // ברירת מחדל - קול נשי אנגלי US
            _synthesizer.SelectVoice("Microsoft Zira Desktop");
        }

        /// <summary>
        /// השמעת טקסט עם קול ברירת מחדל
        /// </summary>
        public void Speak(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            _synthesizer.Speak(text);
        }

        /// <summary>
        /// השמעת טקסט עם קול שנבחר דינמית לפי שם
        /// </summary>
        public void Speak(string text, string voiceName)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(voiceName))
                return;

            try
            {
                _synthesizer.SelectVoice(voiceName);
                _synthesizer.Speak(text);
            }
            catch (Exception ex)
            {
                // במקרה שהקול לא קיים, נשתמש בקול ברירת המחדל
                Console.WriteLine($"Voice '{voiceName}' not found. Using default voice. Error: {ex.Message}");
                _synthesizer.SelectVoice("Microsoft Zira Desktop");
                _synthesizer.Speak(text);
            }
        }

        /// <summary>
        /// מחזיר את כל הקולות המותקנים במערכת
        /// </summary>
        public List<string> GetInstalledVoices()
        {
            var voices = new List<string>();
            foreach (var voice in _synthesizer.GetInstalledVoices())
            {
                voices.Add(voice.VoiceInfo.Name);
            }
            return voices;
        }
    }
}
