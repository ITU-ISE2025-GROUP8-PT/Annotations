namespace Annotations.Blazor.Client.ApiTest
{
    public interface IApiTester
    {
        Task<IEnumerable<string>> GetTestStringsAsync();
    }
}
