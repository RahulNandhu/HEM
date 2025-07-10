using HEM.Api.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        UserStory story = null;

        if (user.ToLower() == "priyanka")
        {
            apiKey = configuration["CopilotAi:ApiKey"];
        }
        else if (user.ToLower() == "salim")
        {
            apiKey = configuration["CopilotAi:AdminKey"];
        }
        else if(user.ToLower() == "shiyas") {
            apiKey = configuration["CopilotAi:SKey"];
        }
        else
        {
            apiKey = configuration["CopilotAi:ApiKey"];
        }

        if (user.ToLower() == "salim")
        {
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
                        //agentKey = "BpoOBDDEQhlCGiGkK5MmxH-in"; //pri
                        //agentKey = "CwI4hdevcR4JP7uQ5NtFmM-in"; //sa
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        var response = await httpClient.PostAsync($"{apiLink}/conversations", null);
        var conversationData = await response.Content.ReadFromJsonAsync<ConversationStartResponse>();

        
        string conversationId = conversationData.ConversationId;
        agentKey = conversationId;

        string lowerInput = input.ToLower();

        bool hasStory = lowerInput.Contains("story") || lowerInput.Contains("stories");
        bool hasUnassigned =
            lowerInput.Contains("unassigned") ||
            lowerInput.Contains("not assigned");
        bool hasS = lowerInput.Contains("split") || lowerInput.Contains("splite");

        if (hasStory && hasUnassigned && !hasS)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                                SELECT TOP 1 Status, Description
                                FROM BCMCHMicro.dbo.UserStories
                                WHERE Status = @status";

                   

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@status", "unassigned");
                        //command.Parameters.AddWithValue("@data", "unassigned");
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (!reader.HasRows)
                            {
                                await Task.Delay(10000);
                                return "There are no unassigned user stories";
                            }
                            if (await reader.ReadAsync())
                            {
                                story = new UserStory
                                {
                                    Status = reader.GetString(0),
                                    Description = reader.GetString(1)
                                    
                                };
                                if (story != null)
                                {
                                    await Task.Delay(10000);
                                    return "The unassigned user stories\n 1." + story.Description;
                                }
                                else
                                {
                                    await Task.Delay(10000);
                                    return "There are no unassigned user stories";
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
        }


        bool hasSplit = lowerInput.Contains("split") || lowerInput.Contains("splite");
        bool hasAssign =
            lowerInput.Contains("assign");

        if (hasSplit && hasAssign)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                                    UPDATE BCMCHMicro.dbo.UserStories
                                    SET Status = @Status
                                    WHERE Id = @Id";
                    input = "split the user story";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", "Assigned");
                        command.Parameters.AddWithValue("@Id", 1);
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //if (story != null)
        //{
        //    string storyJson = JsonSerializer.Serialize(story, new JsonSerializerOptions
        //    {
        //        WriteIndented = true
        //    });
        //    input = $"{input}\n\nuser story: {storyJson}";
        //}

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
        await Task.Delay(20000);

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

    public class UserStory
    {
        public string Description { get; set; }
        public string Status { get; set; }
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
