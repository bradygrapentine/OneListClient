using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ConsoleTables;


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
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            // var args = {};
            var token = "";
            if (args.Length == 0)
            {
                Console.Write("What list would you like? ");
                token = Console.ReadLine();
            }
            else
            {
                token = args[0];
            }
            var url = $"https://one-list-api.herokuapp.com/items?access_token={token}";
            //try:
            var responseAsStream = await client.GetStreamAsync(url);
            var items = await JsonSerializer.DeserializeAsync<List<Item>>(responseAsStream);
            var table = new ConsoleTable("Description", "Created At", "Completed");
            foreach (var item in items)
            {
                table.AddRow(item.Text, item.CreatedAt, item.CompletedStatus);
            }
            table.Write();
        }
    }
}
