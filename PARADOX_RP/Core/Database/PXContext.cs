﻿using AltV.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using PARADOX_RP.Core.Database.Models;
using PARADOX_RP.Game.Administration.Models;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace PARADOX_RP.Core.Database
{
    internal class PXContext : DbContext
    {

        public DbSet<Players> Players { get; set; }
        public DbSet<PlayerCustomization> PlayerCustomization { get; set; }
        public DbSet<PlayerClothesWearing> PlayerClothesWearing { get; set; }
        public DbSet<PlayerTeamData> PlayerTeamData { get; set; }
        public DbSet<PlayerPhoneSettings> PlayerPhoneSettings { get; set; }
        public DbSet<SupportRankModel> SupportRanks { get; set; }
        public DbSet<PermissionModel> Permissions { get; set; }
        public DbSet<PermissionAssignmentModel> PermissionAssignments { get; set; }
        public DbSet<BankATMs> BankATMs { get; set; }
        public DbSet<Clothes> Clothes { get; set; }
        public DbSet<Vehicles> Vehicles { get; set; }
        public DbSet<Garages> Garages { get; set; }
        public DbSet<Injuries> Injuries { get; set; }
        public DbSet<Eastereggs> Eastereggs { get; set; }
        public DbSet<Teams> Teams { get; set; }

        public DbSet<Inventories> Inventories { get; set; }
        public DbSet<InventoryItemAssignments> InventoryItemAssignments { get; set; }
        public DbSet<InventoryItemSignatures> InventoryItemSignatures { get; set; }

        public DbSet<BanList> BanList { get; set; }
        public DbSet<ServerConfig> ServerConfig { get; set; }

        public static readonly ILoggerFactory loggerFactory =
           LoggerFactory.Create(
                builder =>
                {
                    builder.AddConsole();
                }
        );

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connection = $"Server=localhost;Database=altv-paradox_rp;Uid=root;Pwd=divan123;";
            optionsBuilder.UseMySql(connection, ServerVersion.AutoDetect(connection));
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }
    }
}
