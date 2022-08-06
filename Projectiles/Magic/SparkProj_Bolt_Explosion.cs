
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace manamod.Projectiles.Magic
{
	public class SparkProj_Bolt_Explosion : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 64;
			Projectile.height = 64;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.hostile = true;
			Projectile.timeLeft = 5;
			Projectile.alpha = 255;
			Projectile.tileCollide = false;

			Projectile.penetrate = -1;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spark Explosion");
		}
		public override string Texture => "manamod/invisibleTexture";
		public ref float timer => ref Projectile.ai[0];
		public override void AI()
		{
			if (timer == 0)
			{
				for (int i = 0; i < 25; i++)
				{
					int body = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 159, 0f, 0f, 150, new Color(255, 100, 0));	//lingering sparks
					Main.dust[body].scale = 0.4f;
					Main.dust[body].velocity *= 2f;
				}
				for (int i = 0; i < 55; i++)
				{
					int sparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 150);	//orange sparks
					Main.dust[sparks].position = Projectile.Center;
					Main.dust[sparks].velocity = Vector2.Normalize(Main.dust[sparks].velocity);
					Main.dust[sparks].velocity *= Main.rand.NextFloat(5f, 10f);
					Main.dust[sparks].scale *= 1.1f;
					Main.dust[sparks].noGravity = true;

					int goldsparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 228, 0f, 0f, 150);    //golden sparks
					Main.dust[goldsparks].velocity = Vector2.Normalize(Main.dust[goldsparks].velocity);
					Main.dust[goldsparks].velocity *= Main.rand.NextFloat(5f);
					Main.dust[goldsparks].position = Projectile.Center;
					Main.dust[goldsparks].scale = 1.2f;
					Main.dust[goldsparks].noGravity = true;
				}
			}
			timer++;
		}
		public override bool? CanHitNPC(NPC target) => !target.friendly ? null : false;
		public override bool CanHitPlayer(Player target) => (Projectile.Center.Distance(target.Center) < Projectile.width/2) ? true : false;
		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			damage /= 4;
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (!target.townNPC)
			{
				target.AddBuff(BuffID.OnFire, 60);
			}
		}
		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			if (!target.mount.Active || !target.mount.Cart)
				target.velocity = Projectile.Center.DirectionTo(target.Center) * 7.5f;
		}
	}
}