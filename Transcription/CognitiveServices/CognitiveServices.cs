using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Transcription
{
    class CognitiveServices
    {
        public static void GetTranscript(Audio audio)
        {
            try
            {
                Transcript.RecognizeSpeechAsync(audio.Path, "simple", "it-it").Wait();
            }
            catch(Exception ex)
            {
                _ = ex;
            }
        }

        public static Translation GetTranslation(Transcript trans, string[] languages)
        {
            return new Translation();
        }

        private static void PingCognitiveServices()
        {

        }
    }
}
