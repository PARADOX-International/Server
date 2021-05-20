﻿using AltV.Net.Async;
using AltV.Net.Data;
using EntityStreamer;
using Newtonsoft.Json;
using PARADOX_RP.Controllers.Event.Interface;
using PARADOX_RP.Core.Database;
using PARADOX_RP.Core.Database.Models;
using PARADOX_RP.Core.Extensions;
using PARADOX_RP.Core.Factories;
using PARADOX_RP.Core.Module;
using PARADOX_RP.UI;
using PARADOX_RP.UI.Windows;
using PARADOX_RP.Utils;
using PARADOX_RP.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PARADOX_RP.Game.Bank
{
    class BankModule : ModuleBase<BankModule>
    {
        private Dictionary<int, BankATMs> _BankATMs = new Dictionary<int, BankATMs>();

        public BankModule(PXContext px, IEventController eventController) : base("Bank")
        {
            LoadDatabaseTable(px.BankATMs, (BankATMs atm) =>
            {
                _BankATMs.Add(atm.Id, atm);
            });



            eventController.OnClient<PXPlayer, int>("DepositMoney", DepositMoney);
            eventController.OnClient<PXPlayer, int>("WithdrawMoney", WithdrawMoney);
            eventController.OnClient<PXPlayer, string, int>("TransferMoney", TransferMoney);
        }

        public override void OnPlayerConnect(PXPlayer player)
        {
            if (Configuration.Instance.DevMode)
            {
                _BankATMs.ForEach((atm) =>
                {
                    player.AddBlips($"Bankautomat #{atm.Key}", atm.Value.Position, 108, 25, 1, true);

                    if (Configuration.Instance.DevMode)
                        MarkerStreamer.Create(MarkerTypes.MarkerTypeDallorSign, Vector3.Add(atm.Value.Position, new Vector3(0, 0, 1)), new Vector3(1, 1, 1), null, null, new Rgba(37, 165, 202, 200));
                });
            }
        }

        private readonly string _bankName = "N26 Bank";

        public override Task<bool> OnKeyPress(PXPlayer player, KeyEnumeration key)
        {
            if (key == KeyEnumeration.E)
            {
                BankATMs targetATM = _BankATMs.Values.FirstOrDefault(a => a.Position.Distance(player.Position) < 3);
                if (targetATM == null) return Task.FromResult(false);

                WindowManager.Instance.Get<BankWindow>().Show(player, new BankWindowWriter(player.Username, player.Money, player.BankMoney));
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public async void DepositMoney(PXPlayer player, int moneyAmount)
        {
            if (!player.IsValid()) return;
            if (!player.CanInteract()) return;
            if (!WindowManager.Instance.Get<BankWindow>().IsVisible(player)) return;

            if (!await player.TakeMoney(moneyAmount))
            {
                player.SendNotification(_bankName, "Du hast nicht genügend Geld dabei.", NotificationTypes.ERROR);
                return;
            }

            await using (var px = new PXContext())
            {
                player.BankMoney += moneyAmount;
                (await px.Players.FindAsync(player.SqlId)).BankMoney = player.BankMoney;
                await px.SaveChangesAsync();
            }
        }

        public async void WithdrawMoney(PXPlayer player, int moneyAmount)
        {
            if (!player.IsValid()) return;
            if (!player.CanInteract()) return;
            if (!WindowManager.Instance.Get<BankWindow>().IsVisible(player)) return;

            if (player.BankMoney < moneyAmount)
            {
                player.SendNotification(_bankName, "Du hast nicht genügend Geld auf dem Konto!", NotificationTypes.ERROR);
                return;
            }

            await using (var px = new PXContext())
            {

                player.BankMoney -= moneyAmount;
                (await px.Players.FindAsync(player.SqlId)).BankMoney = player.BankMoney;
                await px.SaveChangesAsync();
            }

            await player.AddMoney(moneyAmount);
        }
        private async void TransferMoney(PXPlayer player, string targetString, int moneyAmount)
        {
            if (!player.IsValid()) return;
            if (!player.CanInteract()) return;
            if (!WindowManager.Instance.Get<BankWindow>().IsVisible(player)) return;

            if (player.Username.ToLower() == targetString.ToLower())
            {
                player.SendNotification(_bankName, "Du kannst dir nicht selber Geld senden!", NotificationTypes.ERROR);
                //TODO: add log
                return;
            }

            if (player.BankMoney < moneyAmount)
            {
                player.SendNotification(_bankName, "Du hast nicht genügend Geld auf dem Konto!", NotificationTypes.ERROR);
                return;
            }

            PXPlayer target = Pools.Instance.Get<PXPlayer>(PoolType.PLAYER).FirstOrDefault(p => p.Username.ToLower() == targetString.ToLower());
            if (target == null || !target.IsValid())
            {
                player.SendNotification(_bankName, $"Es wurde kein Konto mit dem Besitzer {targetString} gefunden!", NotificationTypes.ERROR);
                return;
            }

            await using (var px = new PXContext())
            {
                player.BankMoney -= moneyAmount;
                target.BankMoney += moneyAmount;

                Players dbPlayer = await px.Players.FindAsync(player.SqlId);
                Players dbTargetPlayer = await px.Players.FindAsync(target.SqlId);

                dbPlayer.BankMoney = player.BankMoney;
                dbTargetPlayer.BankMoney = target.BankMoney;

                await px.SaveChangesAsync();
            }
        }
    }
}
