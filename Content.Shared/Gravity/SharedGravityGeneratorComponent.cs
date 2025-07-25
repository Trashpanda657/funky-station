// SPDX-FileCopyrightText: 2020 Exp <theexp111@gmail.com>
// SPDX-FileCopyrightText: 2020 Jackson Lewis <inquisitivepenguin@protonmail.com>
// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Power;
using Robust.Shared.GameStates;

namespace Content.Shared.Gravity;

[NetworkedComponent()]
[Virtual]
public partial class SharedGravityGeneratorComponent : Component
{
    /// <summary>
    /// A map of the sprites used by the gravity generator given its status.
    /// </summary>
    [DataField("spriteMap")]
    [Access(typeof(SharedGravitySystem))]
    public Dictionary<PowerChargeStatus, string> SpriteMap = new();

    /// <summary>
    /// The sprite used by the core of the gravity generator when the gravity generator is starting up.
    /// </summary>
    [DataField("coreStartupState")]
    [ViewVariables(VVAccess.ReadWrite)]
    public string CoreStartupState = "startup";

    /// <summary>
    /// The sprite used by the core of the gravity generator when the gravity generator is idle.
    /// </summary>
    [DataField("coreIdleState")]
    [ViewVariables(VVAccess.ReadWrite)]
    public string CoreIdleState = "idle";

    /// <summary>
    /// The sprite used by the core of the gravity generator when the gravity generator is activating.
    /// </summary>
    [DataField("coreActivatingState")]
    [ViewVariables(VVAccess.ReadWrite)]
    public string CoreActivatingState = "activating";

    /// <summary>
    /// The sprite used by the core of the gravity generator when the gravity generator is active.
    /// </summary>
    [DataField("coreActivatedState")]
    [ViewVariables(VVAccess.ReadWrite)]
    public string CoreActivatedState = "activated";
}
