using System;
using System.Data.SqlClient;
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
            string apiUrl = "http://universities.hipolabs.com/search?country=United+States";
            string connectionString = "Server=ANTHONY;Database=sample;Integrated Security=True;"; // Update with your actual SQL Server connection string

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Fetch data from API
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var universities = JsonSerializer.Deserialize<University[]>(responseBody);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        foreach (var university in universities)
                        {
                            string query = "INSERT INTO Universities (Name, Country, StateProvince, AlphaTwoCode, Website, Domain) " +
                                           "VALUES (@Name, @Country, @StateProvince, @AlphaTwoCode, @Website, @Domain)";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Name", university.name);
                                command.Parameters.AddWithValue("@Country", university.country);
                                command.Parameters.AddWithValue("@StateProvince", university.state_province ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@AlphaTwoCode", university.alpha_two_code);
                                command.Parameters.AddWithValue("@Website", university.web_pages?[0] ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Domain", university.domains?[0] ?? (object)DBNull.Value);

                                command.ExecuteNonQuery();
                            }
                        }

                        Console.WriteLine("Data inserted successfully into the database.");
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

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

