﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace oneshotpropick
{
    public class OneshotPropickMod : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterItemClass("ItemOneshotProspectingPick", typeof(ItemOneshotProspectingPick));
        }
    }
}
