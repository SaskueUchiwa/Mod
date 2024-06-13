using Mitochondria.Core.Framework.Modifiers;
using UnityEngine;
using AmongUs.GameOptions;
using System.Linq;
using Reactor.Utilities;
using Mitochondria.Core.Framework.Utilities.Extensions;

namespace SimpleMod;

public class MainGameplayModifier : GameplayModifier
{
    public override void OnRoleRevealed(IntroCutscene introCutscene)
    {
        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
            ModifierManager.Instance.Add(new InvisibleButtonModifier());
        }
    }

    public override void OnIntroCutsceneStarted(IntroCutscene introCutscene)
    {
        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
          introCutscene.RoleText.text = "Invisibler";
          introCutscene.RoleText.color = Palette.ImpostorRed;
          introCutscene.YouAreText.color = Palette.ImpostorRed;
          introCutscene.RoleBlurbText.text = "Can turn invisible for 10 seconds, doubling their speed during this time. Use invisibility to move undetected and gain an advantage over other players";
          PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
        }
    }

    public override void OnLobbyJoined()
    {
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(p => !p.IsHost()))
        player.RpcSendChat($"Welcome To Simple Mod Version {SimpleMod.VersionString}, Enjoy Playing New Roles & GameModes and Much More Is Coming This Year !");
        PluginSingleton<SimpleMod>.Instance.Log.LogMessage("Sending Custom Info On Player Joins");
    }
    public static AudioClip GetIntroSound(RoleTypes roleTypes)
    {
        return RoleManager.Instance.AllRoles.Where((role) => role.Role == roleTypes).FirstOrDefault().IntroSound;
    } 
}