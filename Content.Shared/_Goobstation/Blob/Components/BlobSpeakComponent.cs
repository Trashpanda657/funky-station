// SPDX-FileCopyrightText: 2024 John Space <bigdumb421@gmail.com>
// SPDX-FileCopyrightText: 2024 fishbait <gnesse@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

//using Content.Shared.Language;
using Content.Shared.Radio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Goobstation.Blob.Components;

//[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[RegisterComponent, NetworkedComponent]
public sealed partial class BlobSpeakComponent : Component
{
  //  [DataField]
    //public ProtoId<LanguagePrototype> Language = "Blob";

    //[DataField, AutoNetworkedField]
    //public ProtoId<RadioChannelPrototype> Channel = "Hivemind";

    /// <summary>
    /// Hide entity name
    /// </summary>
    [DataField]
    public bool OverrideName = true;

    [DataField]
    public LocId Name = "speak-vv-blob";
}
