using HEM.Api.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HEM.Api.Services;

public class AiTaskServices(IConfiguration configuration) : IAiTaskService
{
    public async Task<string> TaskSplitting(string input)
    {
        var apiLink = configuration["CopilotAi:ApiLink"];
        var apiKey = configuration["CopilotAi:LeaveKey"];
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

        var response = await httpClient.PostAsync($"{apiLink}/conversations", null);
        var conversationData = await response.Content.ReadFromJsonAsync<ConversationStartResponse>();

        //string conversationId = conversationData.ConversationId;
        var conversationId = "Df3D97FLcC2LskHNpY1kPy-in";

        var activityData = new
        {
            type = "message",
            from = new { id = "user1" },
            text = input
        };

        var sendMessageRequest = new HttpRequestMessage(
        HttpMethod.Post,
        $"{apiLink}/conversations/{conversationId}/activities")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", apiKey) },
            Content = new StringContent(JsonSerializer.Serialize(activityData), Encoding.UTF8, "application/json")
        };

        await httpClient.SendAsync(sendMessageRequest);
        await Task.Delay(10000);

        var getActivitiesRequest = new HttpRequestMessage(HttpMethod.Get, $"{apiLink}/conversations/{conversationId}/activities");
        getActivitiesRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var getActivitiesResponse = await httpClient.SendAsync(getActivitiesRequest);

        var activitiesContent = await getActivitiesResponse.Content.ReadAsStringAsync();
        var activitiesJson = JsonDocument.Parse(activitiesContent);

        var activities = activitiesJson.RootElement.GetProperty("activities").EnumerateArray();
        var responseData = activities
        .Where(a =>
            a.TryGetProperty("from", out var fromProp) &&
            fromProp.TryGetProperty("id", out var idProp) &&
            idProp.GetString() != "user1" &&
            a.TryGetProperty("text", out _)
        )
        .Select(a => a.GetProperty("text").GetString())
        .LastOrDefault();

        if (string.IsNullOrEmpty(responseData))
        {
            throw new InvalidOperationException("No response from Copilot Studio bot.");
        }

        return responseData;

    }

    public class ConversationStartResponse
    {
        public string ConversationId { get; set; }
        public string Token { get; set; }
    }

    public class ActivitySet
    {
        public List<Activity> Activities { get; set; }
    }

    public class Activity
    {
        public string Type { get; set; }
        public From From { get; set; }
        public string Text { get; set; }
    }

    public class From
    {
        public string Id { get; set; }
    }

}
