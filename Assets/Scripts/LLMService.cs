using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;
using Meta.WitAi.TTS.Utilities;
using TMPro;
using UnityEngine.UI;

public class LLMService : MonoBehaviour
{
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
    private string apiKey = ""; 
    public EventHandler eventHandler;
    [SerializeField] public Stroke stroke;
    [SerializeField] public KidneyStones kidneyStones;


    [SerializeField] public TTSSpeaker tts;
    public System.Action<string, string> OnOrganAnswerReceived; 
    public TextMeshPro responseTextContainer;
    public TextMeshPro otherResponseContainer;


    public async Task AskAnatomyQuestion(string question, string organ = "")
    {
        Debug.Log(question);
        string prompt = $@"
- Question: {question}

Select an organ from the list:
[bones, blood_vessels, bladder, brain, bronchi, eyes, heart, kidney, large_intestine, liver, lungs, skin, small_intestine, spine, spleen, stomach, thyroid]

Format the response as JSON:

{{
    ""organ"": ""<detected_or_selected_organ>"",
    ""answer"": ""<your_answer>""
}}

Return only the JSON object and nothing else.

If you think the question is about one of the diseases on this list, put that disease into the organ field instead.
[Stroke,kidney-stones]


In the answer, never add any special characters, this means only alphabetical and punctuation STRICTLY.
MAKE SURE your answer is NO MORE than 30 words.
";
        Debug.Log(prompt);

        string response = await QueryGemini(prompt);
        ParseResponse(response);
    }

    private async Task<string> QueryGemini(string prompt)
    {
        // Escape special JSON characters in the prompt
        string escapedPrompt = EscapeJsonString(prompt);
        string jsonPayload = "{\"contents\": [{\"parts\": [{\"text\": \"" + escapedPrompt + "\"}]}]}";
        Debug.Log(jsonPayload);
        
        using (UnityWebRequest request = new UnityWebRequest(apiUrl + "?key=" + apiKey, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var asyncOperation = request.SendWebRequest();
            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text; // Return API response
            }
            else
            {
                Debug.LogError($"Gemini API request failed: {request.error}");
                return "{\"organ\": \"Unknown\", \"answer\": \"Error fetching response\"}";
            }
        }
    }

    // Helper method to escape characters for JSON
    private string EscapeJsonString(string s)
    {
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");
    }
    private string TruncateString(string input, int maxLength = 240)
    {
        return input.Length > maxLength ? input.Substring(0, maxLength) : input;
    }

    private void ParseResponse(string jsonResponse)
{
    try
    {
        // Deserialize the Gemini response first.
        var responseJson = JsonUtility.FromJson<GeminiResponse>(jsonResponse);
        string textResponse = responseJson.candidates[0].content.parts[0].text;

        // Clean the response to remove markdown formatting (```json ... ```).
        string cleanedResponse = CleanResponse(textResponse);
        
        // Deserialize the cleaned JSON into our AnatomyResponse object.
        AnatomyResponse parsedResponse = JsonUtility.FromJson<AnatomyResponse>(cleanedResponse);

        Debug.Log($"Organ: {parsedResponse.organ}, Answer: {parsedResponse.answer}");

        OnOrganAnswerReceived?.Invoke(parsedResponse.organ, parsedResponse.answer);

        if(parsedResponse.organ == "Stroke")
            {
                StartCoroutine(stroke.PlayAnimation());
            }
        else if(parsedResponse.organ == "kidney-stones")
            {
                StartCoroutine(kidneyStones.PlayAnimation());
            }
            else
            {
                eventHandler.handleOrganChange(parsedResponse.organ);
                parsedResponse.answer = TruncateString(parsedResponse.answer, 280);
                tts.Speak(parsedResponse.answer);
                responseTextContainer.text = parsedResponse.answer;
                otherResponseContainer.text = parsedResponse.answer;
            }
        
        
    }
    catch (System.Exception e)
    {
        Debug.Log(jsonResponse);
        Debug.LogError($"Error parsing response: {e.Message}");
    }
}

// Helper method to remove Markdown code fences if present.
private string CleanResponse(string response)
{
    string trimmed = response.Trim();
    // Check if the response starts with triple backticks.
    if (trimmed.StartsWith("```"))
    {
        // Find the first newline (end of the opening fence).
        int firstNewline = trimmed.IndexOf('\n');
        // Find the last occurrence of triple backticks.
        int lastBackticks = trimmed.LastIndexOf("```");
        if (firstNewline != -1 && lastBackticks != -1 && lastBackticks > firstNewline)
        {
            // Extract the content between the fences.
            return trimmed.Substring(firstNewline, lastBackticks - firstNewline).Trim();
        }
    }
    return response;
}

    [System.Serializable]
    private class AnatomyResponse
    {
        public string organ;
        public string answer;
    }

    [System.Serializable]
    private class GeminiResponse
    {
        public Candidate[] candidates;
    }

    [System.Serializable]
    private class Candidate
    {
        public Content content;
    }

    [System.Serializable]
    private class Content
    {
        public Part[] parts;
    }

    [System.Serializable]
    private class Part
    {
        public string text;
    }
}
