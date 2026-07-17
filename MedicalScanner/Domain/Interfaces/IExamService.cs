using System;
using System.Threading.Tasks;
public interface IExamService
{
    Task<ExameResult> AnalisarExameAsync(string arq, ExamType tipo);
}
