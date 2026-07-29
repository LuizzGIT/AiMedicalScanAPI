using MedicalScanner.Domain.Interfaces;
 
namespace MedicalScanner.Application.Services;
 
public class ExamService : IExamService
{
    private readonly IAiRepository _aiRepository;
 
    public ExamService(IAiRepository aiRepository)
    {
        _aiRepository = aiRepository;
    }
 
   public async Task<ExameResult> AnalisarExameAsync(Stream imageStream, ExamType tipoExame)
{
    // 1. Converte o Stream para bytes
    using var memoryStream = new MemoryStream();
    await imageStream.CopyToAsync(memoryStream);
    var imageBytes = memoryStream.ToArray();
 
    // Log temporário de debug: confirma se a imagem realmente chegou com bytes.
    // Remova depois de confirmar que está tudo ok.
    Console.WriteLine($"[DEBUG] Tamanho da imagem recebida: {imageBytes.Length} bytes");
 
    // 2. Monta o prompt: pedir descrição técnica/objetiva em vez de "diagnóstico"
    // reduz bastante a chance do modelo recusar ou devolver resposta vazia.
    var prompt = $"Você está descrevendo achados visuais " +
                 $"(formato de ondas/gráficos, padrões, cores, textos e números visíveis, " +
                 $"estrutura geral da imagem). Não forneça diagnóstico, apenas descreva o que é " +
                 $"visualmente observável na imagem.";
 
    // 3. Chama o repositório
    var resultStream = _aiRepository.AnalyzeImageStreamAsync(prompt, imageBytes);
 
    // 4. Aguarda e acumula todo o texto gerado pela IA
    var diagnosticoCompleto = "";
    await foreach (var chunk in resultStream)
    {
        // Log temporário de debug: confirma se chunks estão chegando com conteúdo.
        Console.WriteLine($"[DEBUG] Chunk recebido: '{chunk}'");
        diagnosticoCompleto += chunk;
    }
 
    // 5. Monta e retorna o objeto que a sua interface exige
    return new ExameResult
    {
        IdExame = Guid.NewGuid(),
        TipoExame = tipoExame,
        ResultadoExame = new DiagnosticDetails 
        { 
            MedicalObservations = new List<string> { diagnosticoCompleto }
        }
    };
}
}