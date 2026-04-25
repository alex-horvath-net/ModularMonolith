namespace TradingPortal.Trader;

public sealed record PlaceTradeResult(
    bool IsSuccess,
    Guid? TradeId,
    string Message) {
    public static PlaceTradeResult Success(Guid TradeId, string Message) =>
        new(true, TradeId, Message);

    public static PlaceTradeResult Fail(string Message) =>
        new(false, null, Message);
}