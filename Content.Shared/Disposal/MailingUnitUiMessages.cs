// SPDX-FileCopyrightText: 2022 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Disposal;

[Serializable, NetSerializable]
public enum MailingUnitUiKey : byte
{
    Key
}

/// <summary>
///     Message data sent from client to server when a disposal unit ui button is pressed.
/// </summary>
[Serializable, NetSerializable]
public sealed class TargetSelectedMessage : BoundUserInterfaceMessage
{
    public readonly string? Target;

    public TargetSelectedMessage(string? target)
    {
        Target = target;
    }
}
