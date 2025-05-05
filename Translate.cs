
using System.Net.Http;
using System.Text.Json;
using DeepL;
namespace WpfAppTest;


public static class Translate
{
    private static readonly HttpClient client = new();
    private static readonly string? apiKeys;
    private static readonly string? deepLApiKey;
    private static Translator? translator;


    static Translate()
    {
        _ = DotNetEnv.Env.Load();
        deepLApiKey = Environment.GetEnvironmentVariable("DEEPL_API_KEY");
        apiKeys = Environment.GetEnvironmentVariable("GOOGLE_API_KEYS");
        if (deepLApiKey != null)
            translator = new Translator(deepLApiKey);
    }
    public static string GetTranslation(string text)
    {


        var jsonDict = new Dictionary<string, string>()
        {
            { "q", text },
            { "source", "ja" },
            { "target", "en" },
            { "format", "text" }
        };

        var content = new FormUrlEncodedContent(jsonDict);
        var response = client.PostAsync($"https://translation.googleapis.com/language/translate/v2?key={apiKeys}", content).Result;
        var result = response.Content.ReadAsStringAsync().Result;
        var jsonDocument = JsonDocument.Parse(result);
        var translatedText = jsonDocument.RootElement.GetProperty("data")
                            .GetProperty("translations")[0]
                            .GetProperty("translatedText")
                            .GetString() ?? "Translation Failed";
        return translatedText;
    }

    public static string DeepLTranslate(string text)
    {
        if (translator == null)
        {
            Console.WriteLine("Translator is null");
            return "";
        }
        var translatedText = translator.TranslateTextAsync(text, LanguageCode.Japanese, LanguageCode.EnglishAmerican).Result;

        return translatedText.ToString();
    }
}