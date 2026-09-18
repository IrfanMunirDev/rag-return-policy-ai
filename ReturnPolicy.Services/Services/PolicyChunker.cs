namespace ReturnPolicy.Services;

public class PolicyChunker
{
    public List<string> ChunkText(string text, int maxChunkLength = 500)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new List<string>();
        }

        // Split text by double newlines or standard paragraph boundaries
        var paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();

        foreach (var paragraph in paragraphs)
        {
            var trimmed = paragraph.Trim();
            if (trimmed.Length <= maxChunkLength)
            {
                chunks.Add(trimmed);
            }
            else
            {
                // If a paragraph is too long, split by sentences or chunks
                for (int i = 0; i < trimmed.Length; i += maxChunkLength)
                {
                    int length = Math.Min(maxChunkLength, trimmed.Length - i);
                    chunks.Add(trimmed.Substring(i, length).Trim());
                }
            }
        }

        return chunks;
    }
}