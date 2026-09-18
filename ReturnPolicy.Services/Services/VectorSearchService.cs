using System.Numerics.Tensors;

namespace ReturnPolicy.Services;

public class VectorSearchService
{
    //Traditional keyword search (like `SQL LIKE '%return%'`) fails if the user types a word that isn't explicitly in your text file.
    //**Vector search understands intent and meaning**, so even if the user uses completely different phrasing,
    //it will find the right paragraph because the *coordinates* on the meaning map line up!

    public string? FindMostRelevantChunk(ReadOnlyMemory<float> queryVector, List<ChunkEmbedding> storedEmbeddings)
    {
        // 1. "Are there any boxes on our map at all?"
        // NOTE 1: Safety Guard Clause
        // If there are no stored chunks in the database/list, we can't search anything. 
        // Returning null prevents crashes or out-of-bounds exceptions.
        if (storedEmbeddings == null || storedEmbeddings.Count == 0)
        {
            return null;
        }

        string? bestChunk = null;

        // 2. "Set our current closest distance record to 'infinitely far away' 
        // so the very first box we measure automatically becomes our temporary winner."
        // NOTE 2: Tracking the Highest Score
        // We initialize maxSimilarity to the absolute lowest possible float value. 
        // This ensures that the very first similarity score computed will be higher than this, 
        // guaranteeing we always pick a valid starting chunk.
        float maxSimilarity = float.MinValue;

        // 3. "Walk through every single policy paragraph box stored in our database..."
        // NOTE 3: Loop Through All Stored Document Chunks
        // We look at every single paragraph/chunk we embedded earlier to see how close 
        // its meaning matches the user's question.
        foreach (var item in storedEmbeddings)
        {
            // 4. "Take our tape measure (TensorPrimitives.CosineSimilarity) and measure 
            // the angle/distance between the user's GPS pin and this paragraph's GPS coordinates."
            // NOTE 4: Cosine Similarity Calculation via .NET 10 Primitives
            // 'TensorPrimitives.CosineSimilarity' is a built-in, highly optimized mathematical function. 
            // It compares the direction of two arrays of numbers (vectors) in multi-dimensional space.
            // A score closer to 1.0 means the vectors point in the same direction (high semantic match).
            // A score closer to 0 or negative means they are unrelated.
            float similarity = TensorPrimitives.CosineSimilarity(queryVector.Span, item.Vector.Span);


            // 5. "Is this paragraph closer than our previous winner? 
            // If yes, throw away the old winner and crown this new paragraph!"
            // NOTE 5: Evaluate and Keep the Best Winner
            // If the current chunk's score beats our previous high score, 
            // we update our tracker and save this chunk's text.
            if (similarity > maxSimilarity)
            {
                maxSimilarity = similarity;
                bestChunk = item.Chunk;
            }
        }

        // 6. "Hand back the text of the paragraph that was closest to the user's pin."
        // NOTE 6: Return the Winner
        // Returns the exact string text of the chunk that had the highest mathematical alignment with the question.
        return bestChunk;
    }
}

/*
 
The Analogy: The "Concept GPS Map"
Imagine you own a massive warehouse filled with thousands of policy paragraphs.

The Mapmaker (nomic-embed-text):
Before anyone asks anything, you take every single paragraph in your return policy and hand it to a smart Mapmaker (the embedding model). 
The Mapmaker reads a paragraph like "You can return items within 30 days if unused" and places it on a giant GPS Map of Meanings. On this map, distance equals meaning.

Sentences about money and refunds live in the North-West corner.

Sentences about shipping damage live in the South-East corner.

The Pin (queryVector):
When a user types a question into Postman—for example, "Can I get my cash back if I don't want this?"—your system hands that question to the Mapmaker. 
The Mapmaker drops a GPS pin onto that exact same Concept Map. Even though the user used the words "cash back" instead of "refund", the Mapmaker's brain knows what cash means, 
so it drops the pin right next to your refund paragraphs in the North-West corner.

The Search (VectorSearchService):
Now, how does your C# code find the right paragraph? It doesn't look at words; it literally acts like a tape measure or a compass starting from the user's GPS pin, 
checking every single stored paragraph box in your database to see which one is closest.
 
 */
/*
 Understanding vectors and embeddings for the first time can feel like learning a magic trick, but once you see the underlying analogy, 
it clicks into place.

Here is a high-level, plain-English breakdown of what embeddings are and what your `VectorSearchService` is actually doing.

---

### 1. The Core Idea: Turning Text into Coordinates

Computers don’t understand English; they only understand numbers.

* **Embeddings** are a way to translate words, sentences, or entire paragraphs into a long list of numbers (a **vector**, like `[0.25, -0.41, 0.88, ...]`).
* Think of these numbers as **coordinates on a giant, multi-dimensional "Meaning Map."**
* On this map, sentences with **similar meanings are placed close to each other**, while unrelated sentences are placed far apart.
* For example, the sentence *"Can I get my money back?"* and *"What is your refund policy?"* will end up with coordinates that sit right next to each other on the map, even though they share very few actual words.



---

### 2. What is Cosine Similarity? (Measuring the Angle)

Once your return policy paragraphs are converted into coordinates (vectors) and stored, a user comes along and asks a question.

1. Your system converts the user's question into coordinates too.
2. Now you have a coordinate for the **Question** and coordinates for every **Paragraph** in your policy document.
3. How do you mathematically check which paragraph matches the question? You measure the **angle between their arrows** from the center of the map.
* If two arrows point in almost the exact same direction, their **Cosine Similarity** score is close to `1.0` (a direct match in meaning).
* If they point in completely different directions, the score drops toward `0` or negative numbers (unrelated).



---

### 3. What Your Code is Doing (Step-by-Step)

If you look at `VectorSearchService` through this high-level lens, it is simply running a **"Closest Arrow Finder" game**:

1. **The Check (`if (storedEmbeddings == null)`):** Makes sure you actually have paragraphs on your map before you try to search.
2. **The Tracker (`float maxSimilarity = float.MinValue`):** Sets up a temporary "highest score wins" trophy. We start it at the worst possible score so the first paragraph we check automatically takes the crown.
3. **The Loop (`foreach`):** Walks through your database one paragraph at a time.
4. **The Math (`TensorPrimitives.CosineSimilarity`):** Acts like a mathematical protractor, measuring the angle between the **User's Question** and the **Current Paragraph**.
5. **The Crown Swap (`if (similarity > maxSimilarity)`):** If the current paragraph has a higher score (closer angle) than the previous winner, we throw away the old winner and remember this new one.
6. **The Result (`return bestChunk`):** Hands back the single paragraph text that pointed closest to the user's question.

### Why do this instead of normal search?

Traditional keyword search (like `SQL LIKE '%return%'`) fails if the user types a word that isn't explicitly in your text file. **Vector search understands intent and meaning**, so even if the user uses completely different phrasing, it will find the right paragraph because the *coordinates* on the meaning map line up!
 */