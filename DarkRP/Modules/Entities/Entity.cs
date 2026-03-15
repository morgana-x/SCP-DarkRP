using DarkRP.Entities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DarkRP.Modules.Entities
{
    public class Entity : DarkRPModule
    {

        public Dictionary<string, Type> EntityMap = new Dictionary<string, Type>();
        private volatile List<BaseEntity> entities = new List<BaseEntity>();

        public List<BaseEntity> Entities { get { return entities.ToList(); } }


        public static Entity Singleton;
        public override void Load()
        {
            Singleton = this;
            var classes = Assembly.GetExecutingAssembly()
                       .GetTypes()
                       .Where(t => t.IsClass && t.Namespace.StartsWith("DarkRP.Entities"))
                       .ToList();

            foreach (var type in classes.Where((x) => { return x.IsSubclassOf(typeof(BaseEntity)) && !x.ContainsGenericParameters; }))
                RegisterEntity(type);


            PlayerEvents.PickingUpItem += OnPicking;
            PlayerEvents.SearchingPickup += OnSearching;
            PlayerEvents.InteractedToy += OnPickingToy;
            PlayerEvents.PlacedBulletHole += ShotWeapon;
            Scp914Events.ProcessingPickup += ProcessingPickup;
        }
        public override void Unload()
        {
            PlayerEvents.PickingUpItem -= OnPicking;
            PlayerEvents.SearchingPickup -= OnSearching;
            PlayerEvents.InteractedToy -= OnPickingToy;
            PlayerEvents.PlacedBulletHole -= ShotWeapon;
            Scp914Events.ProcessingPickup -= ProcessingPickup;


            entities.Clear();
            EntityMap.Clear();
        }
        public override void Tick()
        {
            foreach (var ent in Entities)
            {
                if (ent.Paused)
                    continue;
                try
                {
                    ent.OnTick();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public static BaseEntity GetEntity(GameObject obj)
        {
            if (obj == null) return null;
            var result = Singleton.Entities.Where((x) => { return x.CoreObject != null && x.CoreObject == obj || x.Interactable != null && x.Interactable.GameObject == obj || x.InteractablePickup != null && x.InteractablePickup.Base != null && x.InteractablePickup.Base.gameObject == obj; }).ToList();
            return result.Count > 0 ? result.First() : null;
        }
        private void ShotWeapon(PlayerPlacedBulletHoleEventArgs e)
        {
            var result = Physics.Raycast(e.RaycastStart, (e.HitPosition - e.RaycastStart).normalized, out RaycastHit info, 9999f);
            if (!result) return;
            if (info.collider == null || info.collider.gameObject == null) return;
            var ent = GetEntity(info.collider.gameObject);
            if (ent == null) return;
            ent.OnDamage(e.Player, 20);
        }
        private void ProcessingPickup(Scp914ProcessingPickupEventArgs e)
        {
            foreach (var ent in Entities)
            {
                if (ent.InteractablePickup == null) continue;
                if (ent.InteractablePickup == e.Pickup)
                {
                    e.IsAllowed = false;
                    break;
                }
            }
        }

        private void OnSearching(PlayerSearchingPickupEventArgs e)
        {
            foreach (var ent in Entities)
            {
                if (e.Pickup != ent.InteractablePickup) continue;
                e.IsAllowed = false;
                e.Pickup.IsInUse = false;
                try
                {
                    ent.OnInteract(e.Player);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return;
            }
        }
        private void OnPicking(PlayerPickingUpItemEventArgs e)
        {
            foreach (var ent in Entities)
            {
                if (e.Pickup != ent.InteractablePickup) continue;
                e.IsAllowed = false;
                ent.InteractablePickup.IsInUse = false;
                break;
            }
        }
        private void OnPickingToy(PlayerInteractedToyEventArgs e)
        {
            foreach (var ent in Entities)
            {
                if (e.Interactable != ent.Interactable) continue;
                try
                {
                    ent.OnInteract(e.Player);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return;
            }
        }
        public void RegisterEntity(Type type)
        {
            var inst = (BaseEntity)Activator.CreateInstance(type);
            inst.LoadConfigs();
            inst = null;

            EntityMap.Add(type.Name, type);
            LabApi.Features.Console.Logger.Info($"Registered entity {type.Name}!");
        }

        internal void AddSpawnedEntity(BaseEntity entity)
        {
            if (!Entities.Contains(entity))
                entities.Add(entity);
        }
        internal void RemoveSpawnedEntity(BaseEntity entity)
        {
            if (Entities.Contains(entity))
                entities.Remove(entity);
        }
        public BaseEntity CreateEntity(string id)
        {
            if (!EntityMap.ContainsKey(id)) { return null; }
            return (BaseEntity)Activator.CreateInstance(EntityMap[id]);
        }
        public BaseEntity SpawnEntity(string id, Vector3 pos, Player owner = null)
        {
            return SpawnEntity(id, pos, new Quaternion(0, 0, 0, 0), owner);
        }
        public BaseEntity SpawnEntity(string id, Vector3 pos, Quaternion rotation, Player? owner = null)
        {
            var ent = CreateEntity(id);
            if (ent == null) return null;
            ent.Owner = owner;
            ent.Spawn(pos, rotation);
            return ent;
        }

        public List<BaseEntity> GetEntities(Player p, string id = "")
        {
            return Entities.Where((x) =>
            {
                return (x.Owner != null && x.Owner == p || x.OwnerUserId == p.UserId) && (id == "" || x.GetType().Name == id);
            }
            ).ToList();
        }
    }
}
