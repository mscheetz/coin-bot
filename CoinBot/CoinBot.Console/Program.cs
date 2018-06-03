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
            //var serviceProvider = new ServiceCollection()
            //    .AddLogging()
            //    .AddScoped<IBinanceRepository, BinanceRepository>()
            //    .AddScoped<IGdaxRepository, GdaxRepository>()
            //    .AddScoped<IFileRepository, FileRepository>()
            //    .AddTransient<IBollingerBandTradeBuilder, BollingerBandTradeBuilder>()
            //    .AddTransient<IPercentageTradeBuilder, PercentageTradeBuilder>()
            //    .AddTransient<ITradeBuilder, TradeBuilder>()
            //    .AddTransient<IExchangeBuilder, ExchangeBuilder>()
            //    .AddTransient<ICoinBotService, CoinBotManager>()
            //    .BuildServiceProvider();

            ////serviceProvider
            ////    .GetService<ILoggerFactory>()
            ////    .AddConsole(LogLevel.Debug);

            //var logger = serviceProvider.GetService<ILoggerFactory>()
            //    .CreateLogger<Program>();
            //logger.LogDebug("Starting bot");

            //var input = System.Console.ReadLine();

            //ProcessSwitches(serviceProvider, args);
        }

        //private static void ProcessSwitches(ServiceProvider serviceProvider, string[] args)
        //{
        //    var svc = serviceProvider.GetService<ICoinBotService>();
        //    if (args.Length == 0)
        //    {
        //        BotHelp();
        //    }
        //    foreach (string arg in args)
        //    {
        //        switch(arg.ToLower())
        //        {
        //            case "h":
        //                BotHelp();
        //                break;
        //            case "r":
        //                System.Console.WriteLine("Starting bot...");
        //                svc.StartBot(Business.Entities.Interval.OneM);
        //                break;
        //            default:
        //                BotHelp();
        //                break;
        //        }
        //    }
        //    svc.StartBot(Business.Entities.Interval.OneM);
        //}

        private static void BotHelp()
        {
            System.Console.WriteLine("/////////////////");
            System.Console.WriteLine("  Bot Help menu");
            System.Console.WriteLine("/////////////////");

            System.Console.WriteLine("Switches:");
            System.Console.WriteLine("-h    help menu");
            System.Console.WriteLine("-r    run bot");
        }
    }
}
