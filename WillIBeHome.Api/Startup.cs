namespace WillIBeHome.Api
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.ML;
    using System.Net;
    using System.Net.Http;
    using WillIBeHome.ML;
    using WillIBeHome.Owntracks;
    using WillIBeHome.Shared;

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
            var owntracksSettings = this.Configuration.GetSection("Owntracks").Get<OwntracksSettings>();
            services.AddHttpClient<IOwntracksApiClient, OwntracksApiClient>(c =>
            {
                c.BaseAddress = owntracksSettings.Uri;
            }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler { Credentials = new NetworkCredential(owntracksSettings.HttpUserName, owntracksSettings.HttpPassword) });

            var mlSettings = this.Configuration.GetSection("ML").Get<MLSettings>();
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
}
