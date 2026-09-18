namespace ReturnPolicy.Core.Models;

//model is a record not a class, so user can not change the data, readonly fields
public record ChunkEmbedding(string Chunk, ReadOnlyMemory<float> Vector);