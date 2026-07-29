namespace MedicalScanner.Domain.Interfaces;

public interface IAiRepository
{
    IAsyncEnumerable<string> AnalyzeImageStreamAsync(string prompt, byte[] imageBytes);
}