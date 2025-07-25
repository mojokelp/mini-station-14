using Robust.Shared.Prototypes;

namespace Content.Shared._CorvaxNext.TurretControl.Components;

[RegisterComponent]
public sealed partial class CorvaxTurretControllerComponent : Component
{
    [DataField]
    public ComponentRegistry RequiredComponents = [];
}
