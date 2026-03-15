using LabApi.Features.Wrappers;
using DarkRP.Extensions;
using DarkRP.Modules.Entities;
using DarkRP.Modules.Items;
using DarkRP.Modules.Players.Jobs;
using System;
using Utils;

namespace DarkRP.Items
{
    public class BatteringRam : CustomItemBase
    {
        public BatteringRam() : base()
        {
        }

        public override ItemType BaseItem => ItemType.ParticleDisruptor;

        DateTime nextFire = DateTime.Now;


        public override void OnGive(Player player)
        {
           ((ParticleDisruptorItem)Item).StoredAmmo = ((ParticleDisruptorItem)Item).MaxAmmo;
      

        }

        public override void OnShoot(Player player)
        {
            if (DateTime.Now < nextFire) return;

            ((ParticleDisruptorItem)Item).StoredAmmo = ((ParticleDisruptorItem)Item).MaxAmmo;
            nextFire = DateTime.Now.AddSeconds(5);
            var lookingDoor = player.GetLookingDoor();
            if (lookingDoor == null)
                return;

            if (lookingDoor.IsOpened)
                return;

            RPDoor door = Modules.Entities.Door.GetRPDoor(lookingDoor);
            if (door == null) return;

            if (door.Teams.Contains("world"))
                return;

            if (door.Owner != null && !Government.IsWarranted(door.Owner))
            {
                player.NotifyTop("Player isn't warranted! .warrant them!!!", Modules.Players.HUD.Notification.NotifyType.Error);
                return;
            }

            ExplosionUtils.ServerSpawnEffect(lookingDoor.Position + new UnityEngine.Vector3(0, 1, 0), ItemType.GrenadeHE);
            lookingDoor.IsOpened = true;
          //  lookingDoor.Lock(Interactables.Interobjects.DoorUtils.DoorLockReason.AdminCommand, true);
        }
    }
}
