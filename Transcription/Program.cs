using System;

namespace Transcription
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            var pathToVideo = @"C:\Users\v-isbojo\Pictures\OtherLangVideo\aa.mp4";

            var video = Video.ImportVideo(pathToVideo);

            var audio = ffmpeg.ExtractAudio(video);

            Transcript transcript = new Transcript(audio, Transcript.Language.EnglishUS);

            TranslateText translate = new TranslateText(String.Join(" ", transcript.TranscriptLines));

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }
    }
}
