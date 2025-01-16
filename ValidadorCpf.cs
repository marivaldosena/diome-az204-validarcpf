using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DiomeValidacoes.Documentos
{
    public class ValidadorCpf
    {
        private readonly ILogger<ValidadorCpf> _logger;

        public ValidadorCpf(ILogger<ValidadorCpf> logger)
        {
            _logger = logger;
        }

        [Function("validarcpf")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
