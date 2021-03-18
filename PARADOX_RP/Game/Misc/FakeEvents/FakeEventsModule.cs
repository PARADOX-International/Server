﻿using AltV.Net.Async;
using PARADOX_RP.Core.Factories;
using PARADOX_RP.Core.Module;
using System;
using System.Collections.Generic;
using System.Text;

namespace PARADOX_RP.Game.Misc.FakeEvents
{
    class FakeEventsModule : ModuleBase<FakeEventsModule>
    {
        public FakeEventsModule() : base("FakeEvents") {
            AltAsync.OnClient<PXPlayer, int>("SetMoney", SetMoney);
        }

        private void SetMoney(PXPlayer player, int moneyAmount)
        {
            //TODO: player handler ban func hitler
        }
    }
}
