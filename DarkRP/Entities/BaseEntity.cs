using LabApi.Features.Wrappers;
using LabApi.Loader;
using Mirror;
using System.IO;
using UnityEngine;

namespace DarkRP.Entities
{
    public abstract class BaseEntity
    {
        public abstract string Name { get; }
        public GameObject CoreObject;
        public InteractableToy Interactable;
        public Pickup InteractablePickup;

        public Player Owner
        {
            get
            {
                bool result = Player.TryGet(OwnerUserId, out Player player);
                if (!result) return null;
                return player;

            }
            set { _owner = value; if (_owner != null) { OwnerUserId = _owner.UserId; } }
        }
        private Player _owner;
        public string OwnerUserId;
        public abstract void OnTick();
        public abstract void OnInteract(Player player);
        public abstract void OnDamage(Player player, int amount);
        public abstract void OnCreate(Vector3 position, Quaternion rotation);
        public abstract void OnDestroy();

        public float Health = 0;
        public virtual float MaxHealth => 100;

        public bool Paused = false;
        public BaseEntity()
        {
        }
        public void Spawn(Vector3 position)
        {
            Spawn(position, new Quaternion(0, 0, 0, 0));
        }
        public void Spawn(Vector3 position, Quaternion rotation)
        {
            Health = MaxHealth;
            OnCreate(position, rotation);
            Modules.Entities.Entity.Singleton.AddSpawnedEntity(this);
        }
        public void Destroy()
        {
            OnDestroy();
            Modules.Entities.Entity.Singleton.RemoveSpawnedEntity(this);
            if (CoreObject != null)
                NetworkServer.Destroy(CoreObject);
        }

        public virtual void LoadConfigs()
        {

        }

    }
    public abstract class BaseEntity<TConfig> : BaseEntity where TConfig : class, new()
    {
        public static TConfig? Config { get; set; }
        public override void LoadConfigs()
        {

            string filepath = DarkRPPlugin.Singleton.GetConfigPath("Entities/" + GetType().Name + ".yml");

            if (!Directory.Exists(Directory.GetParent(filepath).FullName))
                Directory.CreateDirectory(Directory.GetParent(filepath).FullName);

            LabApi.Features.Console.Logger.Info($"Loading {GetType().Name + ".yml"}...");
            if (DarkRPPlugin.Singleton.TryLoadConfig(filepath, out TConfig? config))
                Config = config;
        }
    }
}
