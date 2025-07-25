// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.NPC.Queries;
using Content.Server.NPC.Systems;
using Robust.Shared.Map;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators;

/// <summary>
/// Utilises a <see cref="UtilityQueryPrototype"/> to determine the best target and sets it to the Key.
/// </summary>
public sealed partial class UtilityOperator : HTNOperator
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    [DataField("key")] public string Key = "Target";

    /// <summary>
    /// The EntityCoordinates of the specified target.
    /// </summary>
    [DataField("keyCoordinates")]
    public string KeyCoordinates = "TargetCoordinates";

    [DataField("proto", required: true, customTypeSerializer:typeof(PrototypeIdSerializer<UtilityQueryPrototype>))]
    public string Prototype = string.Empty;

    public override async Task<(bool Valid, Dictionary<string, object>? Effects)> Plan(NPCBlackboard blackboard,
        CancellationToken cancelToken)
    {
        var result = _entManager.System<NPCUtilitySystem>().GetEntities(blackboard, Prototype);
        var target = result.GetHighest();

        if (!target.IsValid())
        {
            return (false, new Dictionary<string, object>());
        }

        var effects = new Dictionary<string, object>()
        {
            {Key, target},
            {KeyCoordinates, new EntityCoordinates(target, Vector2.Zero)}
        };

        return (true, effects);
    }
}
