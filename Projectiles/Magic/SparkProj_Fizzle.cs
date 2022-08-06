
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace manamod.Projectiles.Magic
{
	public class SparkProj_Fizzle : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 5;
			Projectile.height = 5;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.timeLeft = 15;
			Projectile.alpha = 255;

			Projectile.penetrate = -1;
			Projectile.extraUpdates = 3;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 15;
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Weak Spark");
		}
		public override string Texture => "manamod/invisibleTexture";
		public ref float timer => ref Projectile.ai[0];
		public override void AI()
        {
			if (Projectile.wet)
				Projectile.Kill();

			if (timer >= 20f)
			{
				Projectile.velocity.X *= 0.99f;
				Projectile.velocity.Y += 0.1f;
			}
			if (timer > 4)
			{
				int sparks = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 150);
				Main.dust[sparks].noGravity = true;
				Main.dust[sparks].scale = 0.75f;
				Main.dust[sparks].velocity *= 4f;
				Main.dust[sparks].velocity += Projectile.velocity * 0.75f;

				int core = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 228, 0f, 0f, 150);    //fizzle core (golden)
				Main.dust[core].noGravity = true;
				Main.dust[core].velocity *= 2f;
				Main.dust[core].scale = 0.5f;
			}
			timer++;
        }

        public override void Kill(int timeLeft)
        {
			/*for (int i = 0; i < 10; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 158, 0f, 0f, 150);
			}*/
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 60);
		}
	}
}