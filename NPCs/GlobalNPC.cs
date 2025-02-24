﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using tsorcRevamp.Items;
using tsorcRevamp.Items.Weapons.Ranged;
using tsorcRevamp.Items.Weapons.Throwing;

namespace tsorcRevamp.NPCs {
    public class tsorcRevampGlobalNPC : GlobalNPC {


        float enemyValue;
        float multiplier = 1f;
        float divisorMultiplier = 1f;
        int DarkSoulQuantity;

        public override bool InstancePerEntity => true;
        public bool DarkInferno = false;
        public bool CrimsonBurn = false;
        public bool ToxicCatDrain = false;
        public bool ResetToxicCatBlobs = false;
        public bool ViruCatDrain = false;
        public bool ResetViruCatBlobs = false;
        public bool BiohazardDrain = false;
        public bool ResetBiohazardBlobs = false;
        public bool ElectrocutedEffect = false;
        public bool PolarisElectrocutedEffect = false;
        public bool CrescentMoonlight = false;
        public bool Soulstruck = false;


        public override void ResetEffects(NPC npc) {
            DarkInferno = false;
            CrimsonBurn = false;
            ToxicCatDrain = false;
            ResetToxicCatBlobs = false;
            ViruCatDrain = false;
            ResetViruCatBlobs = false;
            BiohazardDrain = false;
            ResetBiohazardBlobs = false;
            ElectrocutedEffect = false;
            PolarisElectrocutedEffect = false;
            CrescentMoonlight = false;
            Soulstruck = false;
        }


        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
            if (tsorcRevampWorld.TheEnd) {
                pool.Clear(); //stop NPC spawns in The End 
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns) {
            if (player.GetModPlayer<tsorcRevampPlayer>().BossZenBuff) {
                maxSpawns = 0;
            }
        }

        //vanilla npc changes moved to separate file

        public override void NPCLoot(NPC npc) {


            #region Dark Souls & Consumable Souls Drops

                if (Soulstruck) {
                    divisorMultiplier = 0.9f; //10% increase
                }

                if (npc.lifeMax > 5 && npc.value >= 10f || npc.boss) { //stop zero-value souls from dropping (the 'or boss' is for expert mode support)

                if (npc.netID != NPCID.JungleSlime) {
                    if (Main.expertMode) { //npc.value is the amount of coins they drop
                        enemyValue = (int)npc.value / (divisorMultiplier*25); //all enemies drop more money in expert mode, so the divisor is larger to compensate
                    }
                    else {
                        enemyValue = (int)npc.value / (divisorMultiplier*10);
                    }
                }

                if (npc.netID == NPCID.JungleSlime) //jungle slimes drop 10 souls
                {
                    if (Main.expertMode) {
                        enemyValue = (int)npc.value / (divisorMultiplier*125);
                    }
                    else {
                        enemyValue = (int)npc.value / (divisorMultiplier*50);
                    }
                }


                multiplier = tsorcRevampPlayer.CheckSoulsMultiplier(Main.LocalPlayer);

                DarkSoulQuantity = (int)(multiplier * enemyValue);

                #region Bosses drop souls once
                if (npc.boss) {
                    if (npc.type == NPCID.MoonLordCore) { //moon lord does not drop coins in 1.3, so his value is 0, but in 1.4 he has a value of 1 plat
                        DarkSoulQuantity = 100000; //1 plat / 10
                    }
                    if (tsorcRevampWorld.Slain.ContainsKey(npc.type)) {
                        DarkSoulQuantity = 0;
                        return;
                    }
                    else {
                        if (Main.netMode == NetmodeID.SinglePlayer) {
                            Main.NewText("The souls of " + npc.GivenOrTypeName + " have been released!", 175, 255, 75);
                        }
                        else if (Main.netMode == NetmodeID.Server) {
                            NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("The souls of " + npc.GivenOrTypeName + " have been released!"), new Color(175, 255, 75));
                        }
                        tsorcRevampWorld.Slain.Add(npc.type, 0);
                        if (Main.expertMode) {
                            DarkSoulQuantity = 0;
                        }
                    }
                }
                #endregion

                #region EoW drops souls in a unique way
                if (((npc.type == NPCID.EaterofWorldsHead) || (npc.type == NPCID.EaterofWorldsBody) || (npc.type == NPCID.EaterofWorldsTail)))
                {

                    DarkSoulQuantity = 110;

                    if (Main.expertMode)
                    {
                        //EoW has 5 more segments in Expert mode, so its drops per segment is reduced slightly to keep it consistent. 
                        DarkSoulQuantity = 102;
                    }
                    if (NPC.downedBoss2)
                    {
                        //EoW still drops this many souls per segment even after the first kill. The difference between normal and expert is small enough it would get rounded away at this point.
                        DarkSoulQuantity = 10;
                    }

                    Item.NewItem(npc.getRect(), mod.ItemType("DarkSoul"), DarkSoulQuantity);
                    DarkSoulQuantity = 0;
                }
                #endregion

                if (DarkSoulQuantity > 0) {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<DarkSoul>(), DarkSoulQuantity);
                }


                // Consumable Soul drops ahead - Current numbers give aprox. +20% souls

                float chance = 0.01f;

                if (((npc.type == NPCID.EaterofWorldsHead) || (npc.type == NPCID.EaterofWorldsBody) || (npc.type == NPCID.EaterofWorldsTail)) == false && !ModContent.GetInstance<tsorcRevampConfig>().LegacyMode) {
                    if (Main.LocalPlayer.GetModPlayer<tsorcRevampPlayer>().SoulSiphon) {
                        chance = 0.015f;
                    }

                    if ((enemyValue >= 1) && (enemyValue <= 200) && (Main.rand.NextFloat() < chance)) // 1% chance of all enemies between enemyValue 1 and 200 dropping FadingSoul aka 1/75
                    {
                        Item.NewItem(npc.getRect(), ModContent.ItemType<FadingSoul>(), 1); // Zombies and eyes are 6 and 7 enemyValue, so will only drop FadingSoul
                    }

                    if ((enemyValue >= 15) && (enemyValue <= 2000) && (Main.rand.NextFloat() < chance)) // 1% chance of all enemies between enemyValue 10 and 2000 dropping LostUndeadSoul aka 1/75
                    {
                        Item.NewItem(npc.getRect(), ModContent.ItemType<LostUndeadSoul>(), 1); // Most pre-HM enemies fall into this category
                    }

                    if ((enemyValue >= 55) && (enemyValue <= 10000) && (Main.rand.NextFloat() < chance)) // 1% chance of all enemies between enemyValue 50 and 10000 dropping NamelessSoldierSoul aka 1/75
                    {
                        Item.NewItem(npc.getRect(), ModContent.ItemType<NamelessSoldierSoul>(), 1); // Most HM enemies fall into this category
                    }

                    if ((enemyValue >= 150) && (enemyValue <= 10000) && (Main.rand.NextFloat() < chance) && Main.hardMode) // 1% chance of all enemies between enemyValue 150 and 10000 dropping ProudKnightSoul aka 1/75
                    {
                        Item.NewItem(npc.getRect(), ModContent.ItemType<ProudKnightSoul>(), 1);
                    }
                }
                //End consumable souls drops
            }
            #endregion

        }

            

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
            if (Main.player[projectile.owner].GetModPlayer<tsorcRevampPlayer>().ConditionOverload) {
                int buffIndex = 0;
                foreach (int buffType in npc.buffType) {

                    if (Main.debuff[buffType]) {
                        damage = (int)(damage * 1.2f); // Is it meant to be stronger with projectiles? -C
                        //nope!
                    }
                    buffIndex++;
                }
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit) {
            if (player.GetModPlayer<tsorcRevampPlayer>().ConditionOverload) {
                int buffIndex = 0;
                foreach (int buffType in npc.buffType) {

                    if (Main.debuff[buffType]) {
                        damage = (int)(damage * 1.2f);
                    }
                    buffIndex++;
                }
            }
        }

        public override bool PreNPCLoot(NPC npc) {
            if (npc.type == NPCID.ChaosElemental) {
                NPCLoader.blockLoot.Add(ItemID.RodofDiscord); //we dont want any sequence breaks, do we
            }
            if (npc.netID == NPCID.JungleSlime)

                NPCLoader.blockLoot.Add(ItemID.SlimeHook);
            return base.PreNPCLoot(npc);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage) {
            if (DarkInferno) {

                if (npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 16;
                if (damage < 2) {
                    damage = 2;
                }

                var N = npc;
                for (int j = 0; j < 6; j++) {
                    int dust = Dust.NewDust(N.position, N.width / 2, N.height / 2, 54, (N.velocity.X * 0.2f), N.velocity.Y * 0.2f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;

                    int dust2 = Dust.NewDust(N.position, N.width / 2, N.height / 2, 58, (N.velocity.X * 0.2f), N.velocity.Y * 0.2f, 100, default, 1f);
                    Main.dust[dust2].noGravity = true;
                }
            }

            if (CrimsonBurn) {
                if (npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 16;
                if (Main.hardMode) npc.lifeRegen -= 16;
                if (damage < 2) {
                    damage = 2;
                }

            }

            if (ToxicCatDrain) {
                if (npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;
                }

                int ToxicCatShotCount = 0;

                for (int i = 0; i < 1000; i++) {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<Projectiles.ToxicCatShot>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI) {
                        ToxicCatShotCount++;
                    }
                }
                if (ToxicCatShotCount >= 4) { //this is to make it worth the players time stickying more than 3 times
                    npc.lifeRegen -= ToxicCatShotCount * 2 * 2; //Use 1st N for damage, second N can be used to make it tick faster.
                    if (damage < ToxicCatShotCount * 1) {
                        damage = ToxicCatShotCount * 1;
                    }
                }
                else {
                    npc.lifeRegen -= ToxicCatShotCount * 1 * 3; 
                    if (damage < ToxicCatShotCount * 1) {
                        damage = ToxicCatShotCount * 1;
                    }
                }
            }

            if (ViruCatDrain) {
                if (npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;
                }

                int ViruCatShotCount = 0;

                for (int i = 0; i < 1000; i++) {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<Projectiles.VirulentCatShot>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI) {
                        ViruCatShotCount++;
                    }
                }
                if (ViruCatShotCount >= 4)  {
                    npc.lifeRegen -= ViruCatShotCount * 4 * 2; //I use 1st N for damage, second N can be used to make it tick faster.
                    if (damage < ViruCatShotCount * 1) {
                        damage = ViruCatShotCount * 1;
                    }
                }
                else {
                    npc.lifeRegen -= ViruCatShotCount * 2 * 3;
                    if (damage < ViruCatShotCount * 1) {
                        damage = ViruCatShotCount * 1;
                    }
                }
            }

            if (BiohazardDrain) {
                if (npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;
                }

                int BiohazardShotCount = 0;

                for (int i = 0; i < 1000; i++) {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<Projectiles.BiohazardShot>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI) {
                        BiohazardShotCount++;
                    }
                }
                if (BiohazardShotCount >= 4) {
                    npc.lifeRegen -= BiohazardShotCount * 8 * 2; //I use 1st N for damage, second N can be used to make it tick faster.
                    if (damage < BiohazardShotCount * 1) {
                        damage = BiohazardShotCount * 1;
                    }
                }
                else {
                    npc.lifeRegen -= BiohazardShotCount * 4 * 3;
                    if (damage < BiohazardShotCount * 1) {
                        damage = BiohazardShotCount * 1;
                    }
                }
            }

            if (ElectrocutedEffect) {
                if (npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 22;
                if (damage < 2) {
                    damage = 2;
                }
            }

            if (PolarisElectrocutedEffect) {
                if (npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 100;
                if (damage < 10) {
                    damage = 10;
                }
            }

            if (CrescentMoonlight) {
                if (!Main.hardMode) {
                    if (npc.lifeRegen > 0) {
                        npc.lifeRegen = 0;
                    }
                    npc.lifeRegen -= 14;
                    if (damage < 2) {
                        damage = 2;
                    }
                }
                else { //double the DoT in HM
                    if (npc.lifeRegen > 0) {
                        npc.lifeRegen = 0;
                    }
                    npc.lifeRegen -= 28;
                    if (damage < 2) {
                        damage = 2;
                    }
                }
            }
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot) {
            if (type == NPCID.Merchant && !ModContent.GetInstance<tsorcRevampConfig>().LegacyMode) {
                shop.item[nextSlot].SetDefaults(ItemID.Bottle); //despite being able to find the archeologist right after (who sells bottled water), it's nice to have
                nextSlot++;
            }
            if (type == NPCID.SkeletonMerchant && !ModContent.GetInstance<tsorcRevampConfig>().LegacyMode) {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Firebomb>());
                shop.item[nextSlot].shopCustomPrice = 25;
                shop.item[nextSlot].shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId;
                nextSlot++;

                if (Main.rand.Next(2) == 0)
                {
                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<EternalCrystal>());
                    shop.item[nextSlot].shopCustomPrice = 5000;
                    shop.item[nextSlot].shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId;
                    nextSlot++;
                }
            }
            if (type == NPCID.GoblinTinkerer && !ModContent.GetInstance<tsorcRevampConfig>().LegacyMode) {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Pulsar>());
                shop.item[nextSlot].shopCustomPrice = 5000;
                shop.item[nextSlot].shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId;
                nextSlot++;

                shop.item[nextSlot].SetDefaults(ModContent.ItemType<ToxicCatalyzer>());
                shop.item[nextSlot].shopCustomPrice = 5000;
                shop.item[nextSlot].shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId;
                nextSlot++;
            }
            if (type == NPCID.Dryad && !ModContent.GetInstance<tsorcRevampConfig>().LegacyMode)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BloodredMossClump>());
                shop.item[nextSlot].shopCustomPrice = 25;
                shop.item[nextSlot].shopSpecialCurrency = tsorcRevamp.DarkSoulCustomCurrencyId;
                nextSlot++;
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) {
            if (npc.GetGlobalNPC<tsorcRevampGlobalNPC>().ToxicCatDrain && (projectile.type == ModContent.ProjectileType<Projectiles.ToxicCatDetonator>() || projectile.type == ModContent.ProjectileType<Projectiles.ToxicCatExplosion>())) {
                npc.GetGlobalNPC<tsorcRevampGlobalNPC>().ResetToxicCatBlobs = true;
                int tags;

                for (int i = 0; i < 1000; i++) {
                    tags = 0;
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<Projectiles.ToxicCatShot>() && p.ai[0] == 1f && p.timeLeft > 2 && p.ai[1] == npc.whoAmI) {
                        for (int q = 0; q < 1000; q++) {
                            Projectile ñ = Main.projectile[q];
                            if (ñ.active && ñ.type == ModContent.ProjectileType<Projectiles.ToxicCatShot>() && ñ.ai[0] == 1f && ñ.ai[1] == npc.whoAmI) {
                                tags++;
                            }
                        }
                        float volume = (tags * 0.3f) + 0.7f;
                        float pitch = tags * 0.08f;
                        Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, SoundID.Item74.Style, volume, -pitch);

                        p.timeLeft = 2;

                        Projectile.NewProjectile(p.Center, npc.velocity, ModContent.ProjectileType<Projectiles.ToxicCatExplosion>(), (int)(projectile.damage * 1.8f), projectile.knockBack, projectile.owner, tags, 0);

                        int buffindex = npc.FindBuffIndex(ModContent.BuffType<Buffs.ToxicCatDrain>());

                        if (buffindex != -1) {
                            npc.DelBuff(buffindex);
                        }
                    }
                }
            }

            if (npc.GetGlobalNPC<tsorcRevampGlobalNPC>().ViruCatDrain && (projectile.type == ModContent.ProjectileType<Projectiles.VirulentCatDetonator>() || projectile.type == ModContent.ProjectileType<Projectiles.VirulentCatExplosion>())) {
                npc.GetGlobalNPC<tsorcRevampGlobalNPC>().ResetViruCatBlobs = true;
                int tags;

                for (int i = 0; i < 1000; i++) {
                    tags = 0;
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<Projectiles.VirulentCatShot>() && p.ai[0] == 1f && p.timeLeft > 2 && p.ai[1] == npc.whoAmI) {
                        for (int q = 0; q < 1000; q++) {
                            Projectile ñ = Main.projectile[q];
                            if (ñ.active && ñ.type == ModContent.ProjectileType<Projectiles.VirulentCatShot>() && ñ.ai[0] == 1f && ñ.ai[1] == npc.whoAmI) {
                                tags++;
                            }
                        }
                        float volume = (tags * 0.3f) + 0.7f;
                        float pitch = tags * 0.08f;
                        Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, SoundID.Item74.Style, volume, -pitch);

                        //Main.NewText(pitch);
                        p.timeLeft = 2;

                        Projectile.NewProjectile(p.Center, npc.velocity, ModContent.ProjectileType<Projectiles.VirulentCatExplosion>(), (projectile.damage * 2), projectile.knockBack, projectile.owner, tags, 0);

                        int buffindex = npc.FindBuffIndex(ModContent.BuffType<Buffs.ViruCatDrain>());

                        if (buffindex != -1) {
                            npc.DelBuff(buffindex);
                        }
                    }
                }
            }

            if (npc.GetGlobalNPC<tsorcRevampGlobalNPC>().BiohazardDrain && (projectile.type == ModContent.ProjectileType<Projectiles.BiohazardDetonator>() || projectile.type == ModContent.ProjectileType<Projectiles.BiohazardExplosion>())) {
                npc.GetGlobalNPC<tsorcRevampGlobalNPC>().ResetBiohazardBlobs = true;
                int tags;

                for (int i = 0; i < 1000; i++) {
                    tags = 0;
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<Projectiles.BiohazardShot>() && p.ai[0] == 1f && p.timeLeft > 2 && p.ai[1] == npc.whoAmI) {
                        for (int q = 0; q < 1000; q++) {
                            Projectile ñ = Main.projectile[q];
                            if (ñ.active && ñ.type == ModContent.ProjectileType<Projectiles.BiohazardShot>() && ñ.ai[0] == 1f && ñ.ai[1] == npc.whoAmI) {
                                tags++;
                            }
                        }
                        float volume = (tags * 0.3f) + 0.7f;
                        float pitch = tags * 0.08f;
                        Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, SoundID.Item74.Style, volume, -pitch);

                        p.timeLeft = 2;

                        Projectile.NewProjectile(p.Center, npc.velocity, ModContent.ProjectileType<Projectiles.BiohazardExplosion>(), (projectile.damage * 2), projectile.knockBack, projectile.owner, tags, 0);

                        int buffindex = npc.FindBuffIndex(ModContent.BuffType<Buffs.BiohazardDrain>());

                        if (buffindex != -1) {
                            npc.DelBuff(buffindex);
                        }
                    }
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor) {
            if (ElectrocutedEffect) {
                int dust = Dust.NewDust(npc.position, npc.width, npc.height, 226, npc.velocity.X * 0f, npc.velocity.Y * 0f, 100, default(Color), .4f);
                Main.dust[dust].noGravity = true;
            }

            if (PolarisElectrocutedEffect) {
                for (int i = 0; i < 2; i++) {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 226, npc.velocity.X * 0f, npc.velocity.Y * 0f, 100, default(Color), .4f);
                    Main.dust[dust].noGravity = true;
                }
                if (Main.rand.Next(2) == 0) {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 226, npc.velocity.X * 0f, npc.velocity.Y * 0f, 100, default(Color), .4f);
                    Main.dust[dust].noGravity = false;
                }
            }

            if (ToxicCatDrain) {
                drawColor = Color.LimeGreen;
                Lighting.AddLight(npc.position, 0.125f, 0.23f, 0.065f);

                if (Main.rand.Next(10) == 0) {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 74, npc.velocity.X * 0f, npc.velocity.Y * 0f, 100, default(Color), .8f); ;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity += npc.velocity;
                    Main.dust[dust].fadeIn = 1f;
                }
            }

            if (ViruCatDrain) {
                drawColor = Color.LimeGreen;
                Lighting.AddLight(npc.position, 0.125f, 0.23f, 0.065f);

                if (Main.rand.Next(6) == 0) {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 74, npc.velocity.X * 0f, npc.velocity.Y * 0f, 100, default(Color), .8f); ;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity += npc.velocity;
                    Main.dust[dust].fadeIn = 1f;
                }
            }

            if (BiohazardDrain) {
                drawColor = Color.LimeGreen;
                Lighting.AddLight(npc.position, 0.125f, 0.23f, 0.065f);

                if (Main.rand.Next(2) == 0) {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 74, npc.velocity.X * 0f, -2f, 100, default(Color), .8f); ;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity += npc.velocity;
                    Main.dust[dust].fadeIn = 1f;
                }
            }

            if (CrescentMoonlight) {
                drawColor = Color.White;

                int dust = Dust.NewDust(npc.position, npc.width, npc.height, 164, npc.velocity.X * 0f, 0f, 100, default(Color), 1f); ;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = false;
                Main.dust[dust].velocity += npc.velocity;
            }

            if (Soulstruck) {
                Lighting.AddLight(npc.Center, .4f, .4f, .850f);

                if (Main.rand.Next(6) == 0)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 68, 0, 0, 30, default(Color), 1.25f);
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity += npc.velocity;
                }
            }
        }



        #region AIWorm
        //higher values cause the wvyern's 'gravity' towards the player to increase
        //lower values basically == longer passes
        public static void AIWorm(NPC npc, int headType, int[] bodyTypes, int tailType, int wormLength = 3, float partDistanceAddon = 0f, float maxSpeed = 8f, float gravityResist = 0.07f, bool fly = false, bool split = false, bool ignoreTiles = false, bool spawnTileDust = true, bool soundEffects = true)
        {
            if (split)
            {
                npc.realLife = -1;
            }
            else
            if (npc.ai[3] > 0f) { npc.realLife = (int)npc.ai[3]; }

            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead) { npc.TargetClosest(true); }
            if (Main.player[npc.target].dead && npc.timeLeft > 300) { npc.timeLeft = 300; }
            if (Main.netMode != 1)
            {
                //spawn pieces (flying)
                if (fly && npc.type == headType && npc.ai[0] == 0f)
                {
                    npc.ai[3] = (float)npc.whoAmI;
                    npc.realLife = npc.whoAmI;
                    int npcID = npc.whoAmI;

                    for (int m = 0; m < wormLength - 1; m++)
                    {
                        int npcType = (m == wormLength - 2 ? tailType : bodyTypes[m]);

                        int newnpcID = NPC.NewNPC((int)(npc.Center.X), (int)(npc.Center.Y), npcType, npc.whoAmI);
                        Main.npc[newnpcID].ai[3] = (float)npc.whoAmI;
                        Main.npc[newnpcID].realLife = npc.whoAmI;
                        Main.npc[newnpcID].ai[1] = (float)npcID;
                        Main.npc[npcID].ai[0] = (float)newnpcID;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, newnpcID);
                        npcID = newnpcID;
                    }
                    npc.netUpdate = true;
                }
                else //spawn pieces
                if ((npc.type == headType || (npc.type != headType && npc.type != tailType)) && npc.ai[0] == 0f)
                {
                    if (npc.type == headType)
                    {
                        if (!split)
                        {
                            npc.ai[3] = (float)npc.whoAmI;
                            npc.realLife = npc.whoAmI;
                        }
                        npc.ai[2] = (float)(wormLength - 2);
                        int nextPiece = (bodyTypes.Length == 0 ? tailType : bodyTypes[0]);
                        npc.ai[0] = (float)NPC.NewNPC((int)(npc.Center.X), (int)(npc.Center.Y), nextPiece, npc.whoAmI);
                    }
                    else
                    if ((npc.type != headType && npc.type != tailType) && npc.ai[2] > 0f)
                    {
                        npc.ai[0] = (float)NPC.NewNPC((int)(npc.Center.X), (int)(npc.Center.Y), bodyTypes[wormLength - 3 - (int)npc.ai[2]], npc.whoAmI);
                    }
                    else
                    {
                        npc.ai[0] = (float)NPC.NewNPC((int)(npc.Center.X), (int)(npc.Center.Y), tailType, npc.whoAmI);
                    }
                    if (!split)
                    {
                        Main.npc[(int)npc.ai[0]].ai[3] = npc.ai[3];
                        Main.npc[(int)npc.ai[0]].realLife = npc.realLife;
                    }
                    Main.npc[(int)npc.ai[0]].ai[1] = (float)npc.whoAmI;
                    Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
                    npc.netUpdate = true;
                }
                //if npc can split, check if pieces are dead and if so split.
                if (split)
                {
                    if (!Main.npc[(int)npc.ai[1]].active && !Main.npc[(int)npc.ai[0]].active)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 10.0);
                        npc.active = false;
                    }
                    if (npc.type == headType && !Main.npc[(int)npc.ai[0]].active)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 10.0);
                        npc.active = false;
                    }
                    if (npc.type == tailType && !Main.npc[(int)npc.ai[1]].active)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 10.0);
                        npc.active = false;
                    }
                    if ((npc.type != headType && npc.type != tailType) && !Main.npc[(int)npc.ai[1]].active)
                    {
                        npc.type = headType;
                        int npcID = npc.whoAmI;
                        float lifePercent = (float)npc.life / (float)npc.lifeMax;
                        float lastPiece = npc.ai[0];
                        npc.SetDefaults(npc.type, -1f);
                        npc.life = (int)((float)npc.lifeMax * lifePercent);
                        npc.ai[0] = lastPiece;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                        npc.whoAmI = npcID;
                    }
                    else
                    if ((npc.type != headType && npc.type != tailType) && !Main.npc[(int)npc.ai[0]].active)
                    {
                        npc.type = tailType;
                        int npcID = npc.whoAmI;
                        float lifePercent = (float)npc.life / (float)npc.lifeMax;
                        float lastPiece = npc.ai[1];
                        npc.SetDefaults(npc.type, -1f);
                        npc.life = (int)((float)npc.lifeMax * lifePercent);
                        npc.ai[1] = lastPiece;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                        npc.whoAmI = npcID;
                    }
                }
                else
                {
                    if (npc.type != headType && (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != npc.aiStyle))
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 10.0);

                        npc.active = false;
                    }
                    if (npc.type != tailType && (!Main.npc[(int)npc.ai[0]].active || Main.npc[(int)npc.ai[0]].aiStyle != npc.aiStyle))
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 10.0);

                        npc.active = false;
                    }
                }
                /**
                if (!npc.active && Main.netMode == NetmodeID.Server) 
                { 
                    NetMessage.SendData(28, -1, -1, "", npc.whoAmI, 1, 0f, 0f, -1); 
                }**/
            }
            int tileX = (int)(npc.position.X / 16f) - 1;
            int tileCenterX = (int)((npc.Center.X) / 16f) + 2;
            int tileY = (int)(npc.position.Y / 16f) - 1;
            int tileCenterY = (int)((npc.Center.Y) / 16f) + 2;
            if (tileX < 0) { tileX = 0; }
            if (tileCenterX > Main.maxTilesX) { tileCenterX = Main.maxTilesX; }
            if (tileY < 0) { tileY = 0; }
            if (tileCenterY > Main.maxTilesY) { tileCenterY = Main.maxTilesY; }
            bool canMove = false;
            if (fly || ignoreTiles) { canMove = true; }
           
            
            if (!canMove || spawnTileDust)
            {
                for (int tX = tileX; tX < tileCenterX; tX++)
                {
                    for (int tY = tileY; tY < tileCenterY; tY++)
                    {
                        if (Main.tile[tX, tY] != null && ((Main.tile[tX, tY].active() && (Main.tileSolid[(int)Main.tile[tX, tY].type] || (Main.tileSolidTop[(int)Main.tile[tX, tY].type] && Main.tile[tX, tY].frameY == 0))) || Main.tile[tX, tY].liquid > 64))
                        {
                            Vector2 tPos;
                            tPos.X = (float)(tX * 16);
                            tPos.Y = (float)(tY * 16);
                            if (npc.position.X + (float)npc.width > tPos.X && npc.position.X < tPos.X + 16f && npc.position.Y + (float)npc.height > tPos.Y && npc.position.Y < tPos.Y + 16f)
                            {
                                canMove = true;
                                if (spawnTileDust && (Main.rand.Next(100)) == 0 && Main.tile[tX, tY].active())
                                {
                                    WorldGen.KillTile(tX, tY, true, true, false);
                                }
                            }
                        }
                    }
                }
            }
            

            if (!canMove && npc.type == headType)
            {
                Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int playerCheckDistance = 1000;
                bool canMove2 = true;
                for (int m3 = 0; m3 < 255; m3++)
                {
                    if (Main.player[m3].active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[m3].position.X - playerCheckDistance, (int)Main.player[m3].position.Y - playerCheckDistance, playerCheckDistance * 2, playerCheckDistance * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            canMove2 = false;
                            break;
                        }
                    }
                }
                if (canMove2) { canMove = true; }
            }
            if (fly)
            {
                if (npc.velocity.X < 0f) { npc.spriteDirection = 1; }
                else
                if (npc.velocity.X > 0f) { npc.spriteDirection = -1; }

            }


            Vector2 npcCenter = npc.Center;
            float playerCenterX = Main.player[npc.target].Center.X;
            float playerCenterY = Main.player[npc.target].Center.Y;
            playerCenterX = (float)((int)(playerCenterX / 16f) * 16); playerCenterY = (float)((int)(playerCenterY / 16f) * 16);
            npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16); npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
            playerCenterX -= npcCenter.X; playerCenterY -= npcCenter.Y;
            float dist = (float)Math.Sqrt((double)(playerCenterX * playerCenterX + playerCenterY * playerCenterY));
            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    npcCenter = npc.Center;
                    playerCenterX = Main.npc[(int)npc.ai[1]].Center.X - npcCenter.X;
                    playerCenterY = Main.npc[(int)npc.ai[1]].Center.Y - npcCenter.Y;
                }
                catch
                {
                }
                npc.rotation = (float)Math.Atan2((double)playerCenterY, (double)playerCenterX) + 1.57f;
                dist = (float)Math.Sqrt((double)(playerCenterX * playerCenterX + playerCenterY * playerCenterY));
                dist = (dist - (float)npc.width - (float)partDistanceAddon) / dist;
                playerCenterX *= dist;
                playerCenterY *= dist;
                npc.velocity = default(Vector2);
                npc.position.X = npc.position.X + playerCenterX;
                npc.position.Y = npc.position.Y + playerCenterY;
                if (fly)
                {
                    if (playerCenterX < 0f) { npc.spriteDirection = 1; return; } //comment these two lines out to make dargon not flip
                    if (playerCenterX > 0f) { npc.spriteDirection = -1; return; } //
                }
            }
            else
            {
                if (!canMove)
                {
                    npc.TargetClosest(true);
                    npc.velocity.Y = npc.velocity.Y + 0.11f;
                    if (npc.velocity.Y > maxSpeed) { npc.velocity.Y = maxSpeed; }
                    if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)maxSpeed * 0.4)
                    {
                        if (npc.velocity.X < 0f) { npc.velocity.X = npc.velocity.X - gravityResist * 1.1f; } else { npc.velocity.X = npc.velocity.X + gravityResist * 1.1f; }
                    }
                    else
                    if (npc.velocity.Y == maxSpeed)
                    {
                        if (npc.velocity.X < playerCenterX) { npc.velocity.X = npc.velocity.X + gravityResist; }
                        else
                        if (npc.velocity.X > playerCenterX) { npc.velocity.X = npc.velocity.X - gravityResist; }
                    }
                    else
                    if (npc.velocity.Y > 4f)
                    {
                        if (npc.velocity.X < 0f) { npc.velocity.X = npc.velocity.X + gravityResist * 0.9f; } else { npc.velocity.X = npc.velocity.X - gravityResist * 0.9f; }
                    }
                }
                else
                {
                    if (soundEffects && npc.soundDelay == 0)
                    {
                        float distSoundDelay = dist / 40f;
                        if (distSoundDelay < 10f) { distSoundDelay = 10f; }
                        if (distSoundDelay > 20f) { distSoundDelay = 20f; }
                        npc.soundDelay = (int)distSoundDelay;
                        Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 1);
                    }
                    dist = (float)Math.Sqrt((double)(playerCenterX * playerCenterX + playerCenterY * playerCenterY));
                    float absPlayerCenterX = Math.Abs(playerCenterX);
                    float absPlayerCenterY = Math.Abs(playerCenterY);
                    float newSpeed = maxSpeed / dist;
                    playerCenterX *= newSpeed;
                    playerCenterY *= newSpeed;
                    bool dontFall = false;
                    if (fly)
                    {
                        if (((npc.velocity.X > 0f && playerCenterX < 0f) || (npc.velocity.X < 0f && playerCenterX > 0f) || (npc.velocity.Y > 0f && playerCenterY < 0f) || (npc.velocity.Y < 0f && playerCenterY > 0f)) && Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) > gravityResist / 2f && dist < 300f)
                        {
                            dontFall = true;
                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < maxSpeed) { npc.velocity *= 1.1f; }
                        }
                        //if (npc.position.Y > Main.player[npc.target].position.Y || (double)(Main.player[npc.target].position.Y / 16f) > Main.worldSurface || Main.player[npc.target].dead) //part jungle wyvern needs 
                        if (Main.player[npc.target].dead) //part jungle wyvern needs 
                        {
                            dontFall = true;
                            if (Math.Abs(npc.velocity.X) < maxSpeed / 2f)
                            {
                                if (npc.velocity.X == 0f) { npc.velocity.X = npc.velocity.X - (float)npc.direction; }
                                npc.velocity.X = npc.velocity.X * 1.1f;
                            }
                            else
                            if (npc.velocity.Y > -maxSpeed) { npc.velocity.Y = npc.velocity.Y - gravityResist; }
                        }
                    }
                    if (!dontFall)
                    {
                        if ((npc.velocity.X > 0f && playerCenterX > 0f) || (npc.velocity.X < 0f && playerCenterX < 0f) || (npc.velocity.Y > 0f && playerCenterY > 0f) || (npc.velocity.Y < 0f && playerCenterY < 0f))
                        {
                            if (npc.velocity.X < playerCenterX) { npc.velocity.X = npc.velocity.X + gravityResist; }
                            else
                            if (npc.velocity.X > playerCenterX) { npc.velocity.X = npc.velocity.X - gravityResist; }
                            if (npc.velocity.Y < playerCenterY) { npc.velocity.Y = npc.velocity.Y + gravityResist; }
                            else
                            if (npc.velocity.Y > playerCenterY) { npc.velocity.Y = npc.velocity.Y - gravityResist; }
                            if ((double)Math.Abs(playerCenterY) < (double)maxSpeed * 0.2 && ((npc.velocity.X > 0f && playerCenterX < 0f) || (npc.velocity.X < 0f && playerCenterX > 0f)))
                            {
                                if (npc.velocity.Y > 0f) { npc.velocity.Y = npc.velocity.Y + gravityResist * 2f; } else { npc.velocity.Y = npc.velocity.Y - gravityResist * 2f; }
                            }
                            if ((double)Math.Abs(playerCenterX) < (double)maxSpeed * 0.2 && ((npc.velocity.Y > 0f && playerCenterY < 0f) || (npc.velocity.Y < 0f && playerCenterY > 0f)))
                            {
                                if (npc.velocity.X > 0f) { npc.velocity.X = npc.velocity.X + gravityResist * 2f; } else { npc.velocity.X = npc.velocity.X - gravityResist * 2f; }
                            }
                        }
                        else
                        if (absPlayerCenterX > absPlayerCenterY)
                        {
                            if (npc.velocity.X < playerCenterX) { npc.velocity.X = npc.velocity.X + gravityResist * 1.1f; }
                            else
                            if (npc.velocity.X > playerCenterX) { npc.velocity.X = npc.velocity.X - gravityResist * 1.1f; }

                            if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)maxSpeed * 0.5)
                            {
                                if (npc.velocity.Y > 0f) { npc.velocity.Y = npc.velocity.Y + gravityResist; } else { npc.velocity.Y = npc.velocity.Y - gravityResist; }
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < playerCenterY) { npc.velocity.Y = npc.velocity.Y + gravityResist * 1.1f; }
                            else
                            if (npc.velocity.Y > playerCenterY) { npc.velocity.Y = npc.velocity.Y - gravityResist * 1.1f; }
                            if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)maxSpeed * 0.5)
                            {
                                if (npc.velocity.X > 0f) { npc.velocity.X = npc.velocity.X + gravityResist; } else { npc.velocity.X = npc.velocity.X - gravityResist; }
                            }
                        }
                    }
                }
                npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
                if (npc.type == headType)
                {
                    if (canMove)
                    {
                        if (npc.localAI[0] != 1f) { npc.netUpdate = true; }
                        npc.localAI[0] = 1f;
                    }
                    else
                    {
                        if (npc.localAI[0] != 0f) { npc.netUpdate = true; }
                        npc.localAI[0] = 0f;
                    }
                    if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                    {
                        npc.netUpdate = true;
                        return;
                    }
                }
            }
        }

        #endregion
    }
}
