using System;

namespace Transcription
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            var pathToVideo = @"C:\Users\v-isbojo\Pictures\OtherLangVideo\itit.mp4";

            var video = Video.ImportVideo(pathToVideo);

            var audio = ffmpeg.ExtractAudio(video);

            CognitiveServices.GetTranscript(audio);

            //var translation = CognitiveServices.GetTranslation(transcript, new[] { "en", "rs", "fr" });
        }
    }
}
