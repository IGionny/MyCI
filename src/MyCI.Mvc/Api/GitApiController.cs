using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCI.Common;
using MyCI.Mvc.Models;


namespace MyCI.Mvc.Api
{
    [Route("Api/Git")]
    public class GitApiController : Controller
    {
        private readonly IOptions<MyCISettings> _ciSettings;
        private readonly ILogger<GitApiController> _logger;

        public GitApiController(IOptions<MyCISettings> ciSettings, ILogger<GitApiController>  logger)
        {
            _ciSettings = ciSettings ?? throw new ArgumentNullException(nameof(ciSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Use thi entry-point as web-hook for git-events like update
        /// </summary>
        [HttpGet("Update")]
        public async Task<IActionResult> Update()
        {
            //Fire & Forget
            Task.Run(async () => await UpdateGitAsync().ConfigureAwait(false));

            //return immediately ok
            return Ok();
        }

        public async Task UpdateGitAsync()
        {
            var result = ShellHelper.Execute("git pull", _ciSettings.Value.SolutionPath);
            _logger.LogInformation("GIT pull result: " + result);
        }
    }
}