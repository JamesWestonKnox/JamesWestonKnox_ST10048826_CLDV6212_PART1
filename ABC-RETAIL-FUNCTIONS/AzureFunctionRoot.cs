using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABC_RETAIL_FUNCTIONS
{
    public class AzureFunctionRoot
    {
        private readonly ILogger<AzureFunctionRoot> _logger;

        public AzureFunctionRoot(ILogger<AzureFunctionRoot> logger)
        {
            _logger = logger;
        }
    }
}
