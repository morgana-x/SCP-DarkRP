using LabApi.Features.Wrappers;
using DarkRP.Entities;
using DarkRP.Extensions;
using DarkRP.Modules.Players.HUD;

namespace DarkRP.Modules.Entities
{
    public class Money : DarkRPModule
    {
     
        public static BaseEntity DropMoney(UnityEngine.Vector3 Position, UnityEngine.Quaternion Rotation, long amount)
        {
            var dropped_money = Entity.Singleton.CreateEntity("spawned_money");
            ((spawned_money)dropped_money).Amount = amount;
            dropped_money.Spawn(Position, Rotation);
            return dropped_money;
        }

        public static BaseEntity DropMoney(Player player, long amount)
        {
            if (player.GetMoney() < amount)
            {
                player.Notify("Insufficient funds!", Notification.NotifyType.Error );
                return null;
            }
            player.AddMoney(-amount);
            player.Notify($"Dropped ${amount}", Notification.NotifyType.Success);

            return DropMoney(player.Camera.position + (player.Camera.forward * 0.8f), UnityEngine.Quaternion.Euler(player.Rotation.eulerAngles.x, 0, 0), amount);
        }

        public override void Load()
        {
           
        }

        public override void Tick()
        {
            
        }

        public override void Unload()
        {
            
        }
    }
}
