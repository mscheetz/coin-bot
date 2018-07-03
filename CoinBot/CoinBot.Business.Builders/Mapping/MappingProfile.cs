using AutoMapper;
using CoinBot.Business.Entities;
using CoinBot.Business.Entities.KuCoinEntities;
using GDAXSharp.Services.Products.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoinBot.Business.Builders.Mapping
{
    public class MappingProfile : Profile, IProfile
    {
        public MappingProfile()
        {

            //CreateMap<BotStick, Candlestick>()
            //    .ForMember(c => c.close, map => map.MapFrom(b => b.close))
            //    .ForMember(c => c.closeTime, map => map.MapFrom(b => b.closeTime))
            //    .ForMember(c => c.high, map => map.MapFrom(b => b.high))
            //    .ForMember(c => c.low, map => map.MapFrom(b => b.low))
            //    .ForMember(c => c.open, map => map.MapFrom(b => b.open))
            //    .ForMember(c => c.openTime, map => map.MapFrom(b => b.openTime))
            //    .ForMember(c => c.volume, map => map.MapFrom(b => b.volume));

            CreateMap<Candlestick, BotStick>()
                .ForMember(b => b.close, map => map.MapFrom(c => c.close))
                .ForMember(b => b.closeTime, map => map.MapFrom(c => c.closeTime))
                .ForMember(b => b.high, map => map.MapFrom(c => c.high))
                .ForMember(b => b.low, map => map.MapFrom(c => c.low))
                .ForMember(b => b.open, map => map.MapFrom(c => c.open))
                .ForMember(b => b.openTime, map => map.MapFrom(c => c.openTime))
                .ForMember(b => b.volume, map => map.MapFrom(c => c.volume))
                .ForMember(b => b.bollingerBand, opt => opt.Ignore())
                .ForMember(b => b.volumeChange, opt => opt.Ignore())
                .ForMember(b => b.volumePercentChange, opt => opt.Ignore());

            //CreateMap<BotStick, Candle>()
            //    .ForMember(c => (decimal)c.Close, map => map.MapFrom(b => b.close))
            //    .ForMember(c => c.Time, map => map.MapFrom(b => b.closeTime))
            //    .ForMember(c => (decimal)c.High, map => map.MapFrom(b => b.high))
            //    .ForMember(c => (decimal)c.Low, map => map.MapFrom(b => b.low))
            //    .ForMember(c => (decimal)c.Open, map => map.MapFrom(b => b.open))
            //    .ForMember(c => c.Volume, map => map.MapFrom(b => b.volume));

            CreateMap<Candle, BotStick>()
                .ForMember(b => b.close, map => map.MapFrom(c => (decimal)c.Close))
                .ForMember(b => b.closeTime, map => map.MapFrom(c => c.Time))
                .ForMember(b => b.high, map => map.MapFrom(c => (decimal)c.High))
                .ForMember(b => b.low, map => map.MapFrom(c => (decimal)c.Low))
                .ForMember(b => b.open, map => map.MapFrom(c => (decimal)c.Open))
                .ForMember(b => b.volume, map => map.MapFrom(c => c.Volume))
                .ForMember(b => b.bollingerBand, opt => opt.Ignore())
                .ForMember(b => b.openTime, opt => opt.Ignore())
                .ForMember(b => b.volumeChange, opt => opt.Ignore())
                .ForMember(b => b.volumePercentChange, opt => opt.Ignore());

            CreateMap<TradeResponse, GDAXSharp.Services.Orders.Models.Responses.OrderResponse>()
                .ForMember(o => o.Id, map => map.MapFrom(t => t.clientOrderId))
                .ForMember(o => o.ExecutedValue, map => map.MapFrom(t => t.executedQty))
                .ForMember(o => o.Id, map => map.MapFrom(t => t.orderId))
                .ForMember(o => o.Size, map => map.MapFrom(t => t.origQty))
                .ForMember(o => o.Price, map => map.MapFrom(t => t.price))
                .ForMember(o => o.Side, map => map.MapFrom(t => t.side))
                .ForMember(o => o.Status, map => map.MapFrom(t => t.status))
                .ForMember(o => o.ProductId, map => map.MapFrom(t => t.symbol))
                .ForMember(o => o.TimeInForce, map => map.MapFrom(t => t.timeInForce))
                .ForMember(o => o.OrderType, map => map.MapFrom(t => t.type));

            CreateMap<OrderResponse, GDAXSharp.Services.Orders.Models.Responses.OrderResponse>()
                .ForMember(o => o.Id, map => map.MapFrom(t => t.clientOrderId))
                .ForMember(o => o.ExecutedValue, map => map.MapFrom(t => t.executedQty))
                .ForMember(o => o.Id, map => map.MapFrom(t => t.orderId))
                .ForMember(o => o.Size, map => map.MapFrom(t => t.origQty))
                .ForMember(o => o.Price, map => map.MapFrom(t => t.price))
                .ForMember(o => o.Side, map => map.MapFrom(t => t.side))
                .ForMember(o => o.Status, map => map.MapFrom(t => t.status))
                .ForMember(o => o.ProductId, map => map.MapFrom(t => t.symbol))
                .ForMember(o => o.TimeInForce, map => map.MapFrom(t => t.timeInForce))
                .ForMember(o => o.OrderType, map => map.MapFrom(t => t.type));

            CreateMap<BinanceOrders, Entities.KuCoinEntities.OrderBook>()
                .ForMember(o => o.pairTotal, map => map.MapFrom(b => b.ignore))
                .ForMember(o => o.price, map => map.MapFrom(b => b.price))
                .ForMember(o => o.quantity, map => map.MapFrom(b => b.quantity));
            
            CreateMap<OrderResponse, Entities.KuCoinEntities.OpenOrderDetail>()
                .ForMember(o => o.filledQuantity, map => map.MapFrom(b => b.executedQty))
                .ForMember(o => o.orderId, map => map.MapFrom(b => b.orderId))
                .ForMember(o => o.price, map => map.MapFrom(b => b.price))
                .ForMember(o => o.quantity, map => map.MapFrom(b => b.origQty))
                .ForMember(o => o.timestamp, map => map.MapFrom(b => b.time))
                .ForMember(o => o.type, map => map.MapFrom(b => b.type));

            CreateMap<OrderResponse, Entities.KuCoinEntities.OrderListDetail>()
                .ForMember(o => o.amount, map => map.MapFrom(b => b.executedQty))
                .ForMember(o => o.oid, map => map.MapFrom(b => b.orderId))
                .ForMember(o => o.dealPrice, map => map.MapFrom(b => b.price))
                .ForMember(o => o.amount, map => map.MapFrom(b => b.origQty))
                .ForMember(o => o.createdAt, map => map.MapFrom(b => b.time))
                .ForMember(o => o.direction, map => map.MapFrom(b => b.type));
        }
    }
}
