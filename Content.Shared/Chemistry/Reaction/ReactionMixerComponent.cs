// SPDX-FileCopyrightText: 2022 Timothy Teakettle <59849408+timothyteakettle@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 Verm <32827189+Vermidia@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.Components;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Reaction;

[RegisterComponent]
public sealed partial class ReactionMixerComponent : Component
{
    /// <summary>
    ///     A list of IDs for categories of reactions that can be mixed (i.e. HOLY for a bible, DRINK for a spoon)
    /// </summary>
    [ViewVariables]
    [DataField]
    public List<ProtoId<MixingCategoryPrototype>> ReactionTypes = default!;

    /// <summary>
    ///     A string which identifies the string to be sent when successfully mixing a solution
    /// </summary>
    [ViewVariables]
    [DataField]
    public LocId MixMessage = "default-mixing-success";

    /// <summary>
    ///     Defines if interacting is enough to mix with this component
    /// </summary>
    [ViewVariables]
    [DataField]
    public bool MixOnInteract = true;

    /// <summary>
    ///     How long it takes to mix with this
    /// </summary>
    [ViewVariables]
    [DataField]
    public TimeSpan TimeToMix = TimeSpan.Zero;
}

[ByRefEvent]
public record struct MixingAttemptEvent(EntityUid Mixed, bool Cancelled = false);

public readonly record struct AfterMixingEvent(EntityUid Mixed, EntityUid Mixer);

[Serializable, NetSerializable]
public sealed partial class ReactionMixDoAfterEvent : SimpleDoAfterEvent
{
}
