using Users.WebAPI;

WebApplicationBuilder applicationBuilder = WebApplication.CreateBuilder(args);

applicationBuilder
    .AddApiServices()
    .AddDatabaseConfiguration()
    .AddCorsPolicy();

WebApplication application = applicationBuilder
    .Build()
    .UseApi();

await application.RunAsync();