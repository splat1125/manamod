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
    public class SpaceGunBeam : ModProjectile
    {
        public override string Texture => "manamod/invisibleTexture";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vaporizing Plasma Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 20;
            Projectile.ignoreWater = true;
        }

        public int existTimer = 0;
        public int splitTimer = 0;
        public int split = 1;

        public override void AI()
        {
            /* Screenshake for if it's meant to be localized in MP instead of just done for the player firing the weapon
            if (existTimer == 0)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active)
                    {
                        Rectangle screen = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
                        if (screen.Intersects(Projectile.Hitbox))
                        {
                            Main.player[i].GetModPlayer<MPlayer>().screenShake = 8;
                        }
                    }
                }
            }*/

            Lighting.AddLight(Projectile.Center, 0, 0.5f, 0);
            if (existTimer >= 3)
            {
                //wacky fancy dust circle stolen from vanilla
                if (existTimer == 4)
                {
                    DelegateMethods.v3_1 = new Vector3(0.6f, 1f, 1f) * 0.2f;
                    Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 10f, 8f, new Utils.TileActionAttempt(DelegateMethods.CastLightOpen));
                    int num = 12;
                    for (int i = 0; i < num; i++)
                    {
                        Vector2 vector = Vector2.UnitX * 0f;
                        vector += -Vector2.UnitY.RotatedBy(num * (6.28318548f / i), default) * new Vector2(1f, 4f);
                        vector = vector.RotatedBy(Projectile.velocity.ToRotation(), default);
                        int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.ShadowbeamStaff, 0f, 0f, 255, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].position = Projectile.Center - vector - Projectile.velocity;
                        Main.dust[dust].scale *= 1.3f;
                        Main.dust[dust].velocity = Projectile.velocity * 0f + vector.SafeNormalize(Vector2.UnitY) * 2;
                    }
                    for (int i = 0; i < 29; i++)
                    {
                        int a = Dust.NewDust(Projectile.Center, 1, 1, DustID.PoisonStaff, Projectile.velocity.X + Main.rand.NextFloat(-4, 4), Projectile.velocity.Y + Main.rand.NextFloat(-4, 4));
                        Main.dust[a].scale = 1.4f;
                        Main.dust[a].velocity *= 0.4f;
                        Main.dust[a].noGravity = true;
                    }

                }

                int b = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.PoisonStaff);
                Main.dust[b].scale = 1.7f;
                Main.dust[b].velocity *= 0.4f;
                Main.dust[b].noGravity = true;

                int c = Dust.NewDust(new Vector2(Projectile.position.X - Projectile.width / 2, Projectile.position.Y - Projectile.height / 2), Projectile.width * 2, Projectile.height * 2, DustID.PoisonStaff, Projectile.velocity.X, Projectile.velocity.Y);
                Main.dust[c].scale = 1.4f;
                Main.dust[c].velocity *= 0.4f;
                Main.dust[c].noGravity = true;
            }
            if (existTimer >= 5 && existTimer % 20 == 0 && Main.rand.NextBool(3))
            {
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity / 2 + new Vector2(Main.rand.NextFloat(-4, 4), Main.rand.Next(-4, 4)), ModContent.ProjectileType<SpaceGunSpark>(), Projectile.damage / 5, 0, Main.myPlayer);
            }
            if (existTimer >= 10 && existTimer % 5 == 0 && splitTimer == 0 && Main.rand.NextBool(2))
            {
                if (Main.rand.NextBool(2))
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
                    split = -1;
                }
                else
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);
                    split = 1;
                }
                splitTimer = 5;
            }
            if (splitTimer > 0)
                splitTimer--;
            if (splitTimer == 1)
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * split);
            existTimer++;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 1;
            return true;
        }

        public override void Kill(int timeLeft)
        {
            //self propulsion
            //for (int i = 0; i < Main.maxPlayers; i++)
            //{
            //    if (Main.player[i].active)
            //    {
            //        Player player = Main.player[i];
            //        if(player.Distance(Projectile.Center) < 80)
            //        {
            //            Vector2 dir = Vector2.Normalize(player.Center - Projectile.Center);
            //            dir *= (80 - player.Distance(Projectile.Center)) / 5;
            //            player.velocity = dir;
            //        }
            //    }
            //}
            for (int i = 0; i < 60; i++)
            {
                int d = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.PoisonStaff, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));
                Main.dust[d].scale = Main.rand.NextFloat(1.5f, 2.5f);
                Main.dust[d].noGravity = true;
            }
        }
    }
}
