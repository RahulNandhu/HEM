using HEM.Api.Interfaces;
using Microsoft.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HEM.Api.Services;

public class AiTaskServices(IConfiguration configuration) : IAiTaskService
{
    public async Task<string> TaskSplitting(string user, string input)
    {
        var apiLink = configuration["CopilotAi:ApiLink"];
        var apiKey = configuration["CopilotAi:ApiKey"];
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        var connectionString = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
        string? agentKey = null;

        if (user.ToLower() == "priyanka")
        {
            apiKey = configuration["CopilotAi:ApiKey"];
        }
        else if (user.ToLower() == "salim")
        {
            apiKey = configuration["CopilotAi:ApiKey"];
        }
        else if(user.ToLower() == "shiyas") {
            apiKey = configuration["CopilotAi:SKey"];
        }
        else
        {
            apiKey = configuration["CopilotAi:ApiKey"];
        }

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $@"
                                   SELECT TOP 1 ApiKey
                                   FROM BCMCHMicro.dbo.AgentDetails
                                   WHERE UserName = @userName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user);
                    await connection.OpenAsync();

                    agentKey = await command.ExecuteScalarAsync() as string;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        var response = await httpClient.PostAsync($"{apiLink}/conversations", null);
        var conversationData = await response.Content.ReadFromJsonAsync<ConversationStartResponse>();

        string conversationId = conversationData.ConversationId;

        var activityData = new
        {
            type = "message",
            from = new { id = "user1" },
            text = input
        };

        var sendMessageRequest = new HttpRequestMessage(
        HttpMethod.Post,
        $"{apiLink}/conversations/{agentKey}/activities")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", apiKey) },
            Content = new StringContent(JsonSerializer.Serialize(activityData), Encoding.UTF8, "application/json")
        };

        await httpClient.SendAsync(sendMessageRequest);
        await Task.Delay(13000);

        var getActivitiesRequest = new HttpRequestMessage(HttpMethod.Get, $"{apiLink}/conversations/{agentKey}/activities");
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


    public class AgentAi
    {
        public int Id { get; set; }
        public string Agent { get; set; }
        public string UserName { get; set; }
        public string ApiKey { get; set; }
    }

    public class From
    {
        public string Id { get; set; }
    }

    public async Task<EmployeeLoginInfo?> LoginDetails(string username, string password)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        EmployeeLoginInfo? employee = null;

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
              SELECT Id, Name, Role, UserName, Password
              FROM BCMCHMicro.dbo.Employees
              WHERE LOWER(UserName) = LOWER(@UserName)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", username);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var dbPassword = reader["Password"] as string;

                            if (dbPassword == password)
                            {
                                employee = new EmployeeLoginInfo
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = reader["Name"] as string,
                                    Role = reader["Role"] as string,
                                    UserName = reader["UserName"] as string
                                };
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return employee;
    }



}
