using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Builders.Mapping;
using CoinBot.Data;
using CoinBot.Data.Interface;
using CoinBot.Manager;
using CoinBot.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace CoinBot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Mappings.RegisterMappings();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddScoped<IBinanceRepository, BinanceRepository>();
            services.AddScoped<IGdaxRepository, GdaxRepository>();
            services.AddScoped<IKuCoinRepository, KuCoinRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddTransient<IBollingerBandTradeBuilder, BollingerBandTradeBuilder>();
            services.AddTransient<IVolumeTradeBuilderOG, VolumeTradeBuilderOG>();
            services.AddTransient<IOrderBookTradeBuilder, OrderBookTradeBuilder>();
            services.AddTransient<IVolumeTradeBuilder, VolumeTradeBuilder>();
            services.AddTransient<ITradeBuilder, TradeBuilder>();
            services.AddTransient<IExchangeBuilder, ExchangeBuilder>();
            services.AddTransient<ICoinBotService, CoinBotManager>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "CoinBot API",
                    Description = "RESTful API endpoints for CoinBot",
                    Contact = new Contact { Name = "CryptoBitfolio", Email = "cryptoBitfolio@gmail.com", Url = "https://twitter.com/CryptoBitfolio" },
                    Version = "1.0"

                });
                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "project.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoinBot API V1");
            });

            app.UseMvc();
        }
    }
}
