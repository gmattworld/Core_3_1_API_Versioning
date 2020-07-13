using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APP.API.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Core_3_1_API_Versioning
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration property
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Set CORS policy name
        /// </summary>
        private readonly string CorsPolicyName = "APPCORS";

        /// <summary>
        /// Set Authentication name
        /// </summary>
        private readonly string AuthName = "OAuth";

        /// <summary>
        /// Set URI token name
        /// </summary>
        private readonly string URITokenName = "token";


        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Core Policy Configuration
            services.AddCors(config =>
            {
                config.AddPolicy(CorsPolicyName, option =>
                {
                    option.WithOrigins("*")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins();
                });
            });

            // Add OAuth JWT
            services.AddAuthentication(config =>
            {
                // Check the cookie to confirm that user is authenticated
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                // On sign in, deal out a cookie
                config.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;

                // Check if user is allowed to perform an action
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(AuthName, config =>
                {
                    // Encrypt JWT Key
                    var secretBytes = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
                    var key = new SymmetricSecurityKey(secretBytes);

                    // Configuration to allow token passed through header
                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        ClockSkew = TimeSpan.Zero
                    };

                    // Configuration to allow token passed through URI
                    config.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Query.ContainsKey(URITokenName))
                            {
                                context.Token = context.Request.Query[URITokenName];
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSwaggerDocumentation();

            services.AddVersionedApiExplorer(options =>
            {
                options.SubstituteApiVersionInUrl = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new MediaTypeApiVersionReader();
                config.ApiVersionSelector = new CurrentImplementationApiVersionSelector(config);
            });

            services.AddMvc(c => c.Conventions.Add(new ApiExplorerGroupPerVersionConvention()));

            services.AddControllers();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(CorsPolicyName);
            app.UseApiVersioning();
            app.UseSwaggerDocumentation();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                .RequireCors(CorsPolicyName);
            });
        }
    }
}
