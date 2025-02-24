﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class AnkorWatHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("30% Less Mana Usage.\n+15 Defense when health is less than 120");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 12;
            item.defense = 14;
            item.value = 100000;
            item.rare = ItemRarityID.Pink;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost -= 0.30f;

            if (player.statLife <= 120)
            {
                player.statDefense += 15;
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AnkorWatChestplate>() && legs.type == ModContent.ItemType<AnkorWatLeggings>();
        }

       public override void UpdateArmorSet(Player player)
        {
            player.manaRegenBuff = true;
            player.statManaMax2 += 160;
            player.manaRegen += 8;
            int dust = Dust.NewDust(new Vector2((float)player.position.X, (float)player.position.Y), player.width, player.height, 60, (player.velocity.X) + (player.direction * 1), player.velocity.Y, 100, Color.Red, 1.0f);
            Main.dust[dust].noGravity = true;
        }

        public override void ArmorSetShadows (Player player){

            player.armorEffectDrawShadow = true;
            
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HallowedHeadgear, 1);
            recipe.AddIngredient(mod.GetItem("DarkSoul"), 10000);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
