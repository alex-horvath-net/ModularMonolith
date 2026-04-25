using TradingPortal.Security;

namespace TradingPortal.Trader;

public sealed class PlaceTradeUserStory(ICurrentUser currentUser) {
    public PlaceTradeResult Execute(PlaceTradeRequest request) {
        if (!currentUser.IsAuthenticated) {
            return PlaceTradeResult.Fail("User is not authenticated.");
        }

        if (!currentUser.IsInRole("Trader")) {
            return PlaceTradeResult.Fail("User is not a trader.");
        }

        if (!currentUser.HasScope("trading.orders.write")) {
            return PlaceTradeResult.Fail("User has no permission to place trades.");
        }

        if (!string.Equals(currentUser.Desk, request.Desk, StringComparison.OrdinalIgnoreCase)) {
            return PlaceTradeResult.Fail("Trader cannot place trades for another desk.");
        }

        return PlaceTradeResult.Success(
            TradeId: Guid.NewGuid(),
            Message: $"Trade accepted for {currentUser.UserName} on {currentUser.Desk} desk.");
    }
}