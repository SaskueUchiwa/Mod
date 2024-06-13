using System;
using Mitochondria.Core.Framework.Modifiers;
using Mitochondria.Core.Framework.Resources.Sprites;
using Mitochondria.Core.Framework.GUI.Hud;

namespace SimpleMod;


  public class InvisibleButtonModifier : GameplayModifier
  {
    private InvisibleButton __invisibleButton;

    public override void OnIntroCutsceneEnding()
    {
        SpriteProvider sp = new EmbeddedSpriteProvider("Invisible.png");
        __invisibleButton = new InvisibleButton("Convert", sp, uses: 1);
        
        CustomHudManager.Instance.MainActionButtonsContainer.Add(__invisibleButton);
    }

    public override void OnDisconnect() => OnGameEndedOrDisconnected();

    public override void OnGameEnded(GameManager gameManager) => OnGameEndedOrDisconnected();

    public override void Dispose()
    {
        if (__invisibleButton != null)
        {
            CustomHudManager.Instance.MainActionButtonsContainer.Remove(__invisibleButton);
        }
        GC.SuppressFinalize(this);
    }

    private void OnGameEndedOrDisconnected() => ModifierManager.Instance.Remove(this);
}