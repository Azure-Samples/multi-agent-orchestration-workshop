namespace MultiAgentWorkshop.Agent.Extensions;

public static class StringExtensions
{
    public static string GetAzureOpenAIResponsesEndpoint(this string endpoint)
    {
        // The AzureOpenAIClient expects the endpoint to be in the format of "https://{resourceName}.openai.azure.com/openai/v1/"
        // This method ensures that the endpoint is correctly formatted regardless of how it's provided in the configuration.
        var url = $"{string.Join("://", endpoint.Split([ ':', '/' ], StringSplitOptions.RemoveEmptyEntries).Take(2))}/openai/v1/";

        return url;
    }
}