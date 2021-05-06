using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ConsoleTables;
using System.Text;
using System.Text.Unicode;
using System.Net.Http.Headers;

namespace OneListClient
{
    class Item
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } // if property names differ from JSON key names
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("complete")]
        public bool Complete { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } // Converts to DateTime automatically because JSON date formatting aligns with C# date formatting
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public string CompletedStatus
        {
            get
            {
                // ternary statement:
                // return   CONDITION   ? VALUE WHEN TRUE    :   VALUE WHEN FALSE
                return Complete ? "completed" : "not completed";
                // var someVar = complete ? 42: 180; // variable assignment with ternary stmt
                // identical to following IF/ELSE block
                // if (complete == true)
                // {
                //     return "completed";
                // }
                // else
                // {
                //     return "not completed";
                // }
            }
        }
    }
    class Program
    {
        static async Task SeeAllToDoListItemsAsync(string token)
        {
            var client = new HttpClient();
            var url = $"https://one-list-api.herokuapp.com/items?access_token={token}";
            var responseAsStream = await client.GetStreamAsync(url);
            var items = await System.Text.Json.JsonSerializer.DeserializeAsync<List<Item>>(responseAsStream);
            var table = new ConsoleTable("Description", "Created At", "Updated At", "Completed");
            Console.WriteLine();
            foreach (var item in items)
            {
                table.AddRow(item.Text, item.CreatedAt, item.UpdatedAt, item.CompletedStatus);
            }
            Console.WriteLine(table);
        }
        static async Task CreateToDoListItemAsync(string token)
        {
            var client = new HttpClient();
            Console.Write("What is the name of your new to-do list item? ");
            var name = Console.ReadLine();
            var newItem = new Item();
            newItem.Text = name;
            var jsonBody = JsonSerializer.Serialize(newItem);
            // We turn this into a StringContent object and indicate we are using JSON
            // by ensuring there is a media type header of `application/json`
            var jsonBodyAsContent = new StringContent(jsonBody);
            jsonBodyAsContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // var jsonBodyAsContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            // Send the POST request to the URL and supply the JSON body
            var url = $"https://one-list-api.herokuapp.com/items?access_token={token}";
            var response = await client.PostAsync(url, jsonBodyAsContent);
            // var updateInput = JsonConvert.SerializeObject(newItem);
            // // var buffer = System.Text.Encoding.UTF8.GetBytes(updateInput);s
            // // var byteContent = new ByteArrayContent(buffer);
            // // byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // var updateContent = new StringContent(updateInput, Encoding.UTF8, "application/json");
            // var url = $"https://one-list-api.herokuapp.com/items?access_token={token}";
            // // var responseAsStream = await client.PostAsync(url, updateContent).Result; This line throws an error
            // var responseAsStream = client.PostAsync(url, updateContent).Result;
            // // var result = await client.PostAsync(updateInput, byteContent).Result;
        }
        static async Task GetToDoListItemAsync(string token)
        {
            var client = new HttpClient();
            Console.Write("What is the id of the to-do list item that you'd like to see? ");
            var id = Console.ReadLine();
            var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";
            var responseAsStream = await client.GetStreamAsync(url);
            var item = await System.Text.Json.JsonSerializer.DeserializeAsync<Item>(responseAsStream);
            Console.WriteLine();
            var table = new ConsoleTable("Description", "Created At", "Updated At", "Completed");
            table.AddRow(item.Text, item.CreatedAt, item.UpdatedAt, item.CompletedStatus);
            Console.WriteLine(table);
        }
        static async Task DeleteToDoListItemAsync(string token)
        {
            var client = new HttpClient();
            Console.Write("What is the id of the to-do list item that you'd like to delete? ");
            var id = Console.ReadLine();
            var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";
            await client.DeleteAsync(url);
        }
        static async Task UpdateToDoListItemCompleteAsync(string token)
        {
            var client = new HttpClient();

            Console.Write("What is the id of the to-do list item that you'd like to mark complete? ");
            var id = Console.ReadLine();
            var updateInput = "{\"complete\": \"true\"}";
            var updateContent = new StringContent(updateInput, Encoding.UTF8, "application/json");
            var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";
            var responseAsStream = await client.PutAsync(url, updateContent);
            // could implement this like Add method is implemented
        }
        static async Task UpdateToDoListItemIncompleteAsync(string token)
        {
            var client = new HttpClient();

            Console.Write("What is the id of the to-do list item that you'd like to mark incomplete? ");
            var id = Console.ReadLine();
            var updateInput = "{\"complete\": \"false\"}";
            var updateContent = new StringContent(updateInput);
            updateContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // var updateContent = new StringContent(updateInput, Encoding.UTF8, "application/json");
            var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";
            var responseAsStream = await client.PutAsync(url, updateContent);
        }
        static string Menu()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("(S) See to-do list");
            Console.WriteLine("(A) Add to-do list item");
            Console.WriteLine("(G) Get to-do list item (must know item id)");
            Console.WriteLine("(D) Delete to-do list item (must know item id)");
            Console.WriteLine("(MC) Mark to-do list item complete (must know item id)");
            Console.WriteLine("(MI) Mark to-do list item incomplete (must know item id)");
            Console.WriteLine("(Q) Quit the application");
            Console.WriteLine();
            Console.Write("Select an option and press Enter: ");
            var choice = Console.ReadLine().ToUpper();
            return choice.ToUpper();
        }
        static async Task Main(string[] args)
        {

            Console.Clear();
            var token = "";
            if (args.Length == 0)
            {
                Console.Write("What list would you like? (Press Enter for Preset List) ");
                var choice = Console.ReadLine();
                if (choice == "")
                {
                    token = "brady_grapentine";
                }
                else
                {
                    token = choice;
                }
            }
            else
            {
                token = args[0];
            }
            var counter = 0;
            while (token != null)
            {
                var menuSelection = Menu();
                switch (menuSelection)
                {
                    case "S":
                        Console.Clear();
                        Console.WriteLine();
                        await SeeAllToDoListItemsAsync(token);
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    case "A":
                        Console.Clear();
                        await CreateToDoListItemAsync(token);
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    case "G":
                        Console.Clear();
                        await GetToDoListItemAsync(token);
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    case "D":
                        Console.Clear();
                        await DeleteToDoListItemAsync(token);
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    case "MC":
                        Console.Clear();
                        await UpdateToDoListItemCompleteAsync(token);
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    case "MI":
                        Console.Clear();
                        await UpdateToDoListItemIncompleteAsync(token);
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    case "Q":
                        Console.Clear();
                        Console.WriteLine("Closing application...");
                        token = null;
                        break;
                    default:
                        counter += 1;
                        if (counter >= 3)
                        {
                            Console.Clear();
                            Console.WriteLine("Closing application...");
                            token = null;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine();
                            Console.WriteLine("Try again");
                            Console.WriteLine();
                            break;
                        }
                        break;
                }
            }
            Console.WriteLine();
            Console.WriteLine("Goodbye");
            Console.WriteLine();
        }
    }
}
