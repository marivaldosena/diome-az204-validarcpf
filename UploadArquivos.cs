using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DiomePlataformaStreamingVideos.Arquivos
{
    public class UploadArquivos
    {
        private readonly ILogger<UploadArquivos> _logger;

        public UploadArquivos(ILogger<UploadArquivos> logger)
        {
            _logger = logger;
        }

        [Function("uploadArquivos")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Iniciando o upload de arquivos.");


            return new OkObjectResult("Upload concluído.");
        }
    }
}