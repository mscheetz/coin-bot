// -----------------------------------------------------------------------------
// <copyright file="Enums" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="2/15/2019 4:53:19 PM" />
// -----------------------------------------------------------------------------

using System.ComponentModel;

namespace coinbot.strategies.zzz_Old_Bot
{

    public enum BandStatus
    {
        Above,
        Inside,
        Below
    }
    public enum Exchange
    {
        NONE,
        BINANCE,
        GDAX,
        KUCOIN
    }

    public enum Interval
    {
        None,
        [Description("1m")]
        OneM,
        [Description("3m")]
        ThreeM,
        [Description("5m")]
        FiveM,
        [Description("15m")]
        FifteenM,
        [Description("30m")]
        ThirtyM,
        [Description("1h")]
        OneH,
        [Description("2h")]
        TwoH,
        [Description("4h")]
        FourH,
        [Description("6h")]
        SixH,
        [Description("8h")]
        EightH,
        [Description("12h")]
        TwelveH,
        [Description("1d")]
        OneD,
        [Description("3d")]
        ThredD,
        [Description("1w")]
        OneW,
        [Description("1M")]
        OneMo
    }

    public enum SignalType
    {
        None,
        Percent,
        Volume,
        BollingerBandUpper,
        BollingerBandLower,
        OrderBook
    }

    public enum Strategy
    {
        None,
        BollingerBands,
        OrderBook,
        Percentage,
        Volume
    }
    public enum TradeStatus
    {
        None,
        LiveTrading,
        PaperTrading
    }

    public enum TradeType
    {
        [Description("NONE")]
        NONE,
        [Description("BUY")]
        BUY,
        [Description("VOLUMEBUY")]
        VOLUMEBUY,
        [Description("VOLUMEBUYSELLOFF")]
        VOLUMEBUYSELLOFF,
        [Description("SELL")]
        SELL,
        [Description("VOLUMESELL")]
        VOLUMESELL,
        [Description("VOLUMESELLBUYOFF")]
        VOLUMESELLBUYOFF,
        [Description("STOPLOSS")]
        STOPLOSS,
        [Description("ORDERBOOKBUY")]
        ORDERBOOKBUY,
        [Description("ORDERBOOKSELL")]
        ORDERBOOKSELL,
        [Description("CANCELTRADE")]
        CANCELTRADE
    }
}