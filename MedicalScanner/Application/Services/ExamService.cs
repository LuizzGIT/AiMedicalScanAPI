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
       var prompt = "Look carefully at every detail in this medical exam image. " +
             "Describe everything you can see: all numbers, measurements, labels, " +
             "wave patterns, line shapes, colors, scales, axes, and any text visible. " +
             "Be as detailed and specific as possible.";
 
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