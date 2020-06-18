using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transcription;

namespace VideoFunAppTests
{
    [TestClass]
    public class Tests
    {
        private const string translationkeySecretIdentifier = "https://francevideofunappsecr.vault.azure.net/secrets/TranslationKey/d32fce40406c490194ebd9a6ff868096";
        private const string subscriptionKeySecretIdentifier = "https://francevideofunappsecr.vault.azure.net/secrets/SubscriptionKey/6e0295187d9d416caa73167391bfe3be";
        private const string applicationId = "473fb467-aa96-4e5f-b70c-1e4296483756";
        private const string testVideoPath = @"./TestResources/testFile.mp4";

        /// <summary>
        /// Performs cleanup after every test by deleting the test output (if it exists).
        /// </summary>
        [TestCleanup]
        public void CleanUpTest()
        {
            var fileNameNoExt = Path.Combine(Path.GetDirectoryName(testVideoPath), Path.GetFileNameWithoutExtension(testVideoPath));
            var wavFileName = fileNameNoExt + ".wav";
            var logFileName = fileNameNoExt + ".txt";

            if (File.Exists(wavFileName))
            {
                File.Delete(wavFileName);
            }

            if (File.Exists(logFileName))
            {
                File.Delete(logFileName);
            }
        }

        [TestMethod]
        public void ffmpegValidTest()
        {
            var video = new Video(testVideoPath);
            var audio = ffmpeg.ExtractAudio(video);

            Assert.IsTrue(File.Exists(audio.Path));
        }

        [TestMethod]
        public void FileDoesNotExist()
        {
            var video = new Video(@"./TestResources/doesNotExist.mp4");
            var audio = ffmpeg.ExtractAudio(video);

            Assert.IsFalse(File.Exists(audio.Path));
        }

        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void SecretProviderInvalidSecret()
        {
            var secretProvider = new SecretProvider("invalid", "invalid", applicationId);

            var secret = secretProvider.GetSubscriptionKey();
        }

        [TestMethod]
        [ExpectedException(typeof(Microsoft.IdentityModel.Clients.ActiveDirectory.AdalServiceException))]
        public void SecretProviderValidSecretInvalidAppId()
        {
            var secretProvider = new SecretProvider(translationkeySecretIdentifier, subscriptionKeySecretIdentifier, "invalid");

            var secret = secretProvider.GetTranslationKey();
        }

        [TestMethod]
        public void SecretProviderSuccessfulWorkflow()
        {
            var secretProvider = new SecretProvider(translationkeySecretIdentifier, subscriptionKeySecretIdentifier, applicationId);

            var secret = secretProvider.GetSubscriptionKey();

            Assert.IsFalse(String.IsNullOrEmpty(secret));
        }
    }
}
