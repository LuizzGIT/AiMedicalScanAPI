using MedicalScanner.Domain.Interfaces;
 
namespace MedicalScanner.Application.Services;
 
public class ExamService : IExamService
{
    private readonly IAiRepository _aiRepository;
 
    public ExamService(IAiRepository aiRepository)
    {
        _aiRepository = aiRepository;
    }
 
    public async Task<ExameResult> AnalisarExameAsync(Stream imageStream)
    {
        //  Converte o Stream para bytes
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream);
        var imageBytes = memoryStream.ToArray();
 
        //  Prompt em inglês simples, pois portugues não funciona
        var prompt = "Describe the visual content of this medical exam image in detail. " +
                     "Include visible patterns, lines, numbers, text, and graphical elements.";
 
        //  Chama o repositório
        var resultStream = _aiRepository.AnalyzeImageStreamAsync(prompt, imageBytes);
 
        //  Aguarda e acumula todo o texto gerado pela IA
        var diagnosticoCompleto = "";
        await foreach (var chunk in resultStream)
        {
            diagnosticoCompleto += chunk;
        }
 
        //  Monta e retorna o resultado
        return new ExameResult
        {
            IdExame = Guid.NewGuid(),
            ResultadoExame = new DiagnosticDetails
            {
                MedicalObservations = new List<string> { diagnosticoCompleto }
            }
        };
    }
}