using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Storage.EntitySystems;
using Content.Shared.Access.Systems;
using Content.Shared.Research.Components;
using Content.Shared.Storage;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using Robust.Shared.Timing; // Для GameTiming
using Robust.Shared.Utility; // Для EntitySpawnEntry
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Content.Shared.Power;
using Robust.Shared.Audio.Systems;
using Content.Shared.TelecrystalExchange;
using Content.Shared.Research.Components;
using Content.Server.Research.Systems;
using Content.Server.Research.Disk;

namespace Content.Server.TelecrystalExchange
{
    public sealed class TelecrystalExchangeSystem : EntitySystem
    {
        [Dependency] private readonly AccessReaderSystem _access = default!;
        [Dependency] private readonly ResearchDiskSystem _diskSystem = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;
        [Dependency] private readonly UserInterfaceSystem _ui = default!;
        [Dependency] private readonly StorageSystem _storage = default!;
        [Dependency] private readonly SharedContainerSystem _container = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<TelecrystalExchangeConsoleComponent, BoundUIOpenedEvent>(OnUIOpened);
            SubscribeLocalEvent<TelecrystalExchangeConsoleComponent, TelecrystalExchangeMessage>(OnExchangeMessage);
            SubscribeLocalEvent<TelecrystalExchangeConsoleComponent, PowerChangedEvent>(OnPowerChanged);
        }

        private void OnPowerChanged(EntityUid uid, TelecrystalExchangeConsoleComponent component, ref PowerChangedEvent args)
        {
            if (!args.Powered)
                _ui.TryCloseAll(uid);
        }

        private void OnUIOpened(EntityUid uid, TelecrystalExchangeConsoleComponent component, BoundUIOpenedEvent args)
        {
            if (!this.IsPowered(uid, EntityManager))
                return;

            UpdateUi(uid, component);
        }

        private void OnExchangeMessage(EntityUid uid, TelecrystalExchangeConsoleComponent component, TelecrystalExchangeMessage args)
        {
            if (!this.IsPowered(uid, EntityManager))
                return;

            if (!_access.IsAllowed(args.Actor, uid))
            {
                DenyExchange(uid, "telecrystal-exchange-error-access");
                return;
            }

            var time = GameTiming.CurTime;
            if (time < component.LastExchangeTime + component.ExchangeCooldown)
                return;

            if (args.Disk == null || !Exists(args.Disk))
            {
                DenyExchange(uid, "telecrystal-exchange-error-no-disk");
                return;
            }

            if (!TryComp<ResearchDiskComponent>(args.Disk, out var disk))
            {
                DenyExchange(uid, "telecrystal-exchange-error-no-disk");
                return;
            }

            if (disk.Points < component.MinPointsToExchange)
            {
                DenyExchange(uid, "telecrystal-exchange-error-not-enough");
                return;
            }

            var crystalsToGive = disk.Points / component.ExchangeRate;
            _diskSystem.DeleteDisk(args.Disk.Value);
            component.LastExchangeTime = time;

            SpawnTelecrystalsNearConsole(uid, crystalsToGive);
            _audio.PlayPvs("/Audio/Machines/ding.ogg", uid);

            UpdateUi(uid, component);
        }

        private void DenyExchange(EntityUid uid, string reason)
        {
            _audio.PlayPvs("/Audio/Machines/buzz-sigh.ogg", uid);
            UpdateUi(uid, GetComp<TelecrystalExchangeConsoleComponent>(uid), reason);
        }

        private void SpawnTelecrystalsNearConsole(EntityUid consoleUid, int amount)
        {
            if (amount <= 0)
                return;

            var xform = Transform(consoleUid);
            var spawnPos = xform.MapPosition.Offset(new Vector2(0.5f, 0));

            var crystalProto = "MaterialTelecrystal";
            var spawnEntities = new List<EntitySpawnEntry>
            {
                new(crystalProto, amount)
            };

            _storage.SpawnEntitiesInRange(spawnPos, spawnEntities, 0.2f);
        }

        private void UpdateUi(EntityUid uid, TelecrystalExchangeConsoleComponent component, string? message = null)
        {
            var state = new TelecrystalExchangeBoundUserInterfaceState(
                component.ExchangeRate,
                component.MinPointsToExchange,
                message);

            _ui.TrySetUiState(uid, TelecrystalExchangeUiKey.Key, state);
        }
    }
}
