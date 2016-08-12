#region Copyright (c) 2016 Periwinkle Software Limited
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace PetStore
{
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Serilog;
    using Swashbuckle.Swagger.Model;

    public class Startup
    {
        private IHostingEnvironment env { get; set; }

        public Startup(IHostingEnvironment env)
        {
            this.env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName.ToLower()}.json", true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMvc().AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
# if DEBUG
                    options.SerializerSettings.Formatting = Formatting.Indented;
#else
                    options.SerializerSettings.Formatting = Formatting.None;
#endif
                });

            services.AddSwaggerGen(c =>
                {
                    c.SingleApiVersion(new Info
                    {
                        Version = "v1",
                        Title = "Swagger Petstore",
                        Description = "This is a sample server Petstore server. You can find out more about Swagger at [http://swagger.io](http://swagger.io) or on [irc.freenode.net, #swagger](http://swagger.io/irc/). For this sample, you can use the api key 'special-key' to test the authorization filters.\n\nFind out more about Swagger",
                        TermsOfService = "http://swagger.io/terms/",
                        Contact = new Contact()
                        {
                            Email = "mailto:apiteam@swagger.io?subject=Swagger",
                            Name = "Swagger",
                            Url = "http://swagger.io",
                        },
                        License = new License()
                        {
                            Name = "Apache 2.0",
                            Url = "http://www.apache.org/licenses/LICENSE-2.0.html",
                        },
                    });
                    c.DescribeAllEnumsAsStrings();
                    c.IncludeXmlComments(GetXmlCommentsPath());
                    c.DocumentFilter<ApplySwaggerDocumentModifications>();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            Log.Logger = new LoggerConfiguration().WriteTo.RollingFile(@"Log-{Date}.txt").CreateLogger();

            app.UseMvc();
            app.UseCors(option => option.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
                //app.UseBrowserLink();
            }

            app.UseSwagger();
            app.UseSwaggerUi();
        }

        private static string GetXmlCommentsPath()
        {
            var app = PlatformServices.Default.Application;
            return Path.Combine(app.ApplicationBasePath, app.ApplicationName + ".xml");
        }
    }
}
