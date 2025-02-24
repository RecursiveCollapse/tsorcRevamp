﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class CrystalSightedHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Dazzling armor cut from crystal\nWhen health falls below 80, ranged damage increases 50% (20% above normal)\nSighted helmet also offers +5% melee critical chance.");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 20;
            item.defense = 5;
            item.value = 7000000;
            item.rare = ItemRarityID.Pink;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<CrystalArmor>() && legs.type == ModContent.ItemType<CrystalGreaves>();
        }

        public override void UpdateEquip(Player player)
        {
            if (player.statLife <= 80)
            {
                player.rangedDamage -= 0.3f;
                player.magicDamage += 0.2f;
                player.meleeCrit += 5;

                int dust = Dust.NewDust(new Vector2((float)player.position.X, (float)player.position.Y), player.width, player.height, 42, (player.velocity.X) + (player.direction * 1), player.velocity.Y, 105, Color.Aqua, 1.0f);
                Main.dust[dust].noGravity = true;
            }
            else
            {
                player.rangedDamage -= 0.3f;
                player.magicDamage -= 0.3f;
                player.meleeCrit += 5;
            }
        }

        public override void UpdateArmorSet(Player player)
        {
            player.meleeSpeed += 0.4f;
            player.meleeDamage += 0.15f;
            player.enemySpawns = true;
        }
        
        public override void ArmorSetShadows (Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CobaltHelmet, 1);
            recipe.AddIngredient(ItemID.CrystalShard, 30);
            recipe.AddIngredient(mod.GetItem("DarkSoul"), 3000);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
