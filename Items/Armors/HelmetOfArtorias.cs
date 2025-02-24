﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class HelmetOfArtorias : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Helmet of Artorias");
            Tooltip.SetDefault("Enchanted helmet of Artorias.\nLonger invincibility when hit plus 30% critical chance.");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.defense = 20;
            item.value = 35500;
            item.rare = ItemRarityID.Purple;
        }

        public override void UpdateEquip(Player player)
        {
            player.longInvince = true;
            player.meleeCrit += 30;
            player.rangedCrit += 30;
            player.magicCrit += 30;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ArmorOfArtorias>() && legs.type == ModContent.ItemType<GreavesOfArtorias>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.lavaImmune = true;
            player.fireWalk = true;
            player.breath = 9999999;
            player.waterWalk = true;
            player.noKnockback = true;
            player.meleeDamage += 0.37f;
            player.magicDamage += 0.37f;
            player.rangedDamage += 0.37f;
            player.meleeSpeed += 0.37f;
            player.moveSpeed += 0.50f;
            player.manaCost -= 0.37f;
            player.lifeRegen += 8;
            player.nightVision = true;
            player.noFallDmg = true;

            int dust = Dust.NewDust(new Vector2((float)player.position.X, (float)player.position.Y), player.width, player.height, 32, player.velocity.X - 3f, player.velocity.Y, 150, Color.Yellow, 1f);
            Main.dust[dust].noGravity = true;

            int i2 = (int)(player.position.X + (float)(player.width / 2) + (float)(8 * player.direction)) / 16;
            int j2 = (int)(player.position.Y + 2f) / 16;
            Lighting.AddLight(i2, j2, 0.92f, 0.8f, 0.65f);
        }
        
        public override void ArmorSetShadows (Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.GetItem("SoulOfArtorias"), 2);
            recipe.AddIngredient(mod.GetItem("GuardianSoul"), 1);
            recipe.AddIngredient(mod.GetItem("DarkSoul"), 60000);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
