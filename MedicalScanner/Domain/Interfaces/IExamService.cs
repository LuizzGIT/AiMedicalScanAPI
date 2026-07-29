using System;
using System.Threading.Tasks;
public interface IExamService
{
    Task<ExameResult> AnalisarExameAsync(Stream imagemStream, ExamType tipo);
}
