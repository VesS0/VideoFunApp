using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Transcription
{
    class SecretProvider
    {
        private const string translationkeySecretIdentifier = "https://francevideofunappsecr.vault.azure.net/secrets/TranslationKey/d32fce40406c490194ebd9a6ff868096";
        private const string subscriptionKeySecretIdentifier = "https://francevideofunappsecr.vault.azure.net/secrets/SubscriptionKey/6e0295187d9d416caa73167391bfe3be";
        private const string applicationId = "473fb467-aa96-4e5f-b70c-1e4296483756";

        private static KeyVaultClient keyVaultClient = null;
        private static AzureServiceTokenProvider tokenProvider = new AzureServiceTokenProvider();

        public static string GetSubscriptionKey()
        {
            return "5d282cd785cf4cf6aacbd809fbdc7576";//  GetSecretFromSecretIdentifier(subscriptionKeySecretIdentifier);
        }

        public static string GetTranslationKey()
        {
            return "40d19132f2224d82aa4fd3b947c11b80";// GetSecretFromSecretIdentifier(translationkeySecretIdentifier);
        }

        private static string GetSecretFromSecretIdentifier(string secretIdentifier)
        {
            try
            {
                EnsureClientInitialized();

                return keyVaultClient.GetSecretAsync(secretIdentifier)
                    .ConfigureAwait(false).GetAwaiter().GetResult().Value;
            }
            catch (Exception e)
            {
                Console.WriteLine(" NO ACCESS TO KEYVAULT SECRETS!!! ");
            }

            return "";
        }

        private static void EnsureClientInitialized()
        {
            if (keyVaultClient == null)
            {
                keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync), new HttpClient());
            }
        }

        private static async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
        { 
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);

            // Naredne dve linije koda mogu biti problem bez certa, njih zakomentarisati i odkomentarisati 2 linije ispod u slucaju toga
            IClientAssertionCertificate cert = new ClientAssertionCertificate(applicationId, GetCertificateWithSubjectFromStore());
            var result = await context.AcquireTokenAsync(resource, cert);

            // U slucaju problema sa certovima odkomentarisati sledece dve linije koda
            //var appCredentials = new ClientCredential("473fb467-aa96-4e5f-b70c-1e4296483756", "?I[]cTVYjnOR6x3OcI_Yd4EmVzt9KrL7");
            //var result = await context.AcquireTokenAsync(resource, appCredentials);

            return result.AccessToken;
        }

        private static X509Certificate2 GetCertificateWithSubjectFromStore(string subjectName = "CN=videofunapp")
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates;
                var currentCerts = certCollection.Find(X509FindType.FindBySubjectDistinguishedName, subjectName, false);

                if (currentCerts.Count == 0)
                {
                    throw new Exception("Certificate is not found.");
                }

                return currentCerts[0];
            }
        }
    }
}
