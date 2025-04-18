using System.Net.Http.Json;

using HttpClient client = new();
client.BaseAddress = new("http://localhost:5299");

while (true)
{
    Console.WriteLine("Question:");
    var question = Console.ReadLine();
    var msg = client.GetFromJsonAsync<string>($"/chat?question={question}");
    Console.Write(msg);
    
   
    Console.WriteLine();
}

