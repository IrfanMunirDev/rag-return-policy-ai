namespace ReturnPolicy.Infrastructure.Entities;

public class PolicyChunkEntity
{
    public int Id { get; set; }

    public string Text { get; set; }

    public byte[] Embedding { get; set; }
}