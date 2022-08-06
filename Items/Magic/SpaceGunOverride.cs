using manamod.Projectiles.Magic.SpaceGun;
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

namespace manamod.Items.Magic
{
    public class SpaceGunOverride : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.SpaceGun;
        public override void SetDefaults(Item item)
        {
            item.StatsModifiedBy.Add(Mod);
            item.UseSound = null;
        }
        public enum castType
        {
            High,
            Med,
            Low
        }
        public castType casting = castType.Med;
        public override GlobalItem Clone(Item from, Item to)
        {
            to.GetGlobalItem<SpaceGunOverride>().casting = from.GetGlobalItem<SpaceGunOverride>().casting;

            return to.GetGlobalItem<SpaceGunOverride>();
        }
        public override void UpdateInventory(Item item, Player player)
        {
            casting = player.statMana switch
            {
                > 90 => castType.High,
                < 20 => castType.Low,
                _ => castType.Med,
            };
        }
        public override float UseSpeedMultiplier(Item item, Player player)
        {
            return casting switch
            {
                castType.High => 0.5f,
                castType.Low => 1f,
                _ => 1.1f,
            };
        }
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            item.mana = casting switch
            {
                castType.High => 12,
                castType.Low => 2,
                _ => 6,
            };
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (casting)
            {
                case castType.High:
                    player.velocity -= new Vector2(velocity.X, velocity.Y) / 4;
                    player.GetModPlayer<ManaModPlayer>().ShakePower = 6f;
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SpaceGunBeam>(), damage * 4, knockback + 5, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.NPCDeath56, position);
                    break;

                case castType.Low:
                    for (int i = 0; i < 5; i++)
                    {
                        int b = Dust.NewDust(position - new Vector2(4, 4) + velocity * 2, 1, 1, DustID.ShadowbeamStaff, velocity.X + Main.rand.NextFloat(-4, 4), velocity.Y + Main.rand.NextFloat(-4, 4));
                        Main.dust[b].scale = 1.4f;
                        Main.dust[b].velocity *= 0.4f;
                        Main.dust[b].noGravity = true;
                    }
                    Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(10));
                    velocity.X = perturbedSpeed.X;
                    velocity.Y = perturbedSpeed.Y;
                    velocity += new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3));
                    Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<SpaceGunSpark>(), damage / 2, 0, player.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item13, position);
                    SoundEngine.PlaySound(SoundID.Item42, position);
                    break;

                default:
                    SoundEngine.PlaySound(SoundID.Item158, position);
                    Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<SpaceGunLaser>(), damage, knockback, player.whoAmI);
                    break;
            }
            return false;
        }
    }
}