using Content.Shared.Roles;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Server._TT.StationHandleJob;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TTStationHandleJobComponent : Component
{
    [DataField, AutoNetworkedField]
    public List<ProtoId<JobPrototype>> Jobs = new();
}
