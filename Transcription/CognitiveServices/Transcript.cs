﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Transcription
{
    public class Transcript
    {
        public Lazy<string> TranscriptBulkText => new Lazy<string>(TranscriptLines.lines.Aggregate(AggregateTranscript));

        public TranscriptLineInfos TranscriptLines { get; internal set; } = new TranscriptLineInfos();

        public IEnumerable<string> GetNextLine()
        {
            foreach (string line in TranscriptLines.lines)
            {
                yield return line;
            }
        }

        public enum Language
        {
            ArabicEgypt,
            ArabicSaudiArabia,
            ArabicUAE,
            ArabicKuwait,
            ArabicQatar,
            Catalan,
            Danish,
            German,
            EnglishAustralia,
            EnglishCanada,
            EnglishUK,
            EnglishIndia,
            EnglishnewZeland,
            EnglishUS,
            SpanishSpain,
            SpanishMexico,
            Finnish,
            French,
            Gujarati,
            Hindi,
            Italian,
            Japanese,
            Korean,
            Marathi,
            Norwegian,
            Dutch,
            Polish,
            PortugueseBrazil,
            PortuguesePortugal,
            Russian,
            Swedish,
            Tamil,
            Telugu,
            ChineseSimplified,
            ChineseTraditional,
            ChineseMandarin,
            Thai,
            Turkey
        }

    static string LanguageCode(Language language)
        {
            string[] languages = new string[]{ "ar-EG", "ar-SA", "ar-AE", "ar-KW", "ar-QA",
                "ca-ES", "da-DK", "de-DE", "en-AU", "en-CA", "en-GB", "en-IN", "en-NZ",
                "en-US", "es-ES", "es-MX", "fi-FI", "fr-CA", "fr-FR", "gu-IN", "hi-IN",
                "it-IT", "ja-JP", "ko-KR", "mr-IN", "nb-NO", "nl-NL", "pl-PL", "pt-BR",
                "pt-PT", "ru-RU", "sv-SE", "ta-IN", "te-IN", "zh-CN", "zh-HK", "zh-TW",
                "th-TH", "tr-TR" };
            return languages[(int)language];
        }

        public Transcript(Audio audio, Language lang)
        {
            RecognizeSpeechAsync(audio.Path, "detailedbest", LanguageCode(lang)).Wait();
        }

        private async Task RecognizeSpeechAsync(
            string pathWav = @"C:\Users\v-isbojo\Pictures\OtherLangVideo\frfr_output.wav",
            string cogServiceOption = "simple",
            string speechLanguage = "en-us"
            )
        {
            var config = SpeechConfig.FromSubscription("5d282cd785cf4cf6aacbd809fbdc7576", "francecentral");

            config.SpeechRecognitionLanguage = speechLanguage;
            switch (cogServiceOption)
            {
                case "simple":
                    config.OutputFormat = OutputFormat.Simple;
                    break;
                case "detailedtext":
                case "detailedbest":
                    config.OutputFormat = OutputFormat.Detailed;
                    break;
                default:
                    throw new ArgumentException();
            }
            var stopRecognition = new TaskCompletionSource<int>();
            using (var audioConfig = AudioConfig.FromWavFileInput(pathWav))
            {
                using (var recognizer = new SpeechRecognizer(config, audioConfig))
                {
                    // Subscribes to events.
                    recognizer.Recognizing += (s, e) =>
                    {
                        Console.WriteLine($"\n    Partial result: {e.Result.Text}.");
                    };

                    recognizer.Recognized += (s, e) =>
                    {

                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            switch (cogServiceOption)
                            {
                                case "simple":
                                case "detailedtext":
                                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                                    AddLineInfo(new TranscriptLineInfo(e.Result.Text, e.Result.OffsetInTicks, e.Result.Duration));
                                    break;
                                case "detailedbest":
                                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Best().FirstOrDefault()?.Text}");
                                    
                                    AddLineInfo(new TranscriptLineInfo(e.Result.Best().FirstOrDefault()?.Text, e.Result.OffsetInTicks, e.Result.Duration));
                                    break;
                                default:
                                    throw new ArgumentException();
                            }
                        }

                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                        }
                    };

                    recognizer.Canceled += (s, e) =>
                    {
                        Console.WriteLine($"CANCELED: Reason={e.Reason}");
                        if (e.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                        stopRecognition.TrySetResult(0);
                    };

                    recognizer.SessionStarted += (s, e) =>
                    {
                        Console.WriteLine("\n    Session started event.");
                    };

                    recognizer.SessionStopped += (s, e) =>
                    {
                        Console.WriteLine("\n    Session stopped event.");
                        Console.WriteLine("\nStop recognition.");
                        stopRecognition.TrySetResult(0);
                    };

                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });

                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                }
            }

            string pathTranscript = pathWav.Substring(0, pathWav.Length - 3) + "txt";

            System.IO.File.WriteAllText(pathTranscript, TranscriptBulkText.Value);
        }

        private static string AggregateTranscript(string aggregated, string newLine)
        {
            return aggregated + Environment.NewLine + newLine;
        }

        private void AddLineInfo(TranscriptLineInfo info)
        {
            TranscriptLines.AddTranscriptionLineInfo(info);
        }

        public class TranscriptLineInfo
        {
            public TranscriptLineInfo(string line, long offset=0, TimeSpan duration = default(TimeSpan))
            {
                this.line = line;
                this.offset = offset;
                this.durationinSeconds = duration;
            }

            public string line;
            public long offset;
            public TimeSpan durationinSeconds;
        }

        public class TranscriptLineInfos
        {
            public List<string> lines = new List<string>();
            public List<long> offsets = new List<long>();
            public List<TimeSpan> duration = new List<TimeSpan>();

            public void AddTranscriptionLineInfo(TranscriptLineInfo info)
            {
                lines.Add(info.line);
                offsets.Add(info.offset);
                duration.Add(info.durationinSeconds);
            }
        }
    }
}
