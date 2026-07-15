using System;

public class ExameResult
{
    public Guid idExame {get; set;}

    public ExamType TipoExame {get; set;}

    public required DiagnosticDetails  ResultadoExame {get; set;} 
}