// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 chairbender <kwhipke1@gmail.com>
// SPDX-FileCopyrightText: 2021 E F R <602406+Efruit@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Paul <ritter.paul1+git@googlemail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <zddm@outlook.es>
// SPDX-FileCopyrightText: 2022 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Morb <14136326+Morb0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Alert;

/// <summary>
/// An alert popup with associated icon, tooltip, and other data.
/// </summary>
[Prototype]
public sealed partial class AlertPrototype : IPrototype
{
    /// <summary>
    /// Type of alert, no 2 alert prototypes should have the same one.
    /// </summary>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// List of icons to use for this alert. Each entry corresponds to a different severity level, starting from the
    /// minimum and incrementing upwards. If severities are not supported, the first entry is used.
    /// </summary>
    [DataField(required: true)]
    public List<SpriteSpecifier> Icons = new();

    /// <summary>
    /// An entity used for displaying the <see cref="Icons"/> in the UI control.
    /// </summary>
    [DataField]
    public EntProtoId AlertViewEntity = "AlertSpriteView";

    /// <summary>
    /// Name to show in tooltip window. Accepts formatting.
    /// </summary>
    [DataField]
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Description to show in tooltip window. Accepts formatting.
    /// </summary>
    [DataField]
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Category the alert belongs to. Only one alert of a given category
    /// can be shown at a time. If one is shown while another is already being shown,
    /// it will be replaced. This can be useful for categories of alerts which should naturally
    /// replace each other and are mutually exclusive, for example lowpressure / highpressure,
    /// hot / cold. If left unspecified, the alert will not replace or be replaced by any other alerts.
    /// </summary>
    [DataField]
    public ProtoId<AlertCategoryPrototype>? Category { get; private set; }

    /// <summary>
    /// Key which is unique w.r.t category semantics (alerts with same category have equal keys,
    /// alerts with no category have different keys).
    /// </summary>
    public AlertKey AlertKey => new(ID, Category);

    /// <summary>
    /// -1 (no effect) unless MaxSeverity is specified. Defaults to 1. Minimum severity level supported by this state.
    /// </summary>
    public short MinSeverity => MaxSeverity == -1 ? (short) -1 : _minSeverity;

    [DataField("minSeverity")] private short _minSeverity = 1;

    /// <summary>
    /// Maximum severity level supported by this state. -1 (default) indicates
    /// no severity levels are supported by the state.
    /// </summary>
    [DataField]
    public short MaxSeverity = -1;

    /// <summary>
    /// Indicates whether this state support severity levels
    /// </summary>
    public bool SupportsSeverity => MaxSeverity != -1;

    /// <summary>
    /// Event raised on the user when they click on this alert.
    /// Can be null.
    /// </summary>
    [DataField]
    public BaseAlertEvent? ClickEvent;

    /// <param name="severity">severity level, if supported by this alert</param>
    /// <returns>the icon path to the texture for the provided severity level</returns>
    public SpriteSpecifier GetIcon(short? severity = null)
    {
        var minIcons = SupportsSeverity
            ? MaxSeverity - MinSeverity
            : 1;

        if (Icons.Count < minIcons)
            throw new InvalidOperationException($"Insufficient number of icons given for alert {ID}");

        if (!SupportsSeverity)
            return Icons[0];

        if (severity == null)
        {
            throw new ArgumentException($"No severity specified but this alert ({AlertKey}) has severity.", nameof(severity));
        }

        if (severity < MinSeverity)
        {
            throw new ArgumentOutOfRangeException(nameof(severity), $"Severity below minimum severity in {AlertKey}.");
        }

        if (severity > MaxSeverity)
        {
            throw new ArgumentOutOfRangeException(nameof(severity), $"Severity above maximum severity in {AlertKey}.");
        }

        return Icons[severity.Value - _minSeverity];
    }
}

[ImplicitDataDefinitionForInheritors]
public abstract partial class BaseAlertEvent : HandledEntityEventArgs
{
    public EntityUid User;

    public ProtoId<AlertPrototype> AlertId;

    protected BaseAlertEvent(EntityUid user, ProtoId<AlertPrototype> alertId)
    {
        User = user;
        AlertId = alertId;
    }
}
