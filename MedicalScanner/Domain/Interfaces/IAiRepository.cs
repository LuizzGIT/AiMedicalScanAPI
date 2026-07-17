using System.Threading.Tasks;

public interface IAiRepository
{
    Task<string> AnalisarImagemAsync(string base64Image, ExamType tipoExame);
}