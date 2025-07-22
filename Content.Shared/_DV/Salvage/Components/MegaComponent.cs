using Robust.Shared.GameStates;

namespace Content.Shared.ADT.Salvage.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class MegaComponent : Component
{
    [DataField]
    public bool Hardmode = false;
}
