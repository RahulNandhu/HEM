namespace HEM.Api.Interfaces;

public interface IAiTaskService
{
    Task<string> TaskSplitting(string name, string input);
}
