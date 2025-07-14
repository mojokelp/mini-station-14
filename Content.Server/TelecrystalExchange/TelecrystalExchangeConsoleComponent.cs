using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.TelecrystalExchange
{
    [RegisterComponent]
    public sealed partial class TelecrystalExchangeConsoleComponent : Component
    {
        [DataField("exchangeRate")]
        public int ExchangeRate = 5;

        [DataField("minPointsToExchange")]
        public int MinPointsToExchange = 1000;

        [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
        public TimeSpan LastExchangeTime;

        [DataField]
        public TimeSpan ExchangeCooldown = TimeSpan.FromSeconds(30);
    }
}
