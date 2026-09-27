using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql;
using Scalar.AspNetCore;
using Users.Application.Mappings;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;
using Users.Persistence.Repositories;
using Users.WebAPI.Common.Converters;
using Users.WebAPI.Common.Extensions;

namespace Users.WebAPI
{
    /// <summary>
    /// Provides extension methods for configuring application services related to dependency injection, including database services, API services, and other middleware
    /// configurations.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Provides extension methods to configure dependency injection for an Authentication API.
        /// </summary>
        /// <remarks>
        /// This static class defines methods for setting up services required by the Authentication API, including API services, authentication, authorization, database
        /// services and CORS policies.
        /// </remarks>
        extension(WebApplicationBuilder applicationBuilder)
        {
            /// <summary>
            /// Adds and configures the API services required for the application.
            /// </summary>
            /// <remarks>
            /// This method registers and configures essential services for the API, including JSON serializer options, AutoMapper, repositories, controllers, and OpenAPI/Swagger for API documentation.
            /// It sets JSON serialization policies to handle naming conventions and adds specific converters for custom data types such as enums, DateOnly, and TimeOnly.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, allowing for method chaining.</returns>
            public WebApplicationBuilder AddApiServices()
            {
                applicationBuilder.Services.Configure<JsonOptions>((options) =>
                {
                    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                    options.SerializerOptions.Converters.Add(new DecimalJsonConverter());
                    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                    options.SerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
                });

                // Registering mappings:
                applicationBuilder.Services.AddAutoMapper(
                    (_) => { },
                    typeof(DomainToDtoMappingProfile)
                );

                applicationBuilder.Services.AddScoped<IUserRepository, UserRepository>();
                applicationBuilder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();

                applicationBuilder.Services.AddControllers();
                applicationBuilder.Services.AddEndpointsApiExplorer();
                applicationBuilder.Services.AddOpenApi((options) =>
                {
                    options.AddDocumentTransformer(async (document, _, cancellationToken) =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        document.Components ??= new OpenApiComponents();
                        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                        OpenApiSecurityScheme scheme = new()
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            Description = $"Obtain the token via API Client (e.g. Postman)!"
                        };

                        document.Components.SecuritySchemes["Bearer"] = scheme;

                        OpenApiSecurityRequirement requirement = new()
                        {
                            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                        };

                        document.Security = [requirement];

                        await Task.CompletedTask;
                    });
                });

                return applicationBuilder;
            }

            /// <summary>
            /// Configures authentication services for the application.
            /// </summary>
            /// <remarks>
            /// This method sets up JWT Bearer authentication for the application.
            /// It configures the authority for token validation using the Identity Server URL specified in the application's configuration.
            /// Additionally, it customizes token validation parameters, such as disabling audience validation and setting the claim type for roles.
            /// </remarks>
            /// <returns>The same <see cref="WebApplicationBuilder"/> instance, allowing for method chaining.</returns>
            public WebApplicationBuilder ConfigureAuthentication()
            {
                applicationBuilder.Services
                    .AddAuthentication("Bearer")
                    .AddJwtBearer((options) =>
                    {
                        options.Authority = applicationBuilder.Configuration["ServicesUrls:IdentityServer"];
                        options.MapInboundClaims = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            RoleClaimType = "role"
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = (context) =>
                            {
                                Console.WriteLine($">>> JWT FAILED: {context.Exception.Message}");

                                return Task.CompletedTask;
                            },
                            OnTokenValidated = (context) =>
                            {
                                Console.WriteLine($">>> JWT VALID, claims: {string.Join(", ", context.Principal!.Claims.Select(ctx => $"{ctx.Type}={ctx.Value}"))}");

                                return Task.CompletedTask;
                            }
                        };
                    });

                return applicationBuilder;
            }

            /// <summary>
            /// Configures authorization services and policies for the application.
            /// </summary>
            /// <remarks>
            /// Sets the default authorization policy to require authenticated users and
            /// registers the <c>ApiScope</c> policy, which requires the <c>scope</c>
            /// claim with the value <c>geek_shopping</c>.
            /// </remarks>
            /// <returns>
            /// The same <see cref="WebApplicationBuilder"/> instance, enabling method chaining.
            /// </returns>
            public WebApplicationBuilder ConfigureAuthorization()
            {
                applicationBuilder.Services.AddAuthorization((options) =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.AddPolicy(
                        "ApiScope",
                        (policy) =>
                        {
                            policy.RequireAuthenticatedUser();
                            policy.RequireClaim("scope", "geek_shopping");
                        }
                    );
                });

                return applicationBuilder;
            }

            public WebApplicationBuilder AddDatabaseConfiguration()
            {
                string connectionString = new NpgsqlConnectionStringBuilder
                {
                    Host = Environment.GetEnvironmentVariable("DATABASE_HOST"),
                    Port = int.Parse(Environment.GetEnvironmentVariable("DATABASE_PORT")!),
                    Username = Environment.GetEnvironmentVariable("DATABASE_USER"),
                    Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD"),
                    Database = Environment.GetEnvironmentVariable("DATABASE_NAME")
                }.ConnectionString;

                applicationBuilder.Services.AddDbContext<AppDbContext>((options) => options.UseNpgsql(connectionString));

                return applicationBuilder;
            }

            /// <summary>
            /// Configures the application's CORS (Cross-Origin Resource Sharing) policy to allow unrestricted access from any origin, enabling development and testing scenarios.
            /// </summary>
            /// <remarks>
            /// This method sets up a default CORS policy named "DefaultCors" within the application's service collection.
            /// The policy permits requests from any origin, with any header, and any HTTP method.
            /// Use this policy to facilitate communication between client and server during development where cross-origin requests are necessary.
            /// Ensure proper configuration in production environments to restrict origins and secure communication.
            /// </remarks>
            public void AddCorsPolicy()
            {
                applicationBuilder.Services.AddCors((options) =>
                {
                    options.AddPolicy("DefaultCors", (policy) =>
                    {
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });
            }
        }

        /// <summary>
        /// Configures the application's middleware pipeline and API behavior.
        /// </summary>
        /// <remarks>
        /// This method sets up essential middleware and configurations for the application, including request localization, HTTPS redirection, routing, CORS policy, request
        /// handling, authentication, authorization, custom middlewares, and controller mapping.
        /// It also handles OpenAPI/Swagger configuration in the development environment.
        /// </remarks>
        /// <param name="application">The <see cref="WebApplication"/> instance to configure.</param>
        /// <returns>The configured <see cref="WebApplication"/> instance, allowing further modifications or starting the application.</returns>
        public static WebApplication UseApi(this WebApplication application)
        {
            string[] supportedCultures = ["pt-BR", "en-US"];

            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[1])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            if(application.Environment.IsDevelopment())
            {
                application.MapOpenApi();
                application.MapScalarApiReference((options) =>
                {
                    options.Authentication = new ScalarAuthenticationOptions
                    {
                        PreferredSecuritySchemes = ["Bearer"]
                    };
                    options.DarkMode = true;
                    options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.Shell, ScalarClient.Httpie);
                    options.DefaultOpenAllTags = false;
                    options.DocumentDownloadType = DocumentDownloadType.Json;
                    options.EnabledClients = [ScalarClient.Httpie];
                    options.ExpandAllModelSections = false;
                    options.ExpandAllResponses = false;
                    options.HideDarkModeToggle = false;
                    options.HideModels = true;
                    options.HideTestRequestButton = false;
                    options.Layout = ScalarLayout.Modern;
                    options.OperationSorter = OperationSorter.Alpha; // or by HTTP method.
                    options.SchemaPropertyOrder = PropertyOrder.Preserve; // or alpha.
                    options.ShowDeveloperTools = DeveloperToolsVisibility.Localhost; // or always or never.
                    options.ShowOperationId = false;
                    options.ShowSidebar = true;
                    options.SortTagsAlphabetically(); // The same as `options.TagSorter = TagSorter.Alpha`.
                    options.Telemetry = true;
                    options.Theme = ScalarTheme.BluePlanet;
                    options.Title = "GeekShopping - ProductAPI";
                });
            }

            application.UseRequestLocalization(localizationOptions);
            application.UseHttpsRedirection();
            application.UseRouting();
            application.UseCors("DefaultCors");
            application.UseAuthentication();
            application.UseAuthorization();
            application.UseMiddlewares();
            application.MapControllers();

            return application;
        }
    }
}