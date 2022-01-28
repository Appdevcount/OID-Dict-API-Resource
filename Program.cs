using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using ResourceServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IServiceCollection services = builder.Services;
services.AddRouting(o => o.LowercaseUrls = true);

services.AddAuthentication(options =>
{
    options.DefaultScheme
      = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    //.AddJwtBearer(options =>
    //{
    //    options.Authority = "https://localhost:7236/";
    //    options.Audience="api_service";
    //    options.
    //})
    //.AddOAuthIntrospection(options =>
    //{
    //    options.Authority = new Uri(OAuthConfig.Authority);
    //    options.Audiences.Add(OAuthConfig.Audience);
    //    options.ClientId = OAuthConfig.ClientId;
    //    options.ClientSecret = OAuthConfig.ClientSecret;
    //    if (environment.IsDevelopment())
    //        options.RequireHttpsMetadata = false;
    //});
//.AddOAuth(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, options =>
//{
//    options.AuthorizationEndpoint = "https://localhost:7236/";
//    options.ClientId = "api_service";
//    options.ClientSecret = "my-api-secret";
//    options.aut = true;
//    options.AutomaticChallenge = true;
//})
//.AddOAuth2Introspection(options =>
//{
//    options.Authority = "https://localhost:7236/";
//    options.Audiences.Add(OAuthConfig.Audience);
//    options.ClientId = "api_service";
//    options.ClientSecret = "my-api-secret";
//    options.
//});
;
// Register the OpenIddict validation components.
services.AddOpenIddict()
  .AddValidation(options =>
  {
      // Note: the validation handler uses OpenID Connect discovery
      // to retrieve the address of the introspection endpoint.
      options.SetIssuer("https://localhost:7236/");
      options.AddAudiences("api_service");//"mvc");// 

      // Configure the validation handler to use introspection and register the client
      // credentials used when communicating with the remote introspection endpoint.
      options.UseIntrospection()
                   .SetClientId("api_service")
                   .SetClientSecret("my-api-secret");

      // Register the System.Net.Http integration.
      options.UseSystemNetHttp();

      // Register the ASP.NET Core host.
      options.UseAspNetCore();
  })

   ;

services.AddControllers();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
});




//services.AddScoped<IAuthorizationHandler, RequireScopeHandler>();

//services.AddAuthorization(options =>
//{
//    options.AddPolicy("dataEventRecordsPolicy", policyUser =>
//    {
//        policyUser.Requirements.Add(new RequireScope());
//    });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseCors(builder =>
    {
        builder.WithOrigins("https://localhost:7286/", "https://localhost:7286/");
        builder.WithMethods("GET");
        builder.WithMethods("POST");
        builder.WithHeaders("Authorization");
        //.AllowCredentials()
        //.SetIsOriginAllowedToAllowWildcardSubdomains()
        //builder.AllowAnyOrigin();
    });

    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseJwtBearerAuthentication(new JwtBearerOptions
//{
//    Authority = "https://localhost:7236/",
//    Audience = "http://localhost:5000",
//    RequireHttpsMetadata = false,
//    c
//});
//app.u(options =>
//{
//    options.Authority = "http://localhost:5000/";
//    options.AutomaticAuthenticate = true;
//    options.AutomaticChallenge = true;
//    options.Audiences.Add("http://localhost:5001/");
//    options.ClientId = "resource_server";
//    options.ClientSecret = Crypto.HashPassword("secret_secret_secret");
//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();