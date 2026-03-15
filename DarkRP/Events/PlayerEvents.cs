using System;
using DarkRP.Events.Arguments.Player;
namespace DarkRP.Events.Arguments.Player
{
    public class JobChangingEventArgs : EventArgs
    {
        public LabApi.Features.Wrappers.Player Player { get; set; }
        public string OldJob { get; set; }
        public string NewJob { get; set; }
        public bool IsAllowed { get; set; }

        public JobChangingEventArgs(LabApi.Features.Wrappers.Player player, string oldJob, string newJob, bool isAllowed)
        {
            Player = player;
            OldJob = oldJob;
            NewJob = newJob;
            IsAllowed = isAllowed;
        }
    }
    public class JobChangedEventArgs : EventArgs
    {
        public LabApi.Features.Wrappers.Player Player { get; set; }
        public string OldJob { get; set; }
        public string NewJob { get; set; }
        public JobChangedEventArgs(LabApi.Features.Wrappers.Player player, string oldJob, string newJob)
        {
            Player = player;
            OldJob = oldJob;
            NewJob = newJob;
        }
    }

    public class HUDGeneratingEventArgs : EventArgs
    {
        public LabApi.Features.Wrappers.Player Player { get; set; }
        public bool IsAllowed { get; set; }

        public HUDGeneratingEventArgs(LabApi.Features.Wrappers.Player player, bool isAllowed)
        {
            Player = player;
            IsAllowed = isAllowed;
        }
    }
}


namespace DarkRP.Events.Handlers
{
    public class PlayerEvents
    {
       

        public static EventHandler<JobChangingEventArgs> JobChanging;

        internal static void JobChangingFire(JobChangingEventArgs e)
        {
            JobChanging?.Invoke(e);
        }

     
        public static EventHandler<JobChangedEventArgs> JobChanged;

        internal static void JobChangedFire(JobChangedEventArgs e)
        {
            JobChanged?.Invoke(e);
        }

        public static EventHandler<HUDGeneratingEventArgs> HUDGenerating;

        internal static void HUDGeneratingFire(HUDGeneratingEventArgs e)
        {
            HUDGenerating?.Invoke(e);
        }
    }
}
