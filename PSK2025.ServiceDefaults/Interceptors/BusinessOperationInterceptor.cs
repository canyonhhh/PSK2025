using System.Security.Claims;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PSK2025.ServiceDefaults.Interceptors
{
    public class BusinessOperationInterceptor(
        ILogger<BusinessOperationInterceptor> logger,
        IHttpContextAccessor httpContextAccessor) : IInterceptor
    {
        private readonly ILogger<BusinessOperationInterceptor> _logger = logger;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public void Intercept(IInvocation invocation)
        {
            var className = invocation.TargetType!.Name;
            var methodName = invocation.Method.Name;
            var parameters = string.Join(", ", invocation.Arguments.Select(a => a?.ToString() ?? "null"));

            var user = _httpContextAccessor.HttpContext?.User;

            string userId = user?.Identity?.IsAuthenticated == true
                ? user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous"
                : "anonymous";

            string userRoles = user?.Identity?.IsAuthenticated == true
                ? string.Join(",", user.FindAll(ClaimTypes.Role).Select(c => c.Value))
                : "none";

            _logger.LogInformation(
                "Business Operation: {Time} | User: {UserId} | Roles: {Roles} | Method: {ClassName}.{MethodName} | Parameters: {Parameters}",
                DateTime.UtcNow, userId, userRoles, className, methodName, parameters);

            try
            {
                invocation.Proceed();

                if (invocation.Method.ReturnType.IsAssignableFrom(typeof(Task)))
                {
                    var task = (Task)invocation.ReturnValue!;
                    task.ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            LogException(className, methodName, t.Exception);
                        }
                        else
                        {
                            _logger.LogInformation(
                                "Business Operation Completed: {Time} | User: {UserId} | Method: {ClassName}.{MethodName}",
                                DateTime.UtcNow, userId, className, methodName);
                        }
                    });
                }
                else
                {
                    _logger.LogInformation(
                        "Business Operation Completed: {Time} | User: {UserId} | Method: {ClassName}.{MethodName}",
                        DateTime.UtcNow, userId, className, methodName);
                }
            }
            catch (Exception ex)
            {
                LogException(className, methodName, ex);
                throw;
            }
        }

        private void LogException(string className, string methodName, Exception exception)
        {
            _logger.LogError(
                exception,
                "Business Operation Exception: {Time} | Method: {ClassName}.{MethodName} | Exception: {ExceptionMessage}",
                DateTime.UtcNow, className, methodName, exception.Message);
        }
    }
}