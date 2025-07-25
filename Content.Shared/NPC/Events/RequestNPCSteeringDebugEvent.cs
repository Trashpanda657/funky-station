// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.NPC.Events;

/// <summary>
/// Raised from client to server to request NPC steering debug info.
/// </summary>
[Serializable, NetSerializable]
public sealed class RequestNPCSteeringDebugEvent : EntityEventArgs
{
    public bool Enabled;
}
