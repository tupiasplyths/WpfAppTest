
using System.Net.Http;
using System.Text.Json;
namespace WpfAppTest;

public static class Translate
{
    public static string GetTranslation(string text)
    {
        _ = DotNetEnv.Env.Load();
        using HttpClient client = new();
        string? apiKeys = System.Environment.GetEnvironmentVariable("GOOGLE_API_KEYS");

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
}