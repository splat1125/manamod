using Microsoft.Xna.Framework;
using manamod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace manamod.Utilities
{
	public class ManaModPlayer : ModPlayer
	{
        public float ShakePower;
        public override void ModifyScreenPosition()
        {
            if (ShakePower > 0f)
            {
                Main.screenPosition += Main.rand.NextVector2Circular(ShakePower, ShakePower);
                ShakePower = MathHelper.Clamp(ShakePower - 0.15f, 0f, 20f);
            }
        }
    }
}