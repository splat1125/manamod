using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace manamod.Projectiles.Magic.SpaceGun
{
    public class SpaceGunLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Green Laser");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 3;
            Projectile.scale = 1.4f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public int existTimer = 0;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0, 0.2f, 0);
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            if (existTimer == 3)
            {
                DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
                Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 10f, 8f, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
                float num = 12;
                for (int i = 0; i < num; i++)
                {
                    Vector2 vector = Vector2.UnitX * 0f;
                    vector += -Vector2.UnitY.RotatedBy(i * (6.28318548f / num), default(Vector2)) * new Vector2(1f, 4f);
                    vector = vector.RotatedBy(Projectile.velocity.ToRotation(), default(Vector2));
                    int a = Dust.NewDust(Projectile.Center, 0, 0, DustID.ShadowbeamStaff, 0f, 0f, 255, default(Color), 1f);
                    Main.dust[a].noGravity = true;
                    Main.dust[a].position = Projectile.Center - vector;
                    Main.dust[a].scale *= 1.3f;
                    Main.dust[a].velocity = Projectile.velocity * 0f + vector.SafeNormalize(Vector2.UnitY) * 2;
                }
            }
            existTimer++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos - Projectile.velocity * 5, null, Color.White, Projectile.rotation, Vector2.Zero, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 4; i < 10; i++)
            {
                float velX = Projectile.oldVelocity.X * (5f / i);
                float velY = Projectile.oldVelocity.Y * (5f / i);
                int a = Dust.NewDust(new Vector2(Projectile.Center.X - velX, Projectile.Center.Y - velY), 4, 4, DustID.PoisonStaff, Projectile.oldVelocity.X / 2, Projectile.oldVelocity.Y / 2, 0, default(Color), 1.8f);
                Main.dust[a].noGravity = true;
                Main.dust[a].velocity *= 0.5f;
                a = Dust.NewDust(new Vector2(Projectile.Center.X - velX, Projectile.Center.Y - velY), 4, 4, DustID.PoisonStaff, Projectile.oldVelocity.X / 2, Projectile.oldVelocity.Y / 2, 0, default(Color), 1.4f);
                Main.dust[a].velocity *= 0.05f;
                Main.dust[a].noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 4; i < 10; i++)
            {
                float velX = Projectile.oldVelocity.X * (5f / i);
                float velY = Projectile.oldVelocity.Y * (5f / i);
                int a = Dust.NewDust(new Vector2(Projectile.Center.X - velX, Projectile.Center.Y - velY), 4, 4, DustID.PoisonStaff, Projectile.oldVelocity.X / 2, Projectile.oldVelocity.Y / 2, 0, default(Color), 1.8f);
                Main.dust[a].noGravity = true;
                Main.dust[a].velocity *= 0.5f;
                a = Dust.NewDust(new Vector2(Projectile.Center.X - velX, Projectile.Center.Y - velY), 4, 4, DustID.PoisonStaff, Projectile.oldVelocity.X / 2, Projectile.oldVelocity.Y / 2, 0, default(Color), 1.4f);
                Main.dust[a].velocity *= 0.05f;
                Main.dust[a].noGravity = true;
            }
        }
    }
}
