using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Transcription
{
    public class SecretProvider
    {
        private string translationkeySecretIdentifier;
        private string subscriptionKeySecretIdentifier;
        private string applicationId;

        public SecretProvider(string translationKeyId, string subscriptionKeyId, string applicationId)
        {
            this.applicationId = applicationId;
            this.translationkeySecretIdentifier = translationKeyId;
            this.subscriptionKeySecretIdentifier = subscriptionKeyId;
        }

        private KeyVaultClient keyVaultClient = null;
        private AzureServiceTokenProvider tokenProvider = new AzureServiceTokenProvider();

        public string GetSubscriptionKey()
        {
            return GetSecretFromSecretIdentifier(subscriptionKeySecretIdentifier);
        }

        public string GetTranslationKey()
        {
            return GetSecretFromSecretIdentifier(translationkeySecretIdentifier);
        }

        private string GetSecretFromSecretIdentifier(string secretIdentifier)
        {
            try
            {
                EnsureClientInitialized();

                return keyVaultClient.GetSecretAsync(secretIdentifier)
                    .ConfigureAwait(false).GetAwaiter().GetResult().Value;
            }
            catch (Exception)
            {
                Console.WriteLine(" NO ACCESS TO KEYVAULT SECRETS!!! ");
                throw;
            }
        }

        private void EnsureClientInitialized()
        {
            if (keyVaultClient == null)
            {
                keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync), new HttpClient());
            }
        }

        private async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
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

        private X509Certificate2 GetCertificateWithSubjectFromStore(string subjectName = "CN=videofunapp")
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
