using Microsoft.Extensions.ML;
using System.Net;
using WillIBeHome.ML;
using WillIBeHome.Owntracks;
using WillIBeHome.Shared;

namespace WillIBeHome.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        OwntracksSettings? owntracksSettings = Configuration.GetSection("Owntracks").Get<OwntracksSettings>();
        services.AddHttpClient<IOwntracksApiClient, OwntracksApiClient>(c =>
        {
            c.BaseAddress = owntracksSettings.Uri;
        }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler { Credentials = new NetworkCredential(owntracksSettings.HttpUserName, owntracksSettings.HttpPassword) });

        MLSettings? mlSettings = Configuration.GetSection("ML").Get<MLSettings>();
        services.AddPredictionEnginePool<Transition, WillBeHomePrediction>()
            .FromFile(mlSettings.ModelPath);

        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
