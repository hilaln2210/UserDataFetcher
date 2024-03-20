using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace UserDataFetcher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the User Data Fetcher!");

            // Define API endpoints
            string[] apiEndpoints = {
                "https://randomuser.me/api/?results=500",
                "https://jsonplaceholder.typicode.com/users",
                "https://dummyjson.com/users",
                "https://reqres.in/api/users"
            };

            List<User> allUsers = new List<User>();

            // Fetch users from each API endpoint
            foreach (var endpoint in apiEndpoints)
            {
                try
                {
                    List<User> users = await FetchUsersFromApi(endpoint);
                    if (users != null)
                    {
                        allUsers.AddRange(users);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to fetch users from {endpoint}: {ex.Message}");
                }
            }

            // Prompt user for folder path and file format
            Console.WriteLine("Enter the folder path to save the file:");
            string folderPath = Console.ReadLine();

            Console.WriteLine("Enter the desired file format (JSON/CSV):");
            string fileFormat = Console.ReadLine().ToUpper();

            // Save users to file
            SaveUserDataToFile(allUsers, folderPath, fileFormat);

            // Display total number of users
            Console.WriteLine($"Total number of users: {allUsers.Count}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static async Task<List<User>> FetchUsersFromApi(string endpoint)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(endpoint);
                response.EnsureSuccessStatusCode(); // Throw exception if not successful

                string responseBody = await response.Content.ReadAsStringAsync();

                if (endpoint.Contains("randomuser.me"))
                {
                    RandomUserResponse randomUserResponse = JsonSerializer.Deserialize<RandomUserResponse>(responseBody);
                    return ConvertToUserList(randomUserResponse);
                }
                else if (endpoint.Contains("jsonplaceholder.typicode.com"))
                {
                    return JsonSerializer.Deserialize<List<User>>(responseBody);
                }
                else if (endpoint.Contains("dummyjson.com"))
                {
                    DummyJsonResponse dummyJsonResponse = JsonSerializer.Deserialize<DummyJsonResponse>(responseBody);
                    return ConvertToUserList(dummyJsonResponse);
                }
                else if (endpoint.Contains("reqres.in"))
                {
                    ReqresResponse reqresResponse = JsonSerializer.Deserialize<ReqresResponse>(responseBody);
                    return ConvertToUserList(reqresResponse);
                }
                else
                {
                    throw new Exception("Unknown API endpoint");
                }
            }
        }

        static List<User> ConvertToUserList(RandomUserResponse response)
        {
            List<User> users = new List<User>();
            foreach (var result in response.results)
            {
                users.Add(new User
                {
                    FirstName = result.name.first,
                    LastName = result.name.last,
                    Email = result.email,
                    SourceId = 1 // Assuming source ID for randomuser.me is 1
                });
            }
            return users;
        }

        static List<User> ConvertToUserList(DummyJsonResponse response)
        {
            List<User> users = new List<User>();
            foreach (var user in response.users)
            {
                users.Add(new User
                {
                    FirstName = user.firstName,
                    LastName = user.lastName,
                    Email = user.email,
                    SourceId = 2 // Assuming source ID for jsonplaceholder.typicode.com is 2
                });
            }
            return users;
        }

        // Renamed method to avoid duplicate signature
        static List<User> ConvertToUserListFromDummyJson(DummyJsonResponse response)
        {
            List<User> users = new List<User>();
            foreach (var user in response.users)
            {
                users.Add(new User
                {
                    FirstName = user.firstName,
                    LastName = user.lastName,
                    Email = user.email,
                    SourceId = 3 // Assuming source ID for dummyjson.com is 3
                });
            }
            return users;
        }

        static List<User> ConvertToUserList(ReqresResponse response)
        {
            List<User> users = new List<User>();
            foreach (var user in response.data)
            {
                users.Add(new User
                {
                    FirstName = user.first_name,
                    LastName = user.last_name,
                    Email = user.email,
                    SourceId = 4 // Assuming source ID for reqres.in is 4
                });
            }
            return users;
        }

        static void SaveUserDataToFile(List<User> users, string folderPath, string fileFormat)
        {
            string filePath = Path.Combine(folderPath, $"Users.{fileFormat.ToLower()}");

            switch (fileFormat.ToUpper())
            {
                case "JSON":
                    string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, json);
                    break;
                case "CSV":
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine("First Name,Last Name,Email,Source Id");
                        foreach (var user in users)
                        {
                            writer.WriteLine($"{user.FirstName},{user.LastName},{user.Email},{user.SourceId}");
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Invalid file format specified.");
                    break;
            }
        }
    }

    public class User
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int SourceId { get; set; }
    }

    // Models for different API responses

    public class RandomUserResponse
    {
        public List<Result> results { get; set; }
    }

    public class Result
    {
        public Name name { get; set; }
        public string email { get; set; }
    }

    public class Name
    {
        public string first { get; set; }
        public string last { get; set; }
    }

    public class DummyJsonResponse
    {
        public List<DummyUser> users { get; set; }
    }

    public class DummyUser
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
    }

    public class ReqresResponse
    {
        public List<UserData> data { get; set; }
    }

    public class UserData
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
    }
}
