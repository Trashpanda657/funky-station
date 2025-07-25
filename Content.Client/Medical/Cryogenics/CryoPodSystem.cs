// SPDX-FileCopyrightText: 2022 Francesco <frafonia@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <drsmugleaf@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Shared.Emag.Systems;
using Content.Shared.Medical.Cryogenics;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using DrawDepth = Content.Shared.DrawDepth.DrawDepth;

namespace Content.Client.Medical.Cryogenics;

public sealed class CryoPodSystem: SharedCryoPodSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CryoPodComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<CryoPodComponent, GetVerbsEvent<AlternativeVerb>>(AddAlternativeVerbs);
        SubscribeLocalEvent<CryoPodComponent, GotEmaggedEvent>(OnEmagged);
        SubscribeLocalEvent<CryoPodComponent, CryoPodPryFinished>(OnCryoPodPryFinished);

        SubscribeLocalEvent<CryoPodComponent, AppearanceChangeEvent>(OnAppearanceChange);
        SubscribeLocalEvent<InsideCryoPodComponent, ComponentStartup>(OnCryoPodInsertion);
        SubscribeLocalEvent<InsideCryoPodComponent, ComponentRemove>(OnCryoPodRemoval);
    }

    private void OnCryoPodInsertion(EntityUid uid, InsideCryoPodComponent component, ComponentStartup args)
    {
        if (!TryComp<SpriteComponent>(uid, out var spriteComponent))
        {
            return;
        }

        component.PreviousOffset = spriteComponent.Offset;
        spriteComponent.Offset = new Vector2(0, 1);
    }

    private void OnCryoPodRemoval(EntityUid uid, InsideCryoPodComponent component, ComponentRemove args)
    {
        if (!TryComp<SpriteComponent>(uid, out var spriteComponent))
        {
            return;
        }

        spriteComponent.Offset = component.PreviousOffset;
    }

    private void OnAppearanceChange(EntityUid uid, CryoPodComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
        {
            return;
        }

        if (!_appearance.TryGetData<bool>(uid, CryoPodComponent.CryoPodVisuals.ContainsEntity, out var isOpen, args.Component)
            || !_appearance.TryGetData<bool>(uid, CryoPodComponent.CryoPodVisuals.IsOn, out var isOn, args.Component))
        {
            return;
        }

        if (isOpen)
        {
            args.Sprite.LayerSetState(CryoPodVisualLayers.Base, "pod-open");
            args.Sprite.LayerSetVisible(CryoPodVisualLayers.Cover, false);
            args.Sprite.DrawDepth = (int) DrawDepth.Objects;
        }
        else
        {
            args.Sprite.DrawDepth = (int) DrawDepth.Mobs;
            args.Sprite.LayerSetState(CryoPodVisualLayers.Base, isOn ? "pod-on" : "pod-off");
            args.Sprite.LayerSetState(CryoPodVisualLayers.Cover, isOn ? "cover-on" : "cover-off");
            args.Sprite.LayerSetVisible(CryoPodVisualLayers.Cover, true);
        }
    }
}

public enum CryoPodVisualLayers : byte
{
    Base,
    Cover,
}
