using CoinBot.Business.Builders;
using CoinBot.Business.Builders.Interface;
using CoinBot.Data;
using CoinBot.Data.Interface;
using CoinBot.Manager;
using CoinBot.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;

namespace CoinBot.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddScoped<IBinanceRepository, BinanceRepository>()
                .AddScoped<IGdaxRepository, GdaxRepository>()
                .AddScoped<IFileRepository, FileRepository>()
                .AddTransient<IBollingerBandTradeBuilder, BollingerBandTradeBuilder>()
                .AddTransient<IPercentageTradeBuilder, PercentageTradeBuilder>()
                .AddTransient<ITradeBuilder, TradeBuilder>()
                .AddTransient<IExchangeBuilder, ExchangeBuilder>()
                .AddTransient<ICoinBotService, CoinBotManager>()
                .BuildServiceProvider();

            //serviceProvider
            //    .GetService<ILoggerFactory>()
            //    .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting bot");

            var svc = serviceProvider.GetService<ICoinBotService>();
            svc.StartBot(Business.Entities.Interval.OneM);
        }
    }
}
