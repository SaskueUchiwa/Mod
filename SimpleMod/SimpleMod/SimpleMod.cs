using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Mitochondria.Core;
using Mitochondria.Options.Settings.Framework;
using Mitochondria.Core.Framework.Options;
using UnityEngine;
using System.Reflection;
using Reactor.Utilities.Extensions;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;

namespace SimpleMod;

[BepInAutoPlugin]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MitochondriaPlugin.Id)]
[BepInDependency(Mitochondria.Options.Settings.MitochondriaSettingsOptionsPlugin.Id)]
[ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
public partial class SimpleMod : BasePlugin
{
        public const string VersionString = "1.0.0";
       // public static System.Version Version = System.Version.Parse(VersionString);

       [CustomSettingsOption(AmongUs.GameOptions.GameModes.Normal, 9)]
       public static CustomNumberOption<SimpleMod> InvisibleDuration;
       public static CustomNumberOption<SimpleMod> InvisibleCooldown;
       public static CustomToggleOption<SimpleMod> EnableAlphaBody;
       public static Sprite LogoSprite;

       private static DLoadImage _iCallLoadImage;

       public Harmony Harmony { get; } = new(Id);

    static SimpleMod()
    {
        InvisibleDuration = new CustomNumberOption<SimpleMod>("Invisible Duration", "Invisible Durations", 15f, new FloatRange(1f, 15f), 1f, null, true, true);
        InvisibleCooldown = new CustomNumberOption<SimpleMod>("Invisible Cooldown", "Invisible Cooldowns", 30f, new FloatRange(0f, 30f), 1f, null, true, true);
        EnableAlphaBody = new CustomToggleOption<SimpleMod>("AKA Phantom Body", "Enable Phantom Invisible", false, true);
    }

     [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
     public static void Postfix(VersionShower __instance)
     {
            var simpleLogo = new GameObject("bannerLogo_SimpleMod");
            simpleLogo.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            var renderer = simpleLogo.AddComponent<SpriteRenderer>();
            renderer.sprite = LogoSprite;

            var position = simpleLogo.AddComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(-0.2f, 1.5f, 8f);
            position.Alignment = AspectPosition.EdgeAlignments.Top;

            position.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) =>
            {
                position.AdjustPosition();
            })));


            var scaler = simpleLogo.AddComponent<AspectScaledAsset>();
            var renderers = new Il2CppSystem.Collections.Generic.List<SpriteRenderer>();
            renderers.Add(renderer);

            scaler.spritesToScale = renderers;
            scaler.aspectPosition = position;

            simpleLogo.transform.SetParent(GameObject.Find("RightPanel").transform);
     }

     [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
     [HarmonyPriority(Priority.VeryHigh)]
     public static class VersionShowerUpdate
     {
      public static void Postfix(VersionShower __instance)
        {
            var text = __instance.text;
            text.text += " - <color=#ff0000>SimpleMod v" + VersionString + "</color>" + "By GameTechGuides";
            text.transform.localPosition += new Vector3(-0.8f, -0.16f, 0f);

            if (GameObject.Find("RightPanel"))
            {
                text.transform.SetParent(GameObject.Find("RightPanel").transform);

                var aspect = text.gameObject.AddComponent<AspectPosition>();
                aspect.Alignment = AspectPosition.EdgeAlignments.Top;
                aspect.DistanceFromEdge = new Vector3(-0.2f, 2.5f, 8f);

                aspect.StartCoroutine(Effects.Lerp(0.1f, new System.Action<float>((p) =>
                {
                    aspect.AdjustPosition();
                })));

                return;
                   }
                }
        }

    public override void Load()
    {     
        Harmony.PatchAll();
        LogoSprite = CreateSprite("SimpleMod.Resources.AvalonUs.png");
        Logger<SimpleMod>.Instance.LogMessage("Simple Mod Loaded !");
    }


     public static Sprite CreateSprite(string name)
        {
            var pixelsPerUnit = 100f;
            var pivot = new Vector2(0.5f, 0.5f);

            var assembly = Assembly.GetExecutingAssembly();
            var tex = CanvasUtilities.CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
}
