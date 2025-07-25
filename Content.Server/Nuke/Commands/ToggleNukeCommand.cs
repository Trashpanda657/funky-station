// SPDX-FileCopyrightText: 2021 Alexander Evgrashin <evgrashin.adl@gmail.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Alex Evgrashin <aevgrashin@yandex.ru>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Server.Administration;
using Content.Shared.Administration;
using JetBrains.Annotations;
using Robust.Shared.Console;

namespace Content.Server.Nuke.Commands;

[UsedImplicitly]
[AdminCommand(AdminFlags.Fun)]
public sealed class ToggleNukeCommand : LocalizedCommands
{
    [Dependency] private readonly IEntityManager _entManager = default!;

    public override string Command => "nukearm";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        EntityUid? bombUid = null;
        NukeComponent? bomb = null;

        if (args.Length >= 2)
        {
            if (!_entManager.TryParseNetEntity(args[1], out bombUid))
            {
                shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
                return;
            }
        }
        else
        {
            var query = _entManager.EntityQueryEnumerator<NukeComponent>();

            while (query.MoveNext(out var bomba, out bomb))
            {
                bombUid = bomba;
                break;
            }

            if (bombUid == null)
            {
                shell.WriteError(Loc.GetString("cmd-nukearm-not-found"));
                return;
            }
        }

        var nukeSys = _entManager.System<NukeSystem>();

        if (args.Length >= 1)
        {
            if (!float.TryParse(args[0], out var timer))
            {
                shell.WriteError("shell-argument-must-be-number");
                return;
            }

            nukeSys.SetRemainingTime(bombUid.Value, timer, bomb);
        }

        nukeSys.ToggleBomb(bombUid.Value, bomb);
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHint(Loc.GetString(Loc.GetString("cmd-nukearm-1-help")));
        }

        if (args.Length == 2)
        {
            return CompletionResult.FromHintOptions(CompletionHelper.Components<NukeComponent>(args[1]), Loc.GetString("cmd-nukearm-2-help"));
        }

        return CompletionResult.Empty;
    }
}
