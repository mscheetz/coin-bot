// -----------------------------------------------------------------------------
// <copyright file="RangeService" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/24/2019 7:50:32 PM" />
// -----------------------------------------------------------------------------

namespace coinbot.strategies
{
    using coinbot.strategies.Contracts.Interfaces;
    using coinbot.strategies.Contracts.Models;
    #region Usings

    using ExchangeHub;
    using ExchangeHub.Contracts;
    using System.Linq;
    using System.Threading.Tasks;

    #endregion Usings

    public class RangeService : IRangeService
    {
        #region Properties

        private Settings _settings;
        private ExchangeHub _exchange = null;
        private decimal _tradeBalance = 0.0M;
        private decimal _baseBalance = 0.0M;

        #endregion Properties

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Bot settings</param>
        /// <param name="exchange">Exchange to trade on</param>
        public RangeService(Settings settings, ExchangeHub exchange)
        {
            this._settings = settings;
            this._exchange = exchange;
        }

        /// <summary>
        /// Run bot
        /// </summary>
        public async Task RunBot()
        {
            await Task.Run(async () =>
            {

                var timeout = GetTimeout(_settings.TimeInterval);

                await CheckBalances(_settings.Pair);
                var klines = await _exchange.GetKLinesAsync(_settings.Pair, _settings.TimeInterval, 1);

                await StopOutCheck(klines[0].Close);
                await BuyCheck(klines[0].Close);
                await SellCheck(klines[0].Close);

                // wait to run
                await Task.Delay((int)timeout);
            });
        }

        /// <summary>
        /// Check balances for current pair
        /// </summary>
        /// <param name="pair">Pair trading against</param>
        private async Task CheckBalances(string pair)
        {
            var balances = await _exchange.GetBalanceAsync();

            _tradeBalance = balances.Where(b => pair.StartsWith(b.Symbol)).Select(b => b.Available).FirstOrDefault();
            _baseBalance = balances.Where(b => pair.EndsWith(b.Symbol)).Select(b => b.Available).FirstOrDefault();
        }

        /// <summary>
        /// Check if in buy range and available balance
        /// If so BUY
        /// </summary>
        /// <param name="lastPrice">Last price</param>
        private async Task BuyCheck(decimal lastPrice)
        {
            if (_baseBalance > 0
                && (lastPrice > _settings.BuyLow || lastPrice < _settings.BuyHigh))
            {
                var quantity = (GetTradeQuantity(_baseBalance, Side.Buy) / lastPrice);
                var order = await _exchange.LimitOrderAsync(_settings.Pair, quantity, lastPrice, Side.Buy);
            }
        }

        /// <summary>
        /// Check if in sell range and available balance
        /// If so SELL
        /// </summary>
        /// <param name="lastPrice">Last price</param>
        private async Task SellCheck(decimal lastPrice)
        {
            if (_tradeBalance > 0
                && (lastPrice > _settings.SellLow || lastPrice < _settings.SellHigh))
            {
                var quantity = GetTradeQuantity(_tradeBalance, Side.Sell);
                var order = await _exchange.LimitOrderAsync(_settings.Pair, quantity, lastPrice, Side.Sell);
            }
        }

        /// <summary>
        /// StopOut check
        /// </summary>
        /// <param name="lastPrice">Last price</param>
        private async Task StopOutCheck(decimal lastPrice)
        {
            if (lastPrice <= _settings.StopPrice)
            {
                var order = await _exchange.MarketOrderAsync(_settings.Pair, _tradeBalance, Side.Sell);
            }
        }

        /// <summary>
        /// Calculate trade quantity
        /// </summary>
        /// <param name="balance">Balance of coin</param>
        /// <param name="side">Trade side</param>
        /// <returns>Decimal of quantity</returns>
        private decimal GetTradeQuantity(decimal balance, Side side)
        {
            var quantity = 0.0M;
            if(side == Side.Buy)
            {
                quantity = balance > _settings.BuyLimit ? _settings.BuyLimit : balance;
            }
            else if(side == Side.Sell)
            {
                quantity = balance > _settings.SellLimit ? _settings.SellLimit : balance;
            }

            return quantity;
        }

        /// <summary>
        /// Get timeout in ms from time interval
        /// </summary>
        /// <param name="timeInterval">TimeInterval to convert</param>
        /// <returns>time in milliseconds</returns>
        private long GetTimeout(TimeInterval timeInterval)
        {
            var msPerMinute = 60000;
            var msPerHour = 3600000;

            switch (timeInterval)
            {
                case TimeInterval.EightH:
                    return 8 * msPerHour;
                case TimeInterval.FifteenM:
                    return 15 * msPerMinute;
                case TimeInterval.FiveM:
                    return 5 * msPerMinute;
                case TimeInterval.FourH:
                    return 4 * msPerHour;
                case TimeInterval.None:
                    return 0;
                case TimeInterval.OneD:
                    return 24 * msPerHour;
                case TimeInterval.OneH:
                    return 1 * msPerHour;
                case TimeInterval.OneM:
                    return 1 * msPerMinute;
                case TimeInterval.OneMo:
                    return 30 * 24 * msPerHour;
                case TimeInterval.OneW:
                    return 7 * 24 * msPerHour;
                case TimeInterval.SixH:
                    return 6 * msPerHour;
                case TimeInterval.ThirtyM:
                    return 30 * msPerMinute;
                case TimeInterval.ThreeD:
                    return 3 * 24 * msPerHour;
                case TimeInterval.ThreeM:
                    return 3 * msPerMinute;
                case TimeInterval.TwelveH:
                    return 12 * msPerHour;
                case TimeInterval.TwoH:
                    return 2 * msPerHour;
                default:
                    return 0;
            }
        }
    }
}