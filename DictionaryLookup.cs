using System.Net.Http;
using System.Text.RegularExpressions;
namespace WpfAppTest;

public static partial class WWWJDict
{
    private static readonly HttpClient client = new();
    private static readonly string BaseUrl = "http://www.edrdg.org/cgi-bin/wwwjdic/wwwjdic?1ZUJ";

    public static List<string> LookUp(string searchKeys)
    {
        string url = BaseUrl + searchKeys;
        List<string> resultsList = [];
        try
        {
            Task<HttpResponseMessage> responseTask = client.GetAsync(url);
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result;
            response.EnsureSuccessStatusCode();

            Task<string> readTask = response.Content.ReadAsStringAsync();
            readTask.Wait();
            string responseBody = readTask.Result;

            int preStart = responseBody.IndexOf("<pre>");
            int preEnd = responseBody.IndexOf("</pre>");

            if (preStart >= 0 && preEnd > preStart)
            {
                string preContent = responseBody.Substring(preStart + 5, preEnd - preStart - 5);

                string[] entries = preContent.Split('\n');

                int count = Math.Min(4, entries.Length);
                for (int i = 0; i < count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(entries[i]))
                    {
                        resultsList.Add(entries[i].Trim());
                        Console.WriteLine(entries[i].Trim());
                    }
                }
            }
            else
            {
                resultsList.Add("No dictionary entries found.");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            resultsList.Add("Error during lookup: " + e.Message);
        }

        return resultsList;
    }

    public static List<JapaneseWord> GetSearchResults(string inputString)
    {
        if (string.IsNullOrWhiteSpace(inputString))
        {
            Console.WriteLine("Input string is empty or whitespace.");
            return [];
        }
        var results = LookUp(inputString).ToString();

        string[] lines = results.Trim().Split('\n');
        List<JapaneseWord> japaneseWords = [];

        foreach (string line in lines)
        {
            string[] parts = line.Split(new[] { " [" }, StringSplitOptions.None);
            if (parts.Length < 2)
            {
                continue;
            }

            string word = parts[0].Trim();
            string[] readingAndMeanings = parts[1].Split(new[] { "]" }, StringSplitOptions.None);
            string reading = readingAndMeanings[0].Trim();
            string meaningsString = readingAndMeanings[1].Trim().TrimStart('/');

            string[] meanings = meaningsString.Split('/');
            List<string> processedMeanings = [];

            for (int i = 0; i < meanings.Length; i++)
            {
                string meaning = meanings[i].Trim();
                meaning = MyRegex().Replace(meaning, "");
                meaning = MyRegex1().Replace(meaning, "");
                if (!string.IsNullOrEmpty(meaning))
                {
                    processedMeanings.Add(meaning);
                }
            }

            japaneseWords.Add(new JapaneseWord(word, reading, processedMeanings));
        }

        return japaneseWords;
    }

    [GeneratedRegex(@"^\(.*?\)\s*")]
    private static partial Regex MyRegex();
    [GeneratedRegex(@"^\(\d+\)\s*")]
    private static partial Regex MyRegex1();
}

public class JapaneseWord(string word, string reading, List<string> meanings)
{
    public string Word { get; set; } = word;
    public string Reading { get; set; } = reading;
    public List<string> Meanings { get; set; } = meanings;
}