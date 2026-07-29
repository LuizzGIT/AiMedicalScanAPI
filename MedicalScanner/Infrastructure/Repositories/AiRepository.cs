using OllamaSharp;
using OllamaSharp.Models;
using MedicalScanner.Domain.Interfaces;

namespace MedicalScanner.Infrastructure.Repositories;

public class AiRepository : IAiRepository
{
    private readonly OllamaApiClient _ollamaClient;

    public AiRepository()
    {
        // 1. Crie um HttpClient customizado
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:11434"),
            
            Timeout = TimeSpan.FromMinutes(10) 
        };

        // 3. Passe o httpClient para o OllamaApiClient
        _ollamaClient = new OllamaApiClient(httpClient);
        
        _ollamaClient.SelectedModel = "moondream";
    }

    public async IAsyncEnumerable<string> AnalyzeImageStreamAsync(string prompt, byte[] imageBytes)
    {
        // O OllamaSharp espera a imagem no formato Base64 dentro de uma lista
        var request = new GenerateRequest
        {
            Prompt = prompt,
            Images = new string[] { Convert.ToBase64String(imageBytes) }
        };

        await foreach (var responseStream in _ollamaClient.GenerateAsync(request))
        {
            if (responseStream != null)
            {
                yield return responseStream.Response;
            }
        }
    }
}