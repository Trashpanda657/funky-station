// SPDX-FileCopyrightText: 2024 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Diagnostics.CodeAnalysis;
using Content.Shared.Atmos;

namespace Content.Server.Atmos;

public struct TileMixtureEnumerator
{
    public readonly TileAtmosphere?[] Tiles;
    public int Index = 0;

    public static readonly TileMixtureEnumerator Empty = new(Array.Empty<TileAtmosphere>());

    internal TileMixtureEnumerator(TileAtmosphere?[] tiles)
    {
        Tiles = tiles;
    }

    public bool MoveNext([NotNullWhen(true)] out GasMixture? mix)
    {
        while (Index < Tiles.Length)
        {
            mix = Tiles[Index++]?.Air;
            if (mix != null)
                return true;
        }

        mix = null;
        return false;
    }
}
