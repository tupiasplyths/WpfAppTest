using System.Text.RegularExpressions;

namespace WpfAppTest;

/// <summary>
/// Provides functionality to look up furigana (readings) for Japanese text.
/// </summary>
public static class FuriganaLookup
{
    /// <summary>
    /// Gets furigana readings for words in the given Japanese text.
    /// </summary>
    /// <param name="text">The Japanese text to analyze.</param>
    /// <returns>A list of JapaneseWord objects containing word and reading pairs.</returns>
    public static List<JapaneseWord> GetFuriganaForText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        var results = new List<JapaneseWord>();
        var processedWords = new HashSet<string>();

        // Split text into potential words (by common Japanese delimiters)
        var words = ExtractJapaneseWords(text);

        foreach (var word in words)
        {
            // Skip if already processed or if it's pure kana (doesn't need furigana)
            if (processedWords.Contains(word) || IsPureKana(word))
            {
                continue;
            }

            processedWords.Add(word);

            // Look up the word in the dictionary
            var lookupResults = WWWJDict.GetSearchResults(word);

            if (lookupResults.Count > 0)
            {
                // Take the first result as the most likely match
                var firstResult = lookupResults[0];
                results.Add(firstResult);
            }
            else
            {
                // If no dictionary result found, add the word without reading
                results.Add(new JapaneseWord(word, "", ["No reading found"]));
            }
        }

        return results;
    }

    /// <summary>
    /// Extracts Japanese words from text, handling various Japanese text patterns.
    /// </summary>
    private static List<string> ExtractJapaneseWords(string text)
    {
        var words = new List<string>();

        // Remove common punctuation and whitespace
        text = text.Replace("\n", " ").Replace("\r", " ").Replace("　", " ");

        // Split by spaces and common delimiters
        var splitChars = new[] { ' ', '、', '。', '！', '？', '「', '」', '『', '』', '（', '）', '(', ')' };
        var segments = text.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in segments)
        {
            var trimmed = segment.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                // For longer segments, try to break them down further if needed
                if (trimmed.Length > 10)
                {
                    // Try to find sub-words using a simple approach
                    var subWords = BreakDownLongSegment(trimmed);
                    words.AddRange(subWords);
                }
                else
                {
                    words.Add(trimmed);
                }
            }
        }

        return words;
    }

    /// <summary>
    /// Breaks down a long text segment into smaller word candidates.
    /// </summary>
    private static List<string> BreakDownLongSegment(string segment)
    {
        var words = new List<string>();

        // Simple approach: try different lengths starting from the segment
        // and also add the full segment
        words.Add(segment);

        // Try to find kanji compounds (2-4 characters)
        for (int i = 0; i < segment.Length; i++)
        {
            for (int len = 2; len <= 4 && i + len <= segment.Length; len++)
            {
                var subWord = segment.Substring(i, len);
                if (ContainsKanji(subWord))
                {
                    words.Add(subWord);
                }
            }
        }

        return words.Distinct().ToList();
    }

    /// <summary>
    /// Checks if the text contains any kanji characters.
    /// </summary>
    private static bool ContainsKanji(string text)
    {
        foreach (char c in text)
        {
            if (IsKanji(c))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the text is purely kana (hiragana/katakana) - no kanji.
    /// </summary>
    private static bool IsPureKana(string text)
    {
        foreach (char c in text)
        {
            if (IsKanji(c))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if a character is a kanji.
    /// </summary>
    private static bool IsKanji(char c)
    {
        // CJK Unified Ideographs: 4E00-9FFF
        // CJK Unified Ideographs Extension A: 3400-4DBF
        // CJK Compatibility Ideographs: F900-FAFF
        return (c >= '\u4E00' && c <= '\u9FFF') ||
               (c >= '\u3400' && c <= '\u4DBF') ||
               (c >= '\uF900' && c <= '\uFAFF');
    }
}
