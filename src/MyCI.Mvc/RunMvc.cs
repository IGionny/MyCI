using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCI.Common;
using Newtonsoft.Json.Serialization;

namespace MyCI.Mvc
{
    public static class RunMvc
    {
        public static string CommonAppSettingFolder =>
            Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "Configurations");

        /// <summary>
        /// Ensure that all the folders needed by this application are present
        /// </summary>
        public static void EnsureDirectories()
        {
            FileSystemUtilities.EnsureDirectory("App_Data");
            FileSystemUtilities.EnsureDirectory("App_Data", "Logs");
            FileSystemUtilities.EnsureDirectory("App_Data", "Configurations");
        }

        private static IConfigurationRoot _configuration;
        public static IConfigurationRoot MakeConfigurationByAppSettingsJson()
        {
            //Initialize configuration by json
            if (_configuration == null)
            {
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(CommonAppSettingFolder)
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile(
                        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                        true, true)
                    .AddJsonFile("appsettings.custom.json", true, true)
                    .AddEnvironmentVariables()
                    .Build();
            }
            return _configuration;
        }

        public static IConfigurationRoot GetConfigurationRoot()
        {
            return _configuration;
        }

        public static IMvcBuilder RemovePlainFormatter(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddMvcOptions(options =>
            {
                //to avoid responses with the content-type = text/plain
                //https://github.com/aspnet/Mvc/issues/3692
                //https://docs.microsoft.com/en-us/aspnet/core/mvc/models/formatting
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            });
            return mvcBuilder;
        }

        /// <summary>
        /// This fix the case
        /// </summary>
        public static IMvcCoreBuilder SetJsonFormatter(this IMvcCoreBuilder mvcCoreBuilder)
        {
            mvcCoreBuilder.AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()).AddJsonFormatters();

            return mvcCoreBuilder;
        }


        public static IMvcBuilder EnableAuthentication(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddMvcOptions(options => // All endpoints need authentication
                options.Filters.Add(
                    new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())));
            return mvcBuilder;

        }

        public static IMvcBuilder SetAuthorizePage(this IMvcBuilder mvcBuilder, string authorizeUrl = "/Account/Login")
        {
            mvcBuilder.AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizePage(authorizeUrl);
            });
            return mvcBuilder;
        }

        
      

    }
}