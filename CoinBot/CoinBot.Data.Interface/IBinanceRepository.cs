using CoinBot.Business.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Data.Interface
{
    public interface IBinanceRepository
    {
        bool SetExchangeApi(ApiInformation exchangeApi);

        Task<IEnumerable<Transaction>> GetTransactions();

        Task<IEnumerable<BinanceTick>> GetCrytpos();

        Task<Candlestick[]> GetCandlestick(string symbol, Interval interval, int limit = 500);

        Task<Account> GetBalance();

        Task<TradeResponse> PostTrade(TradeParams tradeParams);

        Task<TradeResponse> DeleteTrade(CancelTradeParams tradeParams);

        long GetBinanceTime();
    }
}
