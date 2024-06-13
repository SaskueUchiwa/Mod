using AmongUs.GameOptions;
using Il2CppSystem;
using Mitochondria.Core.Api.GUI.Hud.Buttons;
using Mitochondria.Core.Framework.Resources.Sprites;
using Mitochondria.Core.Rpcs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;

namespace SimpleMod;

public  class InvisibleButton : CustomActionButton
{
     public InvisibleButton(string title, SpriteProvider icon, string description = null, TextStyle titleStyle = null, int cooldownTime = 0, int maxUseTime = 0, int? uses = null) : base(title, icon, description, titleStyle, cooldownTime, maxUseTime, uses)
     {

     }

    public override void OnClick()
    {
         RpcSetInvisibleAndSpeed(PlayerControl.LocalPlayer);
         if (SimpleMod.EnableAlphaBody.Value)
         {
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Alpha", 1f);
         }
    }
    public override bool CouldUse()
    {
        return State.Uses.HasUsesRemaining;
    }

    public override bool CanUse()
    {
      return PlayerControl.LocalPlayer != null;
    }

    [MethodRpc((uint)CustomRpc.SetInvisibleAndSpeed)]
    public static void RpcSetInvisibleAndSpeed(PlayerControl player)
    {
       InvisibleManager.SetVisible(player, false, null);
       SpeedManager.SetSpeedModifier(player.MyPhysics, 16f, null);
       Logger<SimpleMod>.Instance.LogMessage($"{player.Data.PlayerName} send set invisible and speed rpc {player.PlayerId}, Player Now Is {player.Visible}, and His Speed Now : {player.MyPhysics.Speed}");
    }

    public enum CustomRpc : uint
    {
       SetInvisibleAndSpeed = 240
    }
}