using Microsoft.AspNetCore.Mvc;
using ReturnPolicy.Models;
using ReturnPolicy.Services;

namespace ReturnPolicy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PolicyRagEmbeddingController : ControllerBase
{
    private readonly PolicyRagEmbeddingService _policyRagEmbeddingService;

    public PolicyRagEmbeddingController(PolicyRagEmbeddingService policyService)
    {
        _policyRagEmbeddingService = policyService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
    {
        string answer = await _policyRagEmbeddingService.AnswerQuestionAsync(request.Question);
        return Ok(new { answer });
    }
}