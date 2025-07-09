namespace HEM.Api.Interfaces;

public interface IAiTaskService
{
    Task<string> TaskSplitting(string name, string input);

    Task<EmployeeLoginInfo?> LoginDetails(string username, string password);
}

public class EmployeeLoginInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string UserName { get; set; }
}
