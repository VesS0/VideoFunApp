using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Transcription
{
    class CognitiveServices
    {
        public static Transcript GetTranscript(Audio audio)
        {
            return new Transcript();
        }

        public static Translation GetTranslation(Transcript trans, string[] languages)
        {
            return new Translation();
        }
    }
}
