// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 themias <89101928+themias@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Temperature.Systems;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Temperature.Components;

/// <summary>
/// Adds thermal energy to entities with <see cref="TemperatureComponent"/> placed on it.
/// </summary>
[RegisterComponent, Access(typeof(SharedEntityHeaterSystem))]
[NetworkedComponent, AutoGenerateComponentState]
public sealed partial class EntityHeaterComponent : Component
{
    /// <summary>
    /// Power used when heating at the high setting.
    /// Low and medium are 33% and 66% respectively.
    /// </summary>
    [DataField]
    public float Power = 2400f;

    /// <summary>
    /// Current setting of the heater. If it is off or unpowered it won't heat anything.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityHeaterSetting Setting = EntityHeaterSetting.Off;

    /// <summary>
    /// An optional sound that plays when the setting is changed.
    /// </summary>
    [DataField]
    public SoundPathSpecifier? SettingSound;
}
