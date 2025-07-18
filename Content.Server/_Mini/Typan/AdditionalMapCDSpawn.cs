using Content.Server._TT.AdditionalMap;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Maps;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.Shuttles.Systems;
using Content.Shared.Shuttles.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.GameTicking.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server._Mini.Typan
{
    public sealed class AdditionalMapCDSpawnSystem : EntitySystem
    {
        [Dependency] private readonly IPrototypeManager _prototype = default!;
        [Dependency] private readonly ItemSlotsSystem _slots = default!;
		[Dependency] private readonly SharedMapSystem _map = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<PostGameMapLoad>(OnPostGameMapLoad);
        }

        private void OnPostGameMapLoad(PostGameMapLoad ev)
		{
			var prototypes = _prototype.EnumeratePrototypes<TTAdditionalMapPrototype>();
			TTAdditionalMapPrototype? matched = null;
			foreach (var proto in prototypes)
			{
				if (proto.MapProtoIds.Contains(ev.GameMap.ID))
				{
					matched = proto;
					break;
				}
			}

			if (matched == null)
			{
				return;
			}
			SpawnCDInConsoles(ev.Map);
		}


        private void SpawnCDInConsoles(MapId mapId)
        {
			var mapUid = _map.GetMap(mapId);

            var shuttleEnt = FindShuttleOnMap(mapId);

            var query = EntityQueryEnumerator<ShuttleConsoleComponent, ItemSlotsComponent, TransformComponent>();

            while (query.MoveNext(out var ent, out _, out var slots, out var transform))
            {
                if (transform.MapID != mapId)
                    continue;
				
                var disk = Spawn("CoordinatesDisk", transform.Coordinates);

                var dest = EnsureComp<ShuttleDestinationCoordinatesComponent>(disk);
                if (shuttleEnt != null)
                {
                    dest.Destination = mapUid;
                }

                Dirty(disk, dest);

                var inserted = _slots.TryInsert(ent, SharedShuttleConsoleComponent.DiskSlotName, disk, null, slots);
            }
        }

        private EntityUid? FindShuttleOnMap(MapId mapId)
        {
            var query = EntityQueryEnumerator<ShuttleComponent, TransformComponent>();
            while (query.MoveNext(out var ent, out _, out var transform))
            {
                if (transform.MapID == mapId)
                    return ent;
            }
            return null;
        }
    }
}
