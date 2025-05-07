using System.Net.Http;

namespace WpfAppTest;

public static class WWWJDict
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

                int count = Math.Min(2, entries.Length);
                for (int i = 0; i < count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(entries[i]))
                    {
                        resultsList.Add(entries[i].Trim());
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
}