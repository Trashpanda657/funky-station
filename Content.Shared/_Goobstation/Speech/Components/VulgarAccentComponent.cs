// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

// CREATED BY Goldminermac
// https://github.com/space-wizards/space-station-14/pull/31149
// LICENSED UNDER THE MIT LICENSE
// SEE README.MD AND LICENSE.TXT IN THE ROOT OF THIS REPOSITORY FOR MORE INFORMATION
using Robust.Shared.GameStates;
using Content.Shared.Dataset;
using Robust.Shared.Prototypes;

namespace Content.Shared.Speech.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class VulgarAccentComponent : Component
{
    [DataField]
    public ProtoId<LocalizedDatasetPrototype> Pack = "SwearWords";

    [DataField]
    public float SwearProb = 0.5f;
}
