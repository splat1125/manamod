using Microsoft.Xna.Framework;
using manamod.Projectiles.Magic;
using manamod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;

namespace manamod.Items.Magic
{
    public class WandOfSparkingOverride : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.WandofSparking;
        public override void SetStaticDefaults()
        {
            Item.staff[ItemID.WandofSparking] = true;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

            if(line != null)
                line.Text = "Conjures a mighty spark that is weak to water";
        }
        public override void SetDefaults(Item item)
        {
            item.damage = 14;
            item.knockBack = 4f;
            item.useStyle = ItemUseStyleID.Shoot;
            item.useAnimation = 24;
            item.useTime = 24;

            item.mana = 4;
            item.DamageType = DamageClass.Magic;
            item.width = 26;
            item.height = 28;

            item.value = 100;
            item.rare = ItemRarityID.Blue;

            item.shoot = ModContent.ProjectileType<SparkProj_Base>();
            item.shootSpeed = 6.5f;
            item.autoReuse = true;
            item.noMelee = true;

            item.StatsModifiedBy.Add(Mod);
        }
        public enum castType
        {
            High,
            Med,
            Low
        }
        public bool doCastFX = false;
        public castType casting = castType.Med;  //default
       public override void UpdateInventory(Item item, Player player)
        {
            switch (player.statMana)
            {
                case >= 85:
                    casting = castType.High;
                    break;

                case <= 25:
                    casting = castType.Low;
                    break;

                default:
                    casting = castType.Med;
                    break;
            }
        }
        public override GlobalItem Clone(Item from, Item to)
        {
            to.GetGlobalItem<WandOfSparkingOverride>().casting = from.GetGlobalItem<WandOfSparkingOverride>().casting;

            return to.GetGlobalItem<WandOfSparkingOverride>();
        }
        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            if (casting == castType.High)   //2x mana cost
                reduce -= -1f;
            else if (casting == castType.Low)    //mana cost of 1
                reduce -= 0.75f;
            else
                reduce -= 0;
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            switch (casting)
            {
                case castType.High: //if we cast the big bolt, take longer
                    return 0.5f;                
                case castType.Low: //if we cast the fizzle, fire 1.5x as fast!
                    return 2.5f;
                default:            //otherwise, our speed is default
                    return 1;
            }
        }
        public override void ModifyShootStats(Item item,Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (casting == castType.High)   //big chunky noita zap
            {
                type = ModContent.ProjectileType<SparkProj_Bolt>();
                damage = 28;
                velocity *= 0.5f;
                doCastFX = true;
            }
            else if (casting == castType.Med)    //default, do nothing
            {
                type = ModContent.ProjectileType<SparkProj_Base>();
                doCastFX = false;
            }
            else if (casting == castType.Low)    //fizzle
            {
                type = ModContent.ProjectileType<SparkProj_Fizzle>();
                damage = 5;
                knockback = 6f;
                doCastFX = false;
            }
        }
        public override bool Shoot(Item item,Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (casting == castType.High)   //big chunky noita zap
            {
                SoundStyle sound = new("manamod/Sounds/noita_lightning_hit") {Volume = 0.6f};
                SoundEngine.PlaySound(sound);
                player.GetModPlayer<ManaModPlayer>().ShakePower = 3.5f;
                player.velocity -= velocity;
                return true;
            }
            else if (casting == castType.Med)    //default, do nothing
            {
                SoundStyle sound = new("manamod/Sounds/noita_spell_bolt") { Volume = 0.4f };
                SoundEngine.PlaySound(sound);
                return true;
            }
            else if (casting == castType.Low)    //fizzle
            {
                SoundStyle sound = new("manamod/Sounds/noita_electric_spark") { Volume = 0.1f };
                SoundEngine.PlaySound(sound);
                return true;
            }
            else return true;
        }
        public override void UseItemFrame(Item item, Player player)
        {
            if (doCastFX && player.itemAnimation < 45)
            {
                int sparks = Dust.NewDust(player.itemLocation, 1, 1, 6, 0f, 0f, 150);
                Main.dust[sparks].noGravity = true;
                Main.dust[sparks].velocity = Vector2.UnitY * Main.rand.NextFloat(-2f);
                Main.dust[sparks].scale = 0.75f;
                Main.dust[sparks].position = Main.dust[sparks].position.MoveTowards(player.itemLocation + (Vector2.UnitX.RotatedBy(player.itemRotation) * Main.rand.NextFloat(20, 36) * player.direction), 100);
             }
        }
    }
}