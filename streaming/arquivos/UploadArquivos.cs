using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace diome.streaming.arquivos
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
            _logger.LogInformation("Processando imagem no Storage");

            try {
                if (!req.Headers.TryGetValue("file-type", out var cabecalhoTipoArquivo)) {
                    _logger.LogInformation("Cabeçalho 'file-type' não foi informado.");
                    return new BadRequestObjectResult("O cabeçalho 'file-type' é obrigatório.");
                }

                var tipoArquivo = cabecalhoTipoArquivo.FirstOrDefault();

                if (!IsTipoArquivoValido(tipoArquivo))
                {
                    _logger.LogInformation("Cabeçalho 'file-type' contém valor inválido.");
                    return new BadRequestObjectResult("O cabeçalho 'file-type' contém valor inválido.");
                }
                
                var form = await req.ReadFormAsync();
                var arquivo = form.Files["file"];
                
                if (!IsArquivoValido(arquivo))
                {
                    _logger.LogInformation("Arquivo inválido.");
                    return new BadRequestObjectResult("Arquivo inválido.");
                } 

            } catch (Exception exception) {
                _logger.LogError("Erro ao processar arquivo. Motivo: {}", exception.Message);
                return new BadRequestObjectResult("Erro ao processar imagem.");
            }

            return new OkObjectResult("Upload concluído.");
        }

        private static bool IsTipoArquivoValido(string? tipoArquivo) {
            bool isArquivoNulo = tipoArquivo == null;
            bool isTipoValido = String.Compare(tipoArquivo, "imagem", StringComparison.Ordinal) == 0 || 
                                String.Compare(tipoArquivo, "video", StringComparison.Ordinal) == 0;
            return isTipoValido && !isArquivoNulo;
        }

        private static bool IsArquivoValido(IFormFile? arquivo)
        {
            bool isArquivoNulo = arquivo == null;
            bool isComprimentoArquivoSatisfatorio = arquivo?.Length > 0;
            
            return isComprimentoArquivoSatisfatorio && !isArquivoNulo;
        }
    }
}