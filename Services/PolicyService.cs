using Microsoft.Extensions.AI;

namespace ReturnPolicy.Services;

public class PolicyService
{
    private readonly IChatClient _chatClient;
    private readonly string _policyPath;

    public PolicyService(IChatClient chatClient)
    {
        _chatClient = chatClient;
        _policyPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "return_policy.txt");
    }

    public async Task<string> GetAnswerAsync(string userQuestion)
    {
        if (!File.Exists(_policyPath))
            throw new FileNotFoundException("Return policy file not found.", _policyPath);

        var policyText = await File.ReadAllTextAsync(_policyPath);

        List<ChatMessage> messages = new()
        {
            new ChatMessage(ChatRole.System, "You are a helpful customer support assistant specializing in return policies."),
            new ChatMessage(ChatRole.System, $"Here is the company's return policy:\n\n{policyText}"),
            new ChatMessage(ChatRole.User, userQuestion)
        };

        var response = await _chatClient.GetResponseAsync(messages);

        return response.Text?.Trim() ?? string.Empty;
    }
}