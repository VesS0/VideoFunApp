using System;
using System.Collections.Generic;
using System.Text;

namespace Transcription
{
    public class Audio
    {
        public string Path { get; set; }

        public Audio() { }
        public Audio(string path)
        {
            this.Path = path;
        }
    }
}
