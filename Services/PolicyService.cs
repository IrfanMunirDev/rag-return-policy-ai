using OpenAI;
using OpenAI.Chat;

namespace ReturnPolicy.Services;

public class PolicyService
{
    private readonly ChatClient _chatClient;
    private readonly string _policyPath;

    public PolicyService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Missing OpenAI:ApiKey in configuration.");

        var client = new OpenAIClient(apiKey);
        _chatClient = client.GetChatClient("gpt-4o-mini");

        _policyPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "return_policy.txt");
    }

    public async Task<string> GetAnswerAsync(string userQuestion)
    {
        if (!File.Exists(_policyPath))
            throw new FileNotFoundException("Return policy file not found.", _policyPath);

        var policyText = await File.ReadAllTextAsync(_policyPath);

        List<ChatMessage> messages = new()
        {
            ChatMessage.CreateSystemMessage("You are a helpful customer support assistant specializing in return policies."),
            ChatMessage.CreateSystemMessage($"Here is the company's return policy:\n\n{policyText}"),
            ChatMessage.CreateUserMessage(userQuestion)
        };

        var result = await _chatClient.CompleteChatAsync(messages);

        return result.Value.Content[0].Text.Trim();
    }
}
