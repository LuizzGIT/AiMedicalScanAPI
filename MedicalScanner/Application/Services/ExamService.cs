using System;
using System.Threading.Tasks;
using System.Text.Json; 

public class ExamService : IExamService
{
    private readonly IAiRepository _aiRepository;
    public ExamService(IAiRepository aiRepository)
    {
        _aiRepository = aiRepository;
    }
    public async Task<ExameResult> AnalisarExameAsync(string arq, ExamType tipo)
    {
        // Chamei o repository da IA passando o Base64 e o Enum
          string respostaIa = await _aiRepository.AnalisarImagemAsync(arq, tipo);

        // Converti o texto retornado pela IA na nossa classe DiagnosticDetails
          var detalhes = JsonSerializer.Deserialize<DiagnosticDetails>(respostaIa);

    // if basico so pra caso der erro da IA 
        if (detalhes == null)
            throw new InvalidOperationException("A inteligência artificial não retornou um formato de diagnóstico válido.");

        // Instanciei e preenchi o ExameResult com um id e os detalhes
        return new ExameResult 
        {
            IdExame = Guid.NewGuid(),
            TipoExame = tipo,
            ResultadoExame = detalhes
        };

    }
}