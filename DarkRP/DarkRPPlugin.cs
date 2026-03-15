using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using System;

namespace DarkRP
{
    public class DarkRPPlugin : Plugin
    {
        public override string Name => "DarkRP";

        public override string Description => "DarkRP Base Plugin";

        public override string Author => "morgana";

        public override Version Version => new Version(1, 0);

        public override Version RequiredApiVersion => new Version(LabApiProperties.CompiledVersion);

        public Module Modules;
        public Entity Entities;

        public static DarkRPPlugin Singleton;

   
        public override void Enable()
        {
            Singleton = this;

            Modules = new Module();
            Modules.Load();

            Entities = new Entity();
            Entities.Load();

        }

        public override void Disable()
        {
            Modules.Unload();
            Entities.Unload();
        }


    }

}
