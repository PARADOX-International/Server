﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PARADOX_RP.Core.Database.Models
{
    class Players
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string HardwareId { get; set; }
        public string DiscordId { get; set; }
        public string SocialClubName { get; set; }
        public string SocialClubHash { get; set; }

        public DateTime LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}