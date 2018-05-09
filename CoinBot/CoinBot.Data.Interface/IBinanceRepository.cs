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

        Task<Account> GetBalance();

        Task<IEnumerable<BinanceTick>> GetCrytpos();

        Task<IEnumerable<Candlestick>> GetCandlestick(string symbol, Interval interval);

        long GetBinanceTime();
    }
}
