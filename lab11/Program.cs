using HtmlAgilityPack;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        // URL of the website to scrape
        string baseUrl = "https://uaserials.pro/films";
        List<string> movieTitlesCollection = new List<string>();

        // Download and parse the first 4 pages in parallel
        Parallel.Invoke(
            () => GetMovieTitles(baseUrl, 1, movieTitlesCollection),
            () => GetMovieTitles(baseUrl, 2, movieTitlesCollection),
            () => GetMovieTitles(baseUrl, 3, movieTitlesCollection),
            () => GetMovieTitles(baseUrl, 4, movieTitlesCollection)
        );

        // Display the movie titles
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Movie Titles:");
        foreach (var title in movieTitlesCollection)
        {
            Console.WriteLine(title);
        }
    }

    static void GetMovieTitles(string baseUrl, int pageNumber, List<string> movieCollection)
    {
        // Construct the URL for the specific page
        string url = $"{baseUrl}/page/{pageNumber}";

        // Create HttpClient instance
        using var httpClient = new HttpClient();

        // Send GET request to the URL
        var response = httpClient.GetAsync(url).Result;

        // Check if the request was successful
        if (response.IsSuccessStatusCode)
        {
            // Read the content of the page
            var content = response.Content.ReadAsStringAsync().Result;

            // Load HTML document
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            // Extract movie titles from the HTML document
            var titles = doc.DocumentNode.SelectNodes("//div[contains(@class, 'th-title') and contains(@class, 'truncate')]");

            // Add movie titles to the list
            if (titles != null)
            {
                foreach (var titleNode in titles)
                {
                    movieCollection.Add(titleNode.InnerText.Trim());
                }
            }
        }
        else
        {
            Console.WriteLine($"Failed to fetch page {pageNumber}");
        }
    }
}
