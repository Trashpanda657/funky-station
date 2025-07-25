// SPDX-FileCopyrightText: 2024 brainfood1183 <113240905+brainfood1183@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT


using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Plunger;

[Serializable, NetSerializable]
public sealed partial class PlungerDoAfterEvent : SimpleDoAfterEvent
{
}
