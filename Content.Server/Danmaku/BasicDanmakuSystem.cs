using Content.Server.Coordinates.Helpers;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Doors.Components;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Physics;
using Robust.Shared.Physics.Components;

namespace Content.Server.Danmaku
{

    /// <summary>
    /// Basic danmaku system. Fires a projectile in the direction of the cursor (not yet)
    /// </summary>
    public sealed class BasicDanmakuSystem : EntitySystem
    {
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<BasicDanmakuComponent, ComponentInit>(OnComponentInit);
            SubscribeLocalEvent<BasicDanmakuComponent, DanmakuBasicActionEvent>(OnTriggerDanmaku);
        }

        private void OnComponentInit(EntityUid uid, BasicDanmakuComponent component, ComponentInit args)
        {
            _actionsSystem.AddAction(uid, component.DanmakuBasicAction, uid);
        }

        private void OnTriggerDanmaku(EntityUid uid, BasicDanmakuComponent component, DanmakuBasicActionEvent args)
        {
            var xform = Transform(uid);
            // Get the tile in front of the mime
            var offsetValue = xform.LocalRotation.ToWorldVec().Normalized;
            var coords = xform.Coordinates.Offset(offsetValue).SnapToGrid(EntityManager);
            // Check there are no walls or mobs there
            foreach (var entity in coords.GetEntitiesInTile())
            {
                PhysicsComponent? physics = null; // We use this to check if it's impassable
                if ((HasComp<MobStateComponent>(entity) && entity != uid) || // Is it a mob?
                    ((Resolve(entity, ref physics, false) && (physics.CollisionLayer & (int) CollisionGroup.Impassable) != 0) // Is it impassable?
                     &&  !(TryComp<DoorComponent>(entity, out var door) && door.State != DoorState.Closed))) // Is it a door that's open and so not actually impassable?
                {
                    _popupSystem.PopupEntity(Loc.GetString("mime-invisible-wall-failed"), uid, uid);
                    return;
                }
            }
            Spawn(component.DanmakuPrototype, coords);
            args.Handled = true;
        }
    }

    public sealed class DanmakuBasicActionEvent : WorldTargetActionEvent
    {
    }
}
