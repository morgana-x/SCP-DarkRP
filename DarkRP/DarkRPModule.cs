using LabApi.Features.Console;
using LabApi.Loader;
using MEC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DarkRP
{
    public abstract class DarkRPModule
    {
        public abstract void Load();
        public abstract void Unload();
        public virtual void Tick() { }
        public virtual void LoadConfigs()
        {
        }
    }

    public abstract class DarkRPModule<TConfig> : DarkRPModule where TConfig : class, new()
    {
        public TConfig? Config { get; set; }
        public override void LoadConfigs()
        {

            string filepath = DarkRPPlugin.Singleton.GetConfigPath("Modules/" + GetType().Name + ".yml");

            if (!Directory.Exists(Directory.GetParent(filepath).FullName))
                Directory.CreateDirectory(Directory.GetParent(filepath).FullName);

            Logger.Info($"Loading {GetType().Name + ".yml"}...");
            if (DarkRPPlugin.Singleton.TryLoadConfig(filepath, out TConfig? config))
                Config = config;
        }
    }
    public class Module
    {

        public Dictionary<Type, object> LoadedModules = new Dictionary<Type, object>();

        public CoroutineHandle moduleTickHandle;
        public void Load()
        {
            var classes = Assembly.GetExecutingAssembly()
                       .GetTypes()
                       .Where(t => t.IsClass && t.Namespace.StartsWith("DarkRP.Modules"))
                       .ToList();

            foreach (var type in classes.Where((x) => { return x.IsSubclassOf(typeof(DarkRPModule)); }))
                AddModule(type);

            moduleTickHandle = Timing.RunCoroutine(Tick());
        }

        public void AddModule(Type type)
        {
            var module = Activator.CreateInstance(type);
            ((DarkRPModule)module).LoadConfigs();
            ((DarkRPModule)module).Load();
            LoadedModules.Add(type, module);
            Logger.Info($"Loaded module {type.Name}");
        }

        public object GetModule(Type type)
        {
            if (LoadedModules.ContainsKey(type)) { return LoadedModules[type]; }
            return null;
        }

        public object GetModule<Type>() { return GetModule(typeof(Type)); }

        public void Unload()
        {
            Timing.KillCoroutines(moduleTickHandle);
            foreach (var pair in LoadedModules)
                ((DarkRPModule)pair.Value).Unload();

            LoadedModules.Clear();
        }

        private IEnumerator<float> Tick()
        {
            while (true)
            {
                foreach (var pair in LoadedModules)
                {
                    try
                    {
                        ((DarkRPModule)pair.Value).Tick();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                yield return Timing.WaitForSeconds(0.1f);
            }
        }
    }
}
