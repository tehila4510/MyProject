using Common.Dto.Chat;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Linq;
public class ChatService : IChatService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public ChatService(IConfiguration configuration, IHttpClientFactory factory)
    {
        _configuration = configuration;
        _httpClient = factory.CreateClient();
    }

    public async Task<object> AskTeacherAsync(UserRequest request)
    {
        var apiKey = _configuration["GeminiSettings:ApiKey"];

        var allMessages = request.History
            .Select(h => (object)new
            {
                role = h.Role,
                parts = new[] { new { text = h.Text } }
            })
            .ToList();

        allMessages.Add(new
        {
            role = "user",
            parts = new[] { new { text = request.Message } }
        });

        var requestBody = new
        {
            system_instruction = new
            {
                parts = new[]
                {
                    new
                    { 
                        text = "You are a patient English teacher." +
                        " Reply in English using simple language. " + 
                        "If the user makes a mistake, bold the correction and explain in Hebrew. " + 
                        "Always end with a follow-up question." 
                    }

                }
            },
            contents = allMessages
        };

        var jsonPayload = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite-preview:generateContent?key={apiKey}";
        
        var response = await _httpClient.PostAsync(url, content);
       response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(responseString);

        using var doc = JsonDocument.Parse(responseString);

        var botReply = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        var updatedHistory = request.History
            .Append(new ChatMessage { Role = "user", Text = request.Message })
            .Append(new ChatMessage { Role = "model", Text = botReply })
            .ToList();

        return new
        {
            reply = botReply,
            updatedHistory = updatedHistory
        };
    }
}