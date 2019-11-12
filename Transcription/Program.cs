using System;

namespace Transcription
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var pathToVideo = @"C:\Repos\video-1573565254.mp4";

            var video = Video.ImportVideo(pathToVideo);

            var audio = ffmpeg.ExtractAudio(video);

            var transcript = CognitiveServices.GetTranscript(audio);

            var translation = CognitiveServices.GetTranslation(transcript, new[] { "en", "rs", "fr" });
        
        }
    }
}
