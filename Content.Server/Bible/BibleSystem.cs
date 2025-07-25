// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Tomás Alves <tomasalves35@gmail.com>
// SPDX-FileCopyrightText: 2022 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Dae <60460608+ZeroDayDaemon@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 double_b <40827162+benjamin-burges@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Bible.Components;
using Content.Server.Ghost.Roles.Events;
using Content.Server.Popups;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Bible;
using Content.Shared.Damage;
using Content.Shared.Ghost.Roles.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared._Goobstation.Religion;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Bible
{
    public sealed class BibleSystem : EntitySystem
    {
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly ActionBlockerSystem _blocker = default!;
        [Dependency] private readonly DamageableSystem _damageableSystem = default!;
        [Dependency] private readonly InventorySystem _invSystem = default!;
        [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;
        [Dependency] private readonly UseDelaySystem _delay = default!;
        [Dependency] private readonly SharedTransformSystem _transform = default!;
        [Dependency] private readonly FlammableSystem _flammableSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<BibleComponent, AfterInteractEvent>(OnAfterInteract);
            SubscribeLocalEvent<SummonableComponent, GetVerbsEvent<AlternativeVerb>>(AddSummonVerb);
            SubscribeLocalEvent<SummonableComponent, GetItemActionsEvent>(GetSummonAction);
            SubscribeLocalEvent<SummonableComponent, SummonActionEvent>(OnSummon);
            SubscribeLocalEvent<FamiliarComponent, MobStateChangedEvent>(OnFamiliarDeath);
            SubscribeLocalEvent<FamiliarComponent, GhostRoleSpawnerUsedEvent>(OnSpawned);
        }

        private readonly Queue<EntityUid> _addQueue = new();
        private readonly Queue<EntityUid> _remQueue = new();

        /// <summary>
        /// This handles familiar respawning.
        /// </summary>
        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            foreach (var entity in _addQueue)
            {
                EnsureComp<SummonableRespawningComponent>(entity);
            }
            _addQueue.Clear();

            foreach (var entity in _remQueue)
            {
                RemComp<SummonableRespawningComponent>(entity);
            }
            _remQueue.Clear();

            var query = EntityQueryEnumerator<SummonableRespawningComponent, SummonableComponent>();
            while (query.MoveNext(out var uid, out var _, out var summonableComp))
            {
                summonableComp.Accumulator += frameTime;
                if (summonableComp.Accumulator < summonableComp.RespawnTime)
                {
                    continue;
                }
                // Clean up the old body
                if (summonableComp.Summon != null)
                {
                    EntityManager.DeleteEntity(summonableComp.Summon.Value);
                    summonableComp.Summon = null;
                }
                summonableComp.AlreadySummoned = false;
                _popupSystem.PopupEntity(Loc.GetString("bible-summon-respawn-ready", ("book", uid)), uid, PopupType.Medium);
                _audio.PlayPvs(summonableComp.SummonSound, uid);
                // Clean up the accumulator and respawn tracking component
                summonableComp.Accumulator = 0;
                _remQueue.Enqueue(uid);
            }
        }

        private void OnAfterInteract(EntityUid uid, BibleComponent component, AfterInteractEvent args)
        {
            if (!args.CanReach)
                return;

            if (!TryComp(uid, out UseDelayComponent? useDelay) || _delay.IsDelayed((uid, useDelay)))
                return;

            if (args.Target == null || args.Target == args.User || !_mobStateSystem.IsAlive(args.Target.Value))
            {
                return;
            }

            if (!HasComp<BibleUserComponent>(args.User))
            {
                _popupSystem.PopupEntity(Loc.GetString("bible-sizzle"), args.User, args.User);

                _audio.PlayPvs(component.SizzleSoundPath, args.User);
                _damageableSystem.TryChangeDamage(args.User, component.DamageOnUntrainedUse, true, origin: uid);
                _delay.TryResetDelay((uid, useDelay));

                return;
            }

            //Goobstation Edit Begin - Religion
            //Public Domain Code Begins
            if (EntityManager.TryGetComponent(args.Target, out ReligionComponent? targetReligion))
            {
                EntityManager.TryGetComponent(args.User, out ReligionComponent? userReligion);
                switch (targetReligion.Type)
                {
                    //Atheist Chaplains set non atheists on fire and heal atheists
                    case Religion.Atheist:
                        if (userReligion != null && userReligion.Type != targetReligion.Type)
                        {
                            DoBibleSmite(uid, useDelay, args);
                            return;
                        }
                        break;

                    //Non-Atheist Chaplains set atheists on fire and heal non-atheists
                    default:
                        if (userReligion != null && userReligion.Type == Religion.Atheist)
                        {
                            DoBibleSmite(uid, useDelay, args);
                            return;
                        }
                        break;
                }
            }
            //Public Domain Code Ends
            //Goobstation Edit End - Religion

            // This only has a chance to fail if the target is not wearing anything on their head and is not a familiar.
            if (!_invSystem.TryGetSlotEntity(args.Target.Value, "head", out var _) && !HasComp<FamiliarComponent>(args.Target.Value))
            {
                if (_random.Prob(component.FailChance))
                {
                    var othersFailMessage = Loc.GetString(component.LocPrefix + "-heal-fail-others", ("user", Identity.Entity(args.User, EntityManager)), ("target", Identity.Entity(args.Target.Value, EntityManager)), ("bible", uid));
                    _popupSystem.PopupEntity(othersFailMessage, args.User, Filter.PvsExcept(args.User), true, PopupType.SmallCaution);

                    var selfFailMessage = Loc.GetString(component.LocPrefix + "-heal-fail-self", ("target", Identity.Entity(args.Target.Value, EntityManager)), ("bible", uid));
                    _popupSystem.PopupEntity(selfFailMessage, args.User, args.User, PopupType.MediumCaution);

                    _audio.PlayPvs(component.BibleHitSound, args.User);
                    _damageableSystem.TryChangeDamage(args.Target.Value, component.DamageOnFail, true, origin: uid);
                    _delay.TryResetDelay((uid, useDelay));
                    return;
                }
            }

            var damage = _damageableSystem.TryChangeDamage(args.Target.Value, component.Damage, true, origin: uid);

            if (damage == null || damage.Empty)
            {
                var othersMessage = Loc.GetString(component.LocPrefix + "-heal-success-none-others", ("user", Identity.Entity(args.User, EntityManager)), ("target", Identity.Entity(args.Target.Value, EntityManager)), ("bible", uid));
                _popupSystem.PopupEntity(othersMessage, args.User, Filter.PvsExcept(args.User), true, PopupType.Medium);

                var selfMessage = Loc.GetString(component.LocPrefix + "-heal-success-none-self", ("target", Identity.Entity(args.Target.Value, EntityManager)), ("bible", uid));
                _popupSystem.PopupEntity(selfMessage, args.User, args.User, PopupType.Large);
            }
            else
            {
                var othersMessage = Loc.GetString(component.LocPrefix + "-heal-success-others", ("user", Identity.Entity(args.User, EntityManager)), ("target", Identity.Entity(args.Target.Value, EntityManager)), ("bible", uid));
                _popupSystem.PopupEntity(othersMessage, args.User, Filter.PvsExcept(args.User), true, PopupType.Medium);

                var selfMessage = Loc.GetString(component.LocPrefix + "-heal-success-self", ("target", Identity.Entity(args.Target.Value, EntityManager)), ("bible", uid));
                _popupSystem.PopupEntity(selfMessage, args.User, args.User, PopupType.Large);
                _audio.PlayPvs(component.HealSoundPath, args.User);
                _delay.TryResetDelay((uid, useDelay));
            }
        }

        //Goobstation Edit Begin - Religion
        //Public Domain Code begin
        private void DoBibleSmite(EntityUid uid, UseDelayComponent useDelay, AfterInteractEvent args)
        {
            _popupSystem.PopupEntity(Loc.GetString("bible-religion-opposing"),
                args.User,
                args.User,
                PopupType.MediumCaution);
            if (EntityManager.TryGetComponent(args.Target, out FlammableComponent? flammable))
            {
                _flammableSystem.SetFireStacks(args.Target.Value, 1);
                _flammableSystem.Ignite(args.Target.Value, args.User);
                _delay.TryResetDelay((uid, useDelay));
                return;
            }
            return;
        }
        //Public Domain Code end
        //Goobstation Edit End - Religion

        private void AddSummonVerb(EntityUid uid, SummonableComponent component, GetVerbsEvent<AlternativeVerb> args)
        {
            if (!args.CanInteract || !args.CanAccess || component.AlreadySummoned || component.SpecialItemPrototype == null)
                return;

            if (component.RequiresBibleUser && !HasComp<BibleUserComponent>(args.User))
                return;

            AlternativeVerb verb = new()
            {
                Act = () =>
                {
                    if (!TryComp(args.User, out TransformComponent? userXform))
                        return;

                    AttemptSummon((uid, component), args.User, userXform);
                },
                Text = Loc.GetString("bible-summon-verb"),
                Priority = 2
            };
            args.Verbs.Add(verb);
        }

        private void GetSummonAction(EntityUid uid, SummonableComponent component, GetItemActionsEvent args)
        {
            if (component.AlreadySummoned)
                return;

            args.AddAction(ref component.SummonActionEntity, component.SummonAction);
        }

        private void OnSummon(Entity<SummonableComponent> ent, ref SummonActionEvent args)
        {
            AttemptSummon(ent, args.Performer, Transform(args.Performer));
        }

        /// <summary>
        /// Starts up the respawn stuff when
        /// the chaplain's familiar dies.
        /// </summary>
        private void OnFamiliarDeath(EntityUid uid, FamiliarComponent component, MobStateChangedEvent args)
        {
            if (args.NewMobState != MobState.Dead || component.Source == null)
                return;

            var source = component.Source;
            if (source != null && HasComp<SummonableComponent>(source))
            {
                _addQueue.Enqueue(source.Value);
            }
        }

        /// <summary>
        /// When the familiar spawns, set its source to the bible.
        /// </summary>
        private void OnSpawned(EntityUid uid, FamiliarComponent component, GhostRoleSpawnerUsedEvent args)
        {
            var parent = Transform(args.Spawner).ParentUid;
            if (!TryComp<SummonableComponent>(parent, out var summonable))
                return;

            component.Source = parent;
            summonable.Summon = uid;
        }

        private void AttemptSummon(Entity<SummonableComponent> ent, EntityUid user, TransformComponent? position)
        {
            var (uid, component) = ent;
            if (component.AlreadySummoned || component.SpecialItemPrototype == null)
                return;
            if (component.RequiresBibleUser && !HasComp<BibleUserComponent>(user))
                return;
            if (!Resolve(user, ref position))
                return;
            if (component.Deleted || Deleted(uid))
                return;
            if (!_blocker.CanInteract(user, uid))
                return;

            // Make this familiar the component's summon
            var familiar = EntityManager.SpawnEntity(component.SpecialItemPrototype, position.Coordinates);
            component.Summon = familiar;

            // If this is going to use a ghost role mob spawner, attach it to the bible.
            if (HasComp<GhostRoleMobSpawnerComponent>(familiar))
            {
                _popupSystem.PopupEntity(Loc.GetString("bible-summon-requested"), user, user, PopupType.Medium);
                _transform.SetParent(familiar, uid);
            }
            component.AlreadySummoned = true;
            _actionsSystem.RemoveAction(user, component.SummonActionEntity);
        }
    }
}
