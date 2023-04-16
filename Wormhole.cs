using Terraria;
using Terraria.ID;
using Terraria.GameInput;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;
using Terraria.Localization;

namespace WormholeHotkey
{
    public class Wormhole : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!KeybindSystem.TeleportKeybind.JustPressed)
                return;

            if (Player.DeadOrGhost)
            {
                Main.NewText($"You are dead");
                return;
            }

            if (Main.cancelWormHole)
            {
                Main.NewText($"Wormhole potions are disabled");
                return;
            }

            if (Player.team < 1)
            {
                Main.NewText($"Be a team player!");
                return;
            }

            if (!HasUnityPotion())
            {
                Main.NewText($"Find some potions");
                return;
            }

            var nextTeammate = GetTeammates().FirstOrDefault();

            if (nextTeammate is null)
            {
                Main.NewText($"Sorry you have no friends :(");
                return;
            }

            TakeUnityPotion();

            ReleaseHooks();

            UnityTeleport(nextTeammate);
        }

        private void ReleaseHooks()
        {
            Player.grappling[0] = -1;
            Player.grapCount = 0;

            for (int p = 0; p < 1000; p++)
            {
                if (Main.projectile[p].active && Main.projectile[p].owner == Player.whoAmI && Main.projectile[p].aiStyle == ProjAIStyleID.Hook)
                {
                    Main.projectile[p].Kill();
                }
            }
        }

        public bool HasUnityPotion()
        {
            for (int i = 0; i < 58; i++)
            {
                if (Player.inventory[i].type == ItemID.WormholePotion && Player.inventory[i].stack > 0)
                {
                    return true;
                }
            }
            if (Player.IsVoidVaultEnabled)
            {
                for (int j = 0; j < 40; j++)
                {
                    if (Player.bank4.item[j].type == ItemID.WormholePotion && Player.bank4.item[j].stack > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private IEnumerable<Player> GetTeammates()
        {
            return Main.player?
                .ToArray()
                .Where(x => 
                    x.team == Player.team && 
                    x.whoAmI != Player.whoAmI &&
                    !x.dead)
                ?? Enumerable.Empty<Player>();
        }

        public void TakeUnityPotion()
        {
            for (int i = 0; i < 58; i++)
            {
                if (Player.inventory[i].type == ItemID.WormholePotion && Player.inventory[i].stack > 0)
                {
                    Player.inventory[i].stack--;
                    if (Player.inventory[i].stack <= 0)
                    {
                        Player.inventory[i].SetDefaults(0);
                    }
                    return;
                }
            }
            if (Player.IsVoidVaultEnabled)
            {
                for (int j = 0; j < 40; j++)
                {
                    if (Player.bank4.item[j].type == ItemID.WormholePotion && Player.bank4.item[j].stack > 0)
                    {
                        Player.bank4.item[j].stack--;
                        if (Player.bank4.item[j].stack <= 0)
                        {
                            Player.bank4.item[j].SetDefaults(0);
                        }
                        return;
                    }
                }
            }
        }

        public void UnityTeleport(Player player)
        {
            var telePos = player.position;
            var style = 3;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Player.Teleport(telePos, style, 0);
                return;
            }

            NetMessage.SendData(MessageID.Teleport, -1, -1, NetworkText.Empty, 2, Player.whoAmI, telePos.X, telePos.Y, style, 0, 0);
        }
    }
}