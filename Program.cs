using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using CsvHelper;
using System.Globalization;
using System.Threading.Tasks;

namespace UserDataFetcher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the User Fetcher!");

            // Fetch the data from each API asynchronously
            var userFromAPI_1 = await FetchUsersFromAPI_1_Async();
            var userFromAPI_2 = await FetchUsersFromAPI_2_Async();
            var userFromAPI_3 = await FetchUsersFromAPI_3_Async();
            var userFromAPI_4 = await FetchUsersFromAPI_4_Async();

            // Combine all users from different sources
            var allUsers = new List<User>();
            allUsers.AddRange(userFromAPI_1);
            allUsers.AddRange(userFromAPI_2);
            allUsers.AddRange(userFromAPI_3);
            allUsers.AddRange(userFromAPI_4);

            // Save user data to file based on user input
            Console.WriteLine("Enter the folder path to save the file:");
            string folderPath = Console.ReadLine();
            Console.WriteLine("Enter the desired file format (JSON/CSV):");
            string fileFormat = Console.ReadLine()?.ToUpper();

            SaveUserDataToFile(allUsers, folderPath, fileFormat);

            // Display the total number of users 
            Console.WriteLine($"Total number of users: {allUsers.Count}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static async Task<List<User>> FetchUsersFromAPI_1_Async()
        {
            return new List<User>(); 
        }

        static async Task<List<User>> FetchUsersFromAPI_2_Async()
        {
            return new List<User>(); 
        }

        static async Task<List<User>> FetchUsersFromAPI_3_Async()
        {
            return new List<User>(); 
        }

        static async Task<List<User>> FetchUsersFromAPI_4_Async()
        {
            return new List<User>(); 
        }

        static void SaveUserDataToFile(List<User> users, string folderPath, string fileFormat)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, $"UserData.{fileFormat?.ToLower()}");

            switch (fileFormat?.ToUpper())
            {
                case "JSON":
                    SaveToJson(users, filePath);
                    break;
                case "CSV":
                    SaveToCsv(users, filePath);
                    break;
                default:
                    Console.WriteLine($"Unsupported file format: {fileFormat}");
                    break;
            }
        }

        static void SaveToJson(List<User> users, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true // Indent JSON for better readability
            };

            var json = JsonSerializer.Serialize(users, options);
            File.WriteAllText(filePath, json);
        }

        static void SaveToCsv(List<User> users, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(users);
            }
        }
    }

    class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SourceId { get; set; }
    }
}
