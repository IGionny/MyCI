using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCI.Common;
using MyCI.Mvc.Models;

namespace MyCI.Mvc.Api
{
    [Route("Api/Docker")]
    public class DockerApiController : Controller
    {
        private readonly IOptions<MyCISettings> _ciSettings;
        private readonly ILogger<DockerApiController> _logger;

        public DockerApiController(IOptions<MyCISettings> ciSettings, ILogger<DockerApiController> logger)
        {
            _ciSettings = ciSettings ?? throw new ArgumentNullException(nameof(ciSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Use this to build docker image
        /// </summary>
        [HttpGet("Build")]
        public async Task<IActionResult> Build()
        {
            //Fire & Forget
            Task.Run(async () => await DockerBuildAsync().ConfigureAwait(false));

            //return immediately ok
            return Ok();
        }



        /// <summary>
        /// Use this to execute tests in the docker image
        /// </summary>
        [HttpGet("Tests")]
        public async Task<IActionResult> RunTests()
        {
            //Fire & Forget
            Task.Run(async () => await DockerTestsAsync().ConfigureAwait(false));

            //return immediately ok
            return Ok();
        }

        /// <summary>
        /// Use this to push the image
        /// </summary>
        [HttpGet("Push")]
        public async Task<IActionResult> PushImage()
        {
            //Fire & Forget
            Task.Run(async () => await DockerPushAsync().ConfigureAwait(false));

            //return immediately ok
            return Ok();
        }


      
        public async Task DockerBuildAsync()
        {
            var result = ShellHelper.Execute("docker build", _ciSettings.Value.SolutionPath);
            _logger.LogInformation("docker build result: " + result);
        }

        public async Task DockerTestsAsync()
        {
            var result = ShellHelper.Execute("docker run", _ciSettings.Value.SolutionPath);
            _logger.LogInformation("docker run result: " + result);
        }

        public async Task DockerPushAsync()
        {
            var result = ShellHelper.Execute("docker push", _ciSettings.Value.SolutionPath);
            _logger.LogInformation("docker run result: " + result);
        }

    }
}