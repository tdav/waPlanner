using System;
using System.Collections.Generic;

namespace waPlanner.ModelViews
{
    public class LangsModel : Dictionary<string, Dictionary<string, string>>
    {
        public string Get(string key, string lang)
        {
            return this[key][lang];
        }
    }
}
