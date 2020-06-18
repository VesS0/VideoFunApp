using System;
using System.Collections.Generic;
using System.Text;
using Transcription.Tools;

namespace Transcription
{
    public class Audio: Media
    {
        public string Path { get; set; }
        
        public Audio() { }
        public Audio(string path)
        {
            this.Path = path;
        }
    }
}
