using System.Collections.Generic;

namespace Game.Development
{
    public interface IGUIDropdownProvider
    {
        bool autoUpdate { get; }
        
        void SetFilter(string filter);

        string[] GetNames();
        object[] GetValues();
    }
}