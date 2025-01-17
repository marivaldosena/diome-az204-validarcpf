using System.Text.Json;
using System.Text.Json.Serialization;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Iniciando a validação de CPF.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Dictionary<string?, string?> data;
            try
            {
                data = JsonSerializer.Deserialize<Dictionary<string?, string?>>(requestBody);
            }
            catch (JsonException)
            {
                return new BadRequestObjectResult(new { message = "Erro ao desserializar o JSON." });
            }

            if (data == null || !data.ContainsKey("cpf"))
            {
                return new BadRequestObjectResult(new { message = "O campo 'cpf' é obrigatório." });
            }

            string? cpf = data["cpf"];

            if (string.IsNullOrWhiteSpace(cpf)) {
                return new BadRequestObjectResult(new { message = "O campo 'cpf' não pode ser nulo, vazio ou espaços em branco." });
            }

            if (!IsValidCpf(cpf))
            {
                return new BadRequestObjectResult(new { message = "CPF inválido." });
            }

            return new OkObjectResult("CPF é válido.");
        }

        internal static bool IsValidCpf(string cpf)
        {
            if (cpf.Length != 11 || !long.TryParse(cpf, out _))
            {
                return false;
            }

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            }

            int resto = soma % 11;
            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            }

            resto = soma % 11;
            if (resto < 2)
            {
                resto = 0;
            }
            else
            {
                resto = 11 - resto;
            }

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}