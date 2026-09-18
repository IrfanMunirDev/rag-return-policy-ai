using Microsoft.AspNetCore.Mvc;
using ReturnPolicy.Models;
using ReturnPolicy.Services;

namespace ReturnPolicy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PolicyRagPromptStuffingController : ControllerBase
{
    private readonly PolicyRagPromptStuffingService _policyRagPromptStuffingService;

    public PolicyRagPromptStuffingController(PolicyRagPromptStuffingService policyRagPromptStuffingService)
    {
        _policyRagPromptStuffingService = policyRagPromptStuffingService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
    {
        string answer = await _policyRagPromptStuffingService.GetAnswerAsync(request.Question);
        return Ok(new { answer });
    }
}