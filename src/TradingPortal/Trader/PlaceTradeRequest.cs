namespace TradingPortal.Trader;

public sealed record PlaceTradeRequest(
    string Desk,
    string Symbol,
    decimal Quantity);
