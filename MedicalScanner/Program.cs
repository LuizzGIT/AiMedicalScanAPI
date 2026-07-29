using MedicalScanner.Application.Services;
using MedicalScanner.Domain.Interfaces;
using MedicalScanner.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
// using MedicalScanner.Domain.Interfaces;
// using MedicalScanner.Application.Services;
// using MedicalScanner.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Injeção de Dependências (CORRIGIDO PARA O OLLAMA)
builder.Services.AddScoped<IAiRepository, AiRepository>();
builder.Services.AddScoped<IExamService, ExamService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// endpoint do arquivo 
app.MapPost("/api/diagnosticos", async (
    IFormFile imagem,
    [FromForm] ExamType tipoExame,
    IExamService examService) =>
{
    if (imagem == null || imagem.Length == 0)
        return Results.BadRequest(new { Erro = "Imagem inválida." });

    // Pega o fluxo de dados puro, sem converter para Base64
    using var stream = imagem.OpenReadStream();

    try
    {
        var resultado = await examService.AnalisarExameAsync(stream, tipoExame);
        return Results.Ok(resultado);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Erro = ex.Message });
    }
})
.DisableAntiforgery();

app.Run();