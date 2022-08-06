
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace manamod.Projectiles.Magic
{
	public class SparkProj_Base : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.timeLeft = 80;
			Projectile.alpha = 255;

			Projectile.penetrate = 2;
			Projectile.extraUpdates = 2;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spark");
		}
		public override string Texture => "manamod/invisibleTexture";
		public ref float timer => ref Projectile.ai[0];
		public override void AI()
        {
			if (Projectile.wet)
				Projectile.Kill();

			if (timer == 0)
            {
				Projectile.velocity += Main.rand.NextVector2Circular(0.25f, 0.25f);
            }

			if(timer == 6)
            {
				for (int i = 0; i < 6; i++)
				{
					int sparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 228, 0f, 0f, 150);    //golden sparks
					Main.dust[sparks].velocity *= 5f;
					Main.dust[sparks].velocity += Projectile.velocity;
					Main.dust[sparks].scale = 1f;
					Main.dust[sparks].noGravity = true;
				}
				for (int i = 0; i < 7; i++)
				{
					int sparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 158, 0f, 0f, 150);    //orange sparks
					Main.dust[sparks].velocity *= 5f;
					Main.dust[sparks].velocity += Projectile.velocity;
					Main.dust[sparks].noGravity = true;
				}
		
			}

			if (timer > 6)
			{
				int sparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 150);
				Main.dust[sparks].noGravity = true;
				Main.dust[sparks].velocity *= 2f;
				Main.dust[sparks].velocity += Projectile.velocity;

				int trail = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 292, 0f, 0f, 150);    //spark trail
				Main.dust[trail].velocity *= 0.5f;
				Main.dust[trail].scale = 0.5f;
				Main.dust[trail].noGravity = true;
				Main.dust[trail].velocity += Projectile.velocity;
			}

			if (timer >= 40)
			{
				Projectile.velocity.X *= 0.95f;
				Projectile.velocity.Y += 0.05f;
				Projectile.velocity = Projectile.velocity.RotatedByRandom(0.3f);
			}

			//int dustChance = (int)(timer < 7 ? 0 : (100f * (Projectile.timeLeft / 60f))); 

			//if (Main.rand.Next(100) < dustChance)	


			timer++;
		}

        public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 10; i++)
			{
				int deathSparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 150);
				Main.dust[deathSparks].scale = 0.4f;
				Main.dust[deathSparks].noLightEmittence = true;
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.Next(2) == 0)
			{
				target.AddBuff(BuffID.OnFire, 90);
			}
		}
	}
}