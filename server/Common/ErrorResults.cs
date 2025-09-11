using Microsoft.AspNetCore.Mvc;

namespace health_app.Common
{
    public class ErrorResults
    {
        public static ActionResult Unauthorized(ControllerBase c, string message,
        string code = "Unauthorized") =>
        Build(c, StatusCodes.Status401Unauthorized, code, "Não autenticado", message);

        public static ActionResult Forbidden(ControllerBase c, string message,
            string code = "Forbidden") =>
            Build(c, StatusCodes.Status403Forbidden, code, "Acesso negado", message);

        public static ActionResult NotFound(ControllerBase c, string message,
            string code = "NotFound") =>
            Build(c, StatusCodes.Status404NotFound, code, "Recurso não encontrado", message);

        public static ActionResult Conflict(ControllerBase c, string message,
            string code = "Conflict",
            IDictionary<string, string[]>? errors = null) =>
            Build(c, StatusCodes.Status409Conflict, code, "Conflito", message, errors);

        public static ActionResult BadRequest(ControllerBase c, string message,
            string code = "BadRequest",
            IDictionary<string, string[]>? errors = null) =>
            Build(c, StatusCodes.Status400BadRequest, code, "Erro de validação", message, errors);

        private static ActionResult Build(ControllerBase c, int status, string code,
            string title, string message, IDictionary<string, string[]>? errors = null)
        {
            var env = new ErrorEnvelope
            {
                Status = status,
                Code = code,
                Title = title,
                Message = message,
                Errors = errors,
                TraceId = c.HttpContext.TraceIdentifier,
                Path = c.HttpContext.Request.Path
            };
            return new ObjectResult(env) { StatusCode = status };
        }

    }
}
