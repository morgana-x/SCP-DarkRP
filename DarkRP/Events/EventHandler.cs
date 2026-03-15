using System;

namespace DarkRP.Events
{
    [Serializable]
    public delegate void EventHandler<TEventArgs>(TEventArgs e);
}
