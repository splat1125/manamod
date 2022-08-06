
using Microsoft.Xna.Framework;
using System;
using manamod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace manamod.Projectiles.Magic
{
	public class SparkProj_Bolt : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.timeLeft = 100;
			Projectile.alpha = 255;

			//Projectile.penetrate = -1;
			Projectile.extraUpdates = 25;
	}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spark Bolt");
		}
		public override string Texture => "manamod/invisibleTexture";

		public static int maxSegmentLength = 10;	//default 10
		public static int maxTime = 100;                    //how many steps will we live for?
		public int timer => maxTime - Projectile.timeLeft;	//how many steps have we been alive for?
		public float progress => timer / (float)maxTime;	//progress of the whole bolt from 0 to 1
		public float progressInv => 1f - progress;    //progress of the whole bolt from 1 to 0
		public int getSegmentLength => (int)(MathHelper.Clamp(progressInv, 0.5f, 1) * maxSegmentLength);	//bolt will be made of shorter and shorter segments as it reaches its end
		public float getDeviationAngle => progress * MathHelper.ToRadians(80f);    //bolt will be able to deviate more and more as it reaches its end
		public float segmentTarget = 0;
		public ref float initialAngle => ref Projectile.ai[0];
		public float clampHigh;
		public float clampLow;

		public override void AI()
		{
			if (Projectile.wet)
				Projectile.Kill();
				
			/*if (timer == 0)
			{
				initialAngle = Projectile.velocity.ToRotation();
				clampHigh = initialAngle + MathHelper.ToRadians(65f);
				clampLow = initialAngle - MathHelper.ToRadians(65f);
			}*/

			if (timer > segmentTarget)
			{
				segmentTarget += getSegmentLength;
				//Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.Clamp(getDeviationAngle, clampLow, clampHigh));
				Projectile.velocity = Projectile.velocity.RotatedByRandom(getDeviationAngle);
			}
			if (timer == 10)	//starter dust
			{
				for (int i = 0; i < 5; i++)
				{
					int sparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 169, 0f, 0f, 150);    //golden sparks
					Main.dust[sparks].velocity *= 6f;
					Main.dust[sparks].velocity += Projectile.velocity;
					Main.dust[sparks].scale = 1.2f;
					Main.dust[sparks].noGravity = true;
				}				
				for (int i = 0; i < 20; i++)
				{
					int sparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 158, 0f, 0f, 150);    //orange sparks
					Main.dust[sparks].velocity *= 5f;
					Main.dust[sparks].velocity += Projectile.velocity;
					Main.dust[sparks].noGravity = true;
				}
			}
            else if (timer > 10)//tracker dust
            {
				int core = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 170, 0f, 0f, 150);    //bolt core (golden)
				Main.dust[core].noGravity = true;
				Main.dust[core].velocity = Vector2.Zero;
				Main.dust[core].scale = progress + 0.5f;
				int sparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 158, 0f, 0f, 150);    //bolt "spray"
				Main.dust[sparks].noGravity = true;
				Main.dust[sparks].velocity *= progress * 4;
				Main.dust[sparks].scale = progress + 0.75f;				
			}
		}

		public override void Kill(int timeLeft)
        {
			if(!Projectile.wet)
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SparkProj_Bolt_Explosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			/*for (int i = 0; i < 10; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 158, 0f, 0f, 150);
			}*/
		}
	}
}