using System;
using System.Collections.Generic;
using System.Text;
using Transcription.Tools;

namespace Transcription
{
    public class Video: Media
    {
        public string Path { get; set; }

        public Video() { }
        public Video(string path)
        {
            Path = path;
        }
        public static Video ImportVideo(string path)
        {
            return new Video(path);
        }
    }
}
