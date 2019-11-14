﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Transcription
{
    /// <summary>
    /// The C# classes that represents the JSON returned by the Translator Text API.
    /// </summary>
    public class TranslationResult
    {
        public DetectedLanguage DetectedLanguage { get; set; }
        public TextResult SourceText { get; set; }
        public Translation[] Translations { get; set; }
    }

    public class DetectedLanguage
    {
        public string Language { get; set; }
        public float Score { get; set; }
    }

    public class TextResult
    {
        public string Text { get; set; }
        public string Script { get; set; }
    }

    public class Translation
    {
        public string Text { get; set; }
        public TextResult Transliteration { get; set; }
        public string To { get; set; }
        public Alignment Alignment { get; set; }
        public SentenceLength SentLen { get; set; }
    }

    public class Alignment
    {
        public string Proj { get; set; }
    }

    public class SentenceLength
    {
        public int[] SrcSentLen { get; set; }
        public int[] TransSentLen { get; set; }
    }

    public class TranslateText
    {
        public enum Language
        {
            Afrikaans,
            Arabic,
            Bangla,
            Bosnian,
            Bulgarian,
            CantoneseTraditional,
            Catalan,
            ChineseSimplified,
            ChineseTraditional,
            Croatian,
            Czech,
            Danish,
            Dutch,
            English,
            Estonian,
            Fijian,
            Filipino,
            Finnish,
            French,
            German,
            Greek,
            HaitianCreole,
            Hebrew,
            Hindi,
            HmongDaw,
            Hungarian,
            Icelandic,
            Indonesian,
            Italian,
            Japanese,
            Kiswahili,
            Klingon,
            KlingonplqaD,
            Korean,
            Latvian,
            Lithuanian,
            Malagasy,
            Malay,
            Maltese,
            Norwegian,
            Persian,
            Polish,
            Portuguese,
            QueretaroOtomi,
            Romanian,
            Russian,
            Samoan,
            SerbianCyrillic,
            SerbianLatin,
            Slovak,
            Slovenian,
            Spanish,
            Swedish,
            Tahitian,
            Tamil,
            Telugu,
            Thai,
            Tongan,
            Turkish,
            Ukrainian,
            Urdu,
            Vietnamese,
            Welsh,
            YucatecMaya
        }

        static string LanguageCode(Language language)
        {
            string[] languages = new string[]{ "af", "ar", "bn", "bs", "bg", "yue", "ca", "zh-Hans",
                "zh-Hant", "hr", "cs", "da", "nl", "en", "et", "fj", "fil", "fi", "fr", "de", "el",
                "ht", "he", "hi", "mww", "hu", "is","id", "it", "ja", "sw", "tlh", "tlh-Qaak", "ko",
                "lv", "lt", "mg", "ms", "mt", "nb", "fa", "pl", "pt", "otq", "ro", "ru", "sm",
                "sr-Cyrl", "sr-Latn", "sk", "sl", "es", "sv", "ty", "ta", "te", "th", "to", "tr",
                "uk", "ur", "vi", "cy", "yua" };
            return languages[(int)language];
        }

        private const string key_var = "40d19132f2224d82aa4fd3b947c11b80";
        //private static readonly string subscriptionKey = Environment.GetEnvironmentVariable(key_var);

        private const string endpoint_var = "https://api-eur.cognitive.microsofttranslator.com";
        //private static readonly string endpoint = Environment.GetEnvironmentVariable(endpoint_var);

        // This sample requires C# 7.1 or later for async/await.
        // Async call to the Translator Text APIS
        static public async Task TranslateTextRequest(
            string route = "/translate?api-version=3.0&to=de&to=it&to=ja&to=th&to=sr-Latn", 
            string inputText="Hello, My name is Isidora")
        {
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                // Set the method to Post.
                request.Method = HttpMethod.Post;
                // Construct the URI and add headers.
                request.RequestUri = new Uri(endpoint_var + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key_var);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                // Deserialize the response using the classes created earlier.
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                // Iterate over the deserialized results.
                foreach (TranslationResult o in deserializedOutput)
                {
                    // Print the detected input language and confidence score.
                    Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", o.DetectedLanguage.Language, o.DetectedLanguage.Score);
                    // Iterate over the results and print each translation.
                    foreach (Translation t in o.Translations)
                    {
                        Console.WriteLine("Translated to {0}: {1}", t.To, t.Text);
                    }
                }
            }
        }
    }
}
