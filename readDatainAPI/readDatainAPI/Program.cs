using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace UniversityAPIClient
{
    // Define a class that matches the structure of the JSON data
    public class University
    {
        public string[] web_pages { get; set; }
        public string state_province { get; set; }
        public string alpha_two_code { get; set; }
        public string country { get; set; }
        public string name { get; set; }
        public string[] domains { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // Define the API URL
            string apiUrl = "http://universities.hipolabs.com/search?country=United+States";

            // Use HttpClient to send a GET request to the API
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode(); // Ensure a successful response

                    // Read the response as a string
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response into a list of University objects
                    var universities = JsonSerializer.Deserialize<University[]>(responseBody);

                    // Iterate through each university in the response
                    foreach (var university in universities)
                    {
                        Console.WriteLine($"Name: {university.name}");
                        Console.WriteLine($"Country: {university.country}");
                        Console.WriteLine($"State/Province: {university.state_province ?? "N/A"}");
                        Console.WriteLine($"Alpha-2 Code: {university.alpha_two_code}");
                        Console.WriteLine($"Website: {university.web_pages?[0]}");
                        Console.WriteLine($"Domain: {university.domains?[0]}");
                        Console.WriteLine();
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error: " + e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            // Wait for the user to close the console
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
