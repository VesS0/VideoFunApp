using System.Diagnostics;
using System.IO;

namespace Transcription
{
    public static class ffmpeg
    {
        public static Audio ExtractAudio(Video video)
        {
            string extension = Path.GetExtension(video.Path);
            string audioNoext = Path.Combine(Path.GetDirectoryName(video.Path), Path.GetFileNameWithoutExtension(video.Path));

            var audio = new Audio(audioNoext + ".wav");

            string cmdConvertmp4Towav;
            // Cog Services can only transcribe audio files of a particular frequency and format
            cmdConvertmp4Towav = "/C .\\Tools\\ffmpeg.exe -i " + "\"" + video.Path + "\"" + " -acodec pcm_s16le -ac 1 -ar 16000 " + "\"" + audio.Path + "\"";
            var processInfo = new ProcessStartInfo("CMD.exe", cmdConvertmp4Towav);

            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = true;
            var process = Process.Start(processInfo);
            process.WaitForExit();
            foreach (var p in Process.GetProcessesByName("ffmpeg"))
            {
                p.Kill();
            }

            return audio;
        }

        public static void Mp4toWav(string speechFile, string wavfile)
        {
            
        }
    }
}
