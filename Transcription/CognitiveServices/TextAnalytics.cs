using System;
using System.Globalization;
using Azure.AI.TextAnalytics;

namespace Transcription
{
    public class TextAnalytics
    {
        TextAnalyticsClient client;
        public string outputText = "";
        static void SentimentAnalysisExample(TextAnalyticsClient client, string inputText)
        {
            DocumentSentiment documentSentiment = client.AnalyzeSentiment(inputText);
            Console.WriteLine($"Document sentiment: {documentSentiment.Sentiment}\n");

            var si = new StringInfo(inputText);
            foreach (var sentence in documentSentiment.Sentences)
            {
                Console.WriteLine($"\tSentence [length {sentence.GraphemeLength}]");
                Console.WriteLine($"\tText: \"{si.SubstringByTextElements(sentence.GraphemeOffset, sentence.GraphemeLength)}\"");
                Console.WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
                Console.WriteLine($"\tPositive score: {sentence.ConfidenceScores.Positive:0.00}");
                Console.WriteLine($"\tNegative score: {sentence.ConfidenceScores.Negative:0.00}");
                Console.WriteLine($"\tNeutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");
            }
        }

        static void EntityRecognitionExample(TextAnalyticsClient client, string inputText)
        {
            var response = client.RecognizeEntities(inputText);
            Console.WriteLine("Named Entities:");
            foreach (var entity in response.Value)
            {
                Console.WriteLine($"\tText: {entity.Text},\tCategory: {entity.Category},\tSub-Category: {entity.SubCategory}");
                Console.WriteLine($"\t\tLength: {entity.GraphemeLength},\tScore: {entity.ConfidenceScore:F2}\n");
            }
        }

        //static void LanguageDetectionExample(TextAnalyticsClient client)
        //{
        //    DetectedLanguage detectedLanguage = client.DetectLanguage("Ce document est rédigé en Français.");
        //    Console.WriteLine("Language:");
        //    Console.WriteLine($"\t{detectedLanguage.Name},\tISO-6391: {detectedLanguage.Iso6391Name}\n");
        //}

        static void EntityPIIExample(TextAnalyticsClient client, string inputText)
        {
            var response = client.RecognizePiiEntities(inputText);
            Console.WriteLine("Personally Identifiable Information Entities:");
            foreach (var entity in response.Value)
            {
                Console.WriteLine($"\tText: {entity.Text},\tCategory: {entity.Category},\tSub-Category: {entity.SubCategory}");
                Console.WriteLine($"\t\tLength: {entity.GraphemeLength},\tScore: {entity.ConfidenceScore:F2}\n");
            }
        }

        static void KeyPhraseExtractionExample(TextAnalyticsClient client, string inputText)
        {
            var response = client.ExtractKeyPhrases(inputText);

            // Printing key phrases
            Console.WriteLine("Key phrases:");

            foreach (string keyphrase in response.Value)
            {
                Console.WriteLine($"\t{keyphrase}");
            }
        }

        public string EntityLinkingExample(string inputText)
        {
            var response = client.RecognizeLinkedEntities(inputText);
            string outputText = "";
            foreach (var entity in response.Value)
            {
                outputText += "Naziv:\n" + entity.Name + "\nURL link:\n" + entity.Url + "\n";
                //Console.WriteLine($"\tName: {entity.Name},\tID: {entity.DataSourceEntityId},\tURL: {entity.Url}\tData Source: {entity.DataSource}");
                //Console.WriteLine("\tMatches:");
                //foreach (var match in entity.Matches)
                //{
                //    Console.WriteLine($"\t\tText: {match.Text}");
                //    Console.WriteLine($"\t\tLength: {match.GraphemeLength},\tScore: {match.ConfidenceScore:F2}\n");
                //}
            }
            return outputText;
        }

        public TextAnalytics()
        {
            TextAnalyticsApiKeyCredential credentials = new TextAnalyticsApiKeyCredential("febeb4159a3d474eaa98d81a55a70554");
            Uri endpoint = new Uri("https://cogsertextanal.cognitiveservices.azure.com/");
            client = new TextAnalyticsClient(endpoint, credentials);
            //var writeText = EntityLinkingExample(client, inputText);
            //System.IO.File.WriteAllText(path, writeText);
        }
    }
}

