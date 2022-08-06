using manamod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace manamod.Projectiles.Magic.SpaceGun
{
    public class SpaceGunSpark : ModProjectile
    {
        public override string Texture => "manamod/invisibleTexture";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volatile Spark");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = true;
            Projectile.penetrate = 5;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.noEnchantmentVisuals = true;
        }

        public int existTimer = 0;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0, 0.2f, 0);
            if (existTimer >= 3)
            {
                int a = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PoisonStaff, 0f, 0f, 100, default(Color), 1f);
                ref float ptr = ref Main.dust[a].position.X;
                ptr -= 2f;
                ptr = ref Main.dust[a].position.Y;
                ptr += 2f;
                Dust dust2 = Main.dust[a];
                dust2.scale += Main.rand.Next(50) * 0.001f;
                Main.dust[a].noGravity = true;
                ptr = ref Main.dust[a].velocity.Y;
                ptr -= 2f;
                ref float ptr2 = ref Projectile.ai[0];
                ptr2 += 1f;
                if (Projectile.ai[0] > 5f)
                {
                    Projectile.ai[0] = 5f;
                    if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                    {
                        ptr = ref Projectile.velocity.X;
                        ptr *= 0.97f;
                        if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01)
                        {
                            Projectile.velocity.X = 0f;
                            Projectile.netUpdate = true;
                        }
                    }
                    ptr = ref Projectile.velocity.Y;
                    ptr += 0.2f;
                }
                Projectile.rotation += Projectile.velocity.X * 0.1f;

                if (Main.rand.NextBool(2))
                {
                    int b = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PoisonStaff, 0f, 0f, 100, default(Color), 1f);
                    ptr = ref Main.dust[b].position.X;
                    ptr -= 2f;
                    ptr = ref Main.dust[b].position.Y;
                    ptr += 2f;
                    dust2 = Main.dust[b];
                    dust2.scale += 0.3f + Main.rand.Next(50) * 0.001f;
                    Main.dust[b].noGravity = true;
                    dust2 = Main.dust[b];
                    dust2.velocity *= 0.1f;
                }
                if (Projectile.velocity.Y < 0.25 && Projectile.velocity.Y > 0.15)
                {
                    ptr = ref Projectile.velocity.X;
                    ptr *= 0.8f;
                }
            }
            if (Projectile.wet)
                Projectile.Kill();


            Projectile.rotation = -Projectile.velocity.X * 0.05f;
            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
            existTimer++;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.velocity.X != Projectile.velocity.X)
                Projectile.velocity.X *= -0.1f;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}
