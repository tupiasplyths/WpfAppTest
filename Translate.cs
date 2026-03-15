using System.Net.Http;
using System.Text;
using System.Text.Json;
using DeepL;

namespace WpfAppTest;

public static class Translate
{
    private static readonly HttpClient client = new();
    private static readonly string? apiKeys;
    private static readonly string? deepLApiKey;
    private static Translator? translator;
    private static readonly string ollamaBaseUrl;


    static Translate()
    {
        _ = DotNetEnv.Env.Load();
        deepLApiKey = Environment.GetEnvironmentVariable("DEEPL_API_KEY");
        apiKeys = Environment.GetEnvironmentVariable("GOOGLE_API_KEYS");
        ollamaBaseUrl = Environment.GetEnvironmentVariable("OLLAMA_BASE_URL") ?? "http://localhost:11434";
        if (deepLApiKey != null)
            translator = new Translator(deepLApiKey);
    }
    /// <summary>
    /// Translates text from Japanese to English using the Google Translate API.
    /// </summary>
    /// <param name="text">The text to translate.</param>
    /// <returns>The translated text, or "Translation Failed" if the translation fails.</returns>
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
        Console.WriteLine(translatedText);
        return translatedText;
    }

    /// <summary>
    /// Translates text using the DeepL API.
    /// </summary>
    /// <param name="text">The text to translate.</param>
    /// <returns>The translated text, or null if an error occurred.</returns>
    public static string DeepLTranslate(string text)
    {
        if (translator == null)
        {
            Console.WriteLine("Translator is null");
            return "";
        }
        Console.WriteLine(text);
        var translatedText = translator.TranslateTextAsync(text, LanguageCode.Japanese, LanguageCode.EnglishAmerican).Result;

        return translatedText.ToString();
    }

    /// <summary>
    /// Translates text using Ollama's gemma3:1b model.
    /// </summary>
    /// <param name="text">The text to translate.</param>
    /// <returns>The translated text, or "Translation Failed" if an error occurred.</returns>
    public static string OllamaTranslate(string text)
    {
        try
        {
            var requestBody = new
            {
                model = "gemma3:1b",
                prompt = $"Translate the following Japanese text to English. Only provide the English translation, no explanations:\n\n{text}",
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync($"{ollamaBaseUrl}/api/generate", content).Result;
            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadAsStringAsync().Result;
            var jsonDocument = JsonDocument.Parse(result);
            var translatedText = jsonDocument.RootElement.GetProperty("response").GetString();

            if (string.IsNullOrWhiteSpace(translatedText))
            {
                Console.WriteLine("Ollama translation returned empty response");
                return "Translation Failed";
            }

            // Clean up the response - remove any extra whitespace
            translatedText = translatedText.Trim();
            Console.WriteLine($"Ollama: {translatedText}");
            return translatedText;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ollama translation error: {ex.Message}");
            return "Translation Failed";
        }
    }
}
