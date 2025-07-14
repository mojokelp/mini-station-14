using Robust.Shared.Serialization;

namespace Content.Shared.TelecrystalExchange
{
    [Serializable, NetSerializable]
    public sealed class TelecrystalExchangeBoundUserInterfaceState : BoundUserInterfaceState
    {
        public int ExchangeRate { get; }
        public int MinPointsToExchange { get; }
        public string? Message { get; }

        public TelecrystalExchangeBoundUserInterfaceState(int exchangeRate, int minPoints, string? message = null)
        {
            ExchangeRate = exchangeRate;
            MinPointsToExchange = minPoints;
            Message = message;
        }
    }

    [Serializable, NetSerializable]
    public sealed class TelecrystalExchangeMessage : BoundUserInterfaceMessage
    {
        public EntityUid Actor { get; }
        public EntityUid? Disk { get; }

        public TelecrystalExchangeMessage(EntityUid actor, EntityUid? disk)
        {
            Actor = actor;
            Disk = disk;
        }
    }

    [NetSerializable, Serializable]
    public enum TelecrystalExchangeUiKey
    {
        Key
    }
}