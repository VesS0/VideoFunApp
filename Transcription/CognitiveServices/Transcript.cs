using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Transcription
{
    public class Transcript
    {
        public Lazy<string> TranscriptBulkText => new Lazy<string>(TranscriptLines.Aggregate(AggregateTranscript));

        public List<string> TranscriptLines { get; set; } = new List<string>();

        public IEnumerable<string> GetNextLine()
        {
            foreach (string line in TranscriptLines)
            {
                yield return line;
            }
        }

        public enum Language
        {
            en = 0,
            fr = 1,
        }

        public Transcript(Audio audio, Language lang)
        {
            RecognizeSpeechAsync(audio.Path, "simple", languages[(int)lang]).Wait();
        }

        private static string[] languages = new string[] { "en-us", "fr-fr" };

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
                                    AddLine(e.Result.Text);
                                    break;
                                case "detailedbest":
                                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Best().FirstOrDefault()?.Text}");
                                    AddLine(e.Result.Best().FirstOrDefault()?.Text);
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

            System.IO.File.WriteAllText(pathTranscript, TranscriptBulkText);
        }

        private static string AggregateTranscript(string aggregated, string newLine)
        {
            return aggregated + Environment.NewLine + newLine;
        }

        private void AddLine(string line)
        {
            TranscriptLines.Add(line);
        }
    }
}
