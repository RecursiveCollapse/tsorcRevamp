﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tsorcRevamp.NPCs.Bosses.WyvernMage
{
	class MechaDragonTail : ModNPC {

		public override void SetDefaults() {
			npc.netAlways = true;
			npc.boss = true;
			npc.npcSlots = 1;
			npc.aiStyle = 6;
			npc.width = 45;
			npc.height = 45;
			npc.knockBackResist = 0f;
			npc.timeLeft = 1750;
			npc.damage = 55;
			npc.defense = 0;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath10;
			npc.lifeMax = 35000;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.behindTiles = true;
			npc.value = 25000;
			npc.buffImmune[BuffID.Poisoned] = true;
			npc.buffImmune[BuffID.OnFire] = true;
			npc.buffImmune[BuffID.Confused] = true;
			npc.buffImmune[BuffID.CursedInferno] = true;
		}

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Wyvern Mage Disciple");
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale) {
			npc.lifeMax = (int)(npc.lifeMax * 0.7f * bossLifeScale);
		}
		public override void AI() {

			//Generic Worm Part Code:
			tsorcRevampGlobalNPC.AIWorm(npc, ModContent.NPCType<MechaDragonHead>(), MechaDragonHead.bodyTypes, ModContent.NPCType<MechaDragonTail>(), 23, -1f, 12f, 0.13f, true, false);

			//Code unique to this body part:
			if (!Main.npc[(int)npc.ai[1]].active)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.active = false;
			}

			Color color = new Color();
			int dust = Dust.NewDust(new Vector2((float)npc.position.X, (float)npc.position.Y + 10), npc.width, npc.height, 6, 0, 0, 100, color, 1.0f);
			Main.dust[dust].noGravity = true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) {
			Vector2 origin = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Color alpha = Color.White;
			SpriteEffects effects = SpriteEffects.None;
			if (npc.spriteDirection == 1) {
				effects = SpriteEffects.FlipHorizontally;
			}
			spriteBatch.Draw(Main.npcTexture[npc.type], new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)Main.npcTexture[npc.type].Width * npc.scale / 2f + origin.X * npc.scale, npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)Main.npcTexture[npc.type].Height * npc.scale / (float)Main.npcFrameCount[npc.type] + 4f + origin.Y * npc.scale + 56f), npc.frame, alpha, npc.rotation, origin, npc.scale, effects, 0f);
			npc.alpha = 255;
			return true;
		}
	}
}
