﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class DragoonHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Harmonized with Sky and Fire\n+120 Mana");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 12;
            item.defense = 15;
            item.value = 10000;
            item.rare = ItemRarityID.Orange;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 120;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DragoonArmor>() && legs.type == ModContent.ItemType<DragoonGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Harmonized with the four elements: fire, water, earth and air, including +2 life regen and 30% boost to all stats";
            player.lavaImmune = true;
            player.fireWalk = true;
            player.breath = 9999999;
            player.waterWalk = true;
            player.noKnockback = true;
            player.meleeDamage += 0.30f;
            player.magicDamage += 0.30f;
            player.rangedDamage += 0.30f;
            player.rangedCrit += 30;
            player.meleeCrit += 30;
            player.magicCrit += 30;
            player.meleeSpeed += 0.30f;
            player.moveSpeed += 0.30f;
            player.manaCost -= 0.30f;
            player.lifeRegen += 2;
            player.wings = 34; // looks like Jim's Wings
            player.wingsLogic = 34;
            player.wingTimeMax = 180;
            
        }
        
        public override void ArmorSetShadows (Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.GetItem("RedHerosHat"), 1);
            recipe.AddIngredient(mod.GetItem("DarkSoul"), 10000);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
