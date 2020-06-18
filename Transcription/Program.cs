using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Transcription
{
    class Program
    {
        private const string translationkeySecretIdentifier = "https://francevideofunappsecr.vault.azure.net/secrets/TranslationKey/d32fce40406c490194ebd9a6ff868096";
        private const string subscriptionKeySecretIdentifier = "https://francevideofunappsecr.vault.azure.net/secrets/SubscriptionKey/6e0295187d9d416caa73167391bfe3be";
        private const string applicationId = "473fb467-aa96-4e5f-b70c-1e4296483756";

        static int LevenshtainDistance<L, T>(L generatedWords, L gtWords)
        where T : IEquatable<T>
        where L : IEnumerable<T>
        {
            int levenshtainDistance = 0;

            if (generatedWords.Count() == 0)
            {
                return gtWords.Count();
            }

            if (gtWords.Count() == 0)
            {
                return generatedWords.Count();
            }

            int[,] distanceMatrix = new int[generatedWords.Count() + 1, gtWords.Count() + 1];
            for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            {
                distanceMatrix[i, 0] = i;
            }
            for (int i = 0; i < distanceMatrix.GetLength(1); i++)
            {
                distanceMatrix[0, i] = i;
            }

            for (int i = 1; i < distanceMatrix.GetLength(0); i++)
            {
                for (int j = 1; j < distanceMatrix.GetLength(1); j++)
                {
                    int cost = 1;
                    if (generatedWords.ElementAt(i - 1).Equals(gtWords.ElementAt(j - 1)))
                    {
                        cost = 0;
                    }

                    int a = distanceMatrix[i - 1, j] + 1;
                    int b = distanceMatrix[i, j - 1] + 1;
                    int c = distanceMatrix[i - 1, j - 1] + cost;
                    distanceMatrix[i, j] = Math.Min(a, Math.Min(b, c));
                }
            }

            levenshtainDistance = distanceMatrix[generatedWords.Count(), gtWords.Count()];

            return levenshtainDistance;
        }

        public static float WordErrorRate(string testTranscript, string generatedTranscript)
        {
            using (var gtReader = new System.IO.StreamReader(testTranscript))
            {
                string gtText = gtReader.ReadToEnd();

                List<string> gtWords = gtText.Split('\n').ToList();
                var realTranscript = new List<string>();
                gtWords = gtWords.Where((x) => !x.StartsWith("00:")&& !x.StartsWith("WEBVTT") && !x.StartsWith("Kind:") && !x.StartsWith("Language:")).ToList();
                foreach (var gtWord in gtWords)
                {
                    realTranscript.AddRange(gtWord.Split(' '));
                }
                realTranscript.RemoveAll(s => s == string.Empty);
                List<string> generatedWords = generatedTranscript.Split(' ').ToList();
                generatedWords.RemoveAll(s => s == string.Empty);

                float errorRate = LevenshtainDistance<List<string>, string>(generatedWords, realTranscript);
                return (errorRate / realTranscript.Count());
            }
        }

        public static void TranscriptTranslation()
        {
            //Console.WriteLine("Hello World!");
            var pathToVideo = @"C:\Users\v-isbojo\Pictures\OtherLangVideo\aa.mp4";

            var video = Video.ImportVideo(pathToVideo);

            var audio = ffmpeg.ExtractAudio(video);
            var secretProvider = new SecretProvider(translationkeySecretIdentifier, subscriptionKeySecretIdentifier, applicationId);

            Transcript transcript = new Transcript(secretProvider, audio, Transcript.Language.EnglishUS);

            Translation translate = new Translation(secretProvider, transcript);

            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        public static void TestingTranscript()
        {
            var secretProvider = new SecretProvider(translationkeySecretIdentifier, subscriptionKeySecretIdentifier, applicationId);

            //write value
            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("E:\\diplomskiTesting", "HeyHeyHeyIspravljeno.txt")))
            {
                string[] foldersInDirectory = Directory.GetDirectories("E:\\diplomskiTesting");
            float wordErrorRateSum = 0;
            int testCount = 0;
            foreach (var folder in foldersInDirectory)
            {
                string[] subfoldersInDirectory = Directory.GetDirectories(folder);
                string testFile = "";
                foreach (var subFolder in subfoldersInDirectory)
                {
                    var allFiles = Directory.GetFiles(subFolder);
                        
                        Parallel.ForEach(allFiles, (file) =>
                        {
                            if (file.EndsWith(".mp4"))
                            {
                                try
                                {
                                    var video = Video.ImportVideo(file);

                                    var audio = ffmpeg.ExtractAudio(video);

                                    Transcript transcript = new Transcript(secretProvider, audio, Path.GetFileName(folder));

                                    testFile = allFiles.Where((f) => f.Contains(Path.GetFileNameWithoutExtension(file))).Where(x => x.EndsWith(".vtt")).FirstOrDefault();
                                    var wer = WordErrorRate(testFile, transcript.TranscriptBulkText.Value);
                                    wordErrorRateSum += wer;
                                    outputFile.WriteLine("WER for file " + file + " " + wer);
                                    testCount++;
                                    Console.WriteLine("Press any key to continue.");
                                    //Console.ReadKey();

                                }
                                catch (Exception ex)
                                {
                                    outputFile.WriteLine("Skipping " + file);
                                    Console.WriteLine("Skipping " + file);
                                }
                            }

                        });
                    //foreach (var file in allFiles)
                    //{
                    //}
                }

                    outputFile.WriteLine(Path.GetFileName(folder));
                    outputFile.WriteLine(wordErrorRateSum);
                    outputFile.WriteLine(testCount);
                    outputFile.WriteLine(wordErrorRateSum / testCount);
                }
                outputFile.WriteLine("KRAJ");
                outputFile.WriteLine(wordErrorRateSum);
                outputFile.WriteLine(testCount);
                outputFile.WriteLine(wordErrorRateSum / testCount);
            }
        }

        static void Main(string[] args)
        {
            TestingTranscript();

            //WordErrorRate("E:\\Data\\de-DE\\list=PLzPiBVgAHXijVDasy92X6lZkl0DvFgSEg\\Alles ist möglich, dem, der da sieht _ Franziska Pohlmann _ TEDxLeuphanaUniversityLüneburg-nyani1mvZQw.de.vtt", "");


            //var video = Video.ImportVideo("E:\\diplomskiTesting\\de-DE\\YouTubeTEDTalk\\1HyCI4Q0-r4.mp4");

            //var audio = ffmpeg.ExtractAudio(video);

            //Transcript transcript = new Transcript(audio, "de-DE");

            //var testFile = "E:\\diplomskiTesting\\de-DE\\YouTubeTEDTalk\\1HyCI4Q0-r4.vtt";
            //var wer = WordErrorRate(testFile, "");
            //Console.WriteLine("Press any key to continue.");
            //Console.ReadKey();
        }
    }
}
