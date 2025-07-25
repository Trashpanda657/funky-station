// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.ContextMenu.UI;
using Content.Shared.Verbs;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Verbs.UI;

public sealed partial class ConfirmationMenuElement : ContextMenuElement
{
    public const string StyleClassConfirmationContextMenuButton = "confirmationContextMenuButton";

    public readonly Verb Verb;

    public override string Text
    {
        set
        {
            var message = new FormattedMessage();
            message.PushColor(Color.White);
            message.AddMarkupPermissive(value.Trim());
            Label.SetMessage(message);
        }
    }

    public ConfirmationMenuElement(Verb verb, string? text) : base(text)
    {
        Verb = verb;
        Icon.Visible = false;

        SetOnlyStyleClass(StyleClassConfirmationContextMenuButton);
    }
}
