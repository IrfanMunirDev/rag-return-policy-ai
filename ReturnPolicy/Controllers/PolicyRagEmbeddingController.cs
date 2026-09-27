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

    [HttpPost("stream")]
    public async Task StreamAsk(
        [FromBody] QuestionRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsync("Error: Question cannot be empty.", cancellationToken);
            return;
        }

        // Set content type for raw text streaming
        Response.ContentType = "text/plain; charset=utf-8";
        Response.Headers.Append("X-Content-Type-Options", "nosniff");

        //FlushAsync  is used to flush the response buffer to the client, ensuring that the client receives the data as it is when generated.
        await foreach (var token in _policyRagEmbeddingService.AnswerQuestionStreamAsync(request.Question, cancellationToken))
        {
            await Response.WriteAsync(token, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }

        /*
         1. The Myth of "Request, Response, Close"
         In a standard HTTP request (like your Ask endpoint):

         Client sends a request.

         Server does work, builds the entire response in memory, calculates a Content-Length header (e.g., "this response is exactly 450 bytes"), and sends it all back at once.

         The connection finishes.

         Streaming breaks this rule by omitting the Content-Length header and instead using a header called Transfer-Encoding: chunked.

         This tells the browser or Postman: "I have no idea how long this response is going to be, and I am not waiting until the end to send it. 
        I am going to keep this single HTTP connection open and push data to you in small pieces ('chunks'). 
        When I send an empty chunk at the very end, you'll know I'm finished."

         2. What is FlushAsync() actually doing?
         Normally, web servers try to be efficient. If your code produces a single word like "Hello", 
        the server doesn't immediately shoot it across the network socket because that causes a lot of network overhead. 
        Instead, it holds text in an output buffer until enough data builds up to make a efficient network packet.

         await Response.Body.FlushAsync(cancellationToken); tells ASP.NET Core:

         "Stop waiting for the buffer to fill up! Take whatever words are sitting in memory right now, 
        force them out of the network socket immediately, and push them down the wire to the client."

         This is what creates the instant typing effect. 
        Without .FlushAsync(), Ollama would generate words, 
        but they would sit trapped in the server's buffer until the entire paragraph finished generating—defeating the purpose of streaming.

         */
    }
}