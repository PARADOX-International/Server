﻿using AltV.Net;
using PARADOX_RP.Core.Module;
using System;
using System.Collections.Generic;
using PARADOX_RP.Core.Extensions;
using AltV.Net.Async;
using PARADOX_RP.Core.Factories;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;

namespace PARADOX_RP.Handlers
{
    class EventHandler : IEventHandler
    {
        private readonly IEnumerable<IModuleBase> _modules;
        public EventHandler(IEnumerable<IModuleBase> modules)
        {
            _modules = modules;

            AltAsync.OnClient<PXPlayer>("Pressed_E", PressedE);
            AltAsync.OnPlayerConnect += OnPlayerConnect;
            AltAsync.OnPlayerDisconnect += OnPlayerConnect;
        }


        public void Load()
        {
            _modules.ForEach(e =>
            {
                if (e.Enabled)
                    e.OnModuleLoad();
            });
        }

        private async void PressedE(PXPlayer player)
        {
            await _modules.ForEach(async e =>
            {
                if (e.Enabled)
                    if (await e.OnKeyPress(player, Utils.Enums.KeyEnumeration.E)) return;
            });
        }

        private async Task OnPlayerConnect(IPlayer player, string reason)
        {
            PXPlayer pxPlayer = (PXPlayer)player;
            await _modules.ForEach(e =>
            {
                if (e.Enabled)
                    e.OnPlayerConnect(pxPlayer);
            });
        }
    }
}