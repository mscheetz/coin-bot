using CoinBot.Business.Builders.Interface;
using CoinBot.Business.Entities;
using CoinBot.Core;
using CoinBot.Data;
using CoinBot.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Business.Builders
{
    public class PercentageTradeBuilder : IPercentageTradeBuilder
    {
        private IBinanceRepository _repo;
        private IFileRepository _fileRepo;
        private Helper _helper = new Helper();
        private ITradeBuilder _trader;
        private BotSettings botSettings;
        private string _symbol;
        private bool currentlyTrading;
        private TradeType tradeType;
        private decimal _lastBuy = 0.00000000M;
        private decimal _lastSell = 0.00000000M;
        private int tradeNumber = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public PercentageTradeBuilder()
        {
            _repo = new BinanceRepository();
            _fileRepo = new FileRepository();
            _trader = new TradeBuilder();
            SetupBuilder();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repo">Repository interface</param>
        public PercentageTradeBuilder(IBinanceRepository repo, IFileRepository fileRepo, ITradeBuilder trader)
        {
            _repo = repo;
            _fileRepo = fileRepo;
            _trader = trader;
            SetupBuilder();
        }

        private void SetupBuilder()
        {
            _trader = new TradeBuilder();
            SetBotSettings(_trader.GetBotSettings());
            //if (botSettings.tradingStrategy == Strategy.Percentage)
            //    RunBot(Interval.FiveM);
        }

        /// <summary>
        /// Set BotSettings
        /// </summary>
        /// <param name="settings">Updated Bot Settings</param>
        /// <returns>Boolean when complete</returns>
        public bool SetBotSettings(BotSettings settings)
        {
            botSettings = settings;
            _symbol = botSettings.tradingPair;
            currentlyTrading = botSettings.startBotAutomatically == null
                                ? false
                                : (bool)botSettings.startBotAutomatically;

            return true;
        }

        /// <summary>
        /// Start trading
        /// </summary>
        /// <param name="interval">Candlestick interval</param>
        public void StartTrading(Interval interval)
        {
            currentlyTrading = true;

            RunBot(interval);
        }

        /// <summary>
        /// Stop Trading
        /// </summary>
        public void StopTrading()
        {
            currentlyTrading = false;
        }

        /// <summary>
        /// Run Trading Bot
        /// </summary>
        /// <param name="interval">Candlestick Interval</param>
        private void RunBot(Interval interval)
        {
            _trader.SetupRepository();
            tradeType = TradeType.BUY;
            var currentStick = new Candlestick();
            var previousStick = new Candlestick();

            if (botSettings.tradingStatus == TradeStatus.PaperTrading)
                _trader.SetPaperBalance();

            while (currentlyTrading)
            {
                Task.WaitAll(Task.Delay(botSettings.priceCheck));
                var candleStickArray = _trader.GetCandlesticks(_symbol, interval, 5);
                int i = candleStickArray.Length - 1;
                currentStick = candleStickArray[i];

                if (previousStick.close == 0)
                    previousStick = candleStickArray[i];
                var tradeMade = false;

                if (_trader.StoppedOutCheck(currentStick.close))
                {
                    tradeType = TradeType.BUY;
                }
                tradeNumber++;

                if (tradeType == TradeType.BUY)
                {
                    if ((BuyPercentReached(currentStick.close)
                        && MooningAndTankingCheck(currentStick))
                        || tradeNumber == 1)
                    {
                        _trader.BuyCrypto(currentStick.close);
                        _lastBuy = currentStick.close;
                        tradeType = TradeType.SELL;
                        tradeMade = true;
                    }
                }
                else if (tradeType == TradeType.SELL)
                {
                    if (SellPercentReached(currentStick.close)
                        && MooningAndTankingCheck(currentStick))
                    {
                        _trader.SellCrypto(currentStick.close);
                        _lastSell = currentStick.close;
                        tradeType = TradeType.BUY;
                        tradeMade = true;
                    }
                }
                //if (tradeMade)
                //{
                //    _trader.UpdateBalances();
                //}
                previousStick = currentStick;
            }
        }

        /// <summary>
        /// Check if buy percent has been reached
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <returns>Boolean of result</returns>
        public bool BuyPercentReached(decimal currentPrice)
        {
            var percent = _helper.GetBuyPercent(currentPrice, _lastSell);

            return percent >= botSettings.buyPercent;
        }

        /// <summary>
        /// Check if sell percent has been reached
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <returns>Boolean of result</returns>
        public bool SellPercentReached(decimal currentPrice)
        {
            var percent = _helper.GetSellPercent(currentPrice, _lastBuy);

            return percent >= botSettings.sellPercent;
        }

        /// <summary>
        /// Check if mooning or tanking
        /// </summary>
        /// <param name="tradingStick">Current trading stick</param>
        /// <returns>Boolean of result</returns>
        public bool MooningAndTankingCheck(Candlestick tradingStick)
        {
            if(botSettings.mooningTankingTime == 0)
            {
                return false;
            }

            var nextStick = GetNextCandlestick();

            if(tradeType == TradeType.BUY)
            {
                if (tradingStick.close > nextStick.close 
                    && BuyPercentReached(nextStick.close))
                {
                    MooningAndTankingCheck(nextStick);
                }
                else
                {
                    return BuyPercentReached(nextStick.close) ? true : false;
                }
            }
            else
            {
                if (tradingStick.close < nextStick.close
                    && SellPercentReached(nextStick.close))
                {
                    MooningAndTankingCheck(nextStick);
                }
                else
                {
                    return SellPercentReached(nextStick.close) ? true : false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get next candlestick
        /// </summary>
        /// <returns>Candlestick object</returns>
        private Candlestick GetNextCandlestick()
        {
            Task.WaitAll(Task.Delay(botSettings.mooningTankingTime));

            var candlesticks = _trader.GetCandlesticks(_symbol, Interval.OneM, 1);

            return candlesticks[0];
        }
    }
}
