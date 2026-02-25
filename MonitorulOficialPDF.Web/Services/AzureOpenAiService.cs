using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace MonitorulOficialPDF.Web.Services
{
    public class AzureOpenAiService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureOpenAiService> _logger;

        public AzureOpenAiService(IConfiguration configuration, ILogger<AzureOpenAiService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> AnalyzeTextAsync(string text)
        {
            var endpoint = _configuration["AzureOpenAI:Endpoint"];
            var key = _configuration["AzureOpenAI:Key"];
            var deploymentName = _configuration["AzureOpenAI:DeploymentName"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key) || string.IsNullOrEmpty(deploymentName))
            {
                _logger.LogWarning("Azure OpenAI is not configured. Set AzureOpenAI:Endpoint, AzureOpenAI:Key, and AzureOpenAI:DeploymentName in configuration.");
                return "Azure OpenAI service is not configured. Please set endpoint, key, and deployment name.";
            }

            try
            {
                var credential = new AzureKeyCredential(key);
                var client = new AzureOpenAIClient(new Uri(endpoint), credential);
                var chatClient = client.GetChatClient(deploymentName);

                var messages = new ChatMessage[]
                {
                    new SystemChatMessage("Ești un asistent care analizează și rezumă documente oficiale din Monitorul Oficial al României. Răspunde în limba română."),
                    new UserChatMessage($"Analizează și rezumă următorul text extras dintr-un document al Monitorului Oficial:\n\n{text}")
                };

                var completion = await chatClient.CompleteChatAsync(messages);
                var result = completion.Value.Content[0].Text;

                _logger.LogInformation("Successfully analyzed text with Azure OpenAI");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing text with Azure OpenAI");
                return $"Error during text analysis: {ex.Message}";
            }
        }
    }
}
