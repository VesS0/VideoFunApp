using System;

namespace Transcription
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var video = Video.ImportVideo("blabla");

            var audio = ffmpeg.ExtractAudio(video);

            var transcript = CognitiveServices.GetTranscript(audio);

            var translation = CognitiveServices.GetTranslation(transcript, new[] { "en", "rs", "fr" });
        
        }
    }
}
