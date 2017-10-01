/* This class get a result from Translator Text API
 * Tryout page:
 * http://docs.microsofttranslator.com/text-translate.html#!/default/get_Translate
 */
using System;
using System.Net.Http;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace JavaKucheSample
{
    public class TranslateClient
    {
        const string uriBase = @"https://api.microsofttranslator.com/v2/http.svc/Translate";

        public static async Task<string> TranslateTextAsync(string rowText, string from, string to)
        {
            var token = await AuthenticationToken.GetBearerTokenAsync(Secrets.TranslatorTextApiKey);

            using (var client = new HttpClient())
            {
                var sendUri = $@"{uriBase}?appid={token}&text={rowText}&from={from}&to={to}&category=generalnn";
                var stream = await client.GetStreamAsync(sendUri);
                if (stream == null)
                    throw new Exception("Could not translated.");

                var doc = XElement.Load(stream);
                return doc.Value;
            }
        }
    }

    public class AuthenticationToken : IDisposable
    {
        private const string tokenUri = @"https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
        private static readonly HttpClient client;

        static AuthenticationToken()
        {
            client = new HttpClient();
        }

        public static async Task<string> GetBearerTokenAsync(string apiKey)
        {
            var token = await GetTokenAsync(apiKey);
            //"Bearer "を付与してリターンしてます。
            return $"Bearer {token}";
        }

        private static async Task<string> GetTokenAsync(string apiKey)
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            var resuponse = await client.PostAsync(tokenUri, new StringContent(string.Empty));
            return await resuponse.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {
            //TODO Dispose support !
        }
    }
}
