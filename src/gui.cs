namespace Oxide.Plugins
{
    using UnityEngine;
    using static Oxide.Plugins.GUICreator;

    partial class RustPluginTemplate : RustPlugin
    {
        partial void initGUI()
        {
            guiCreator = (GUICreator)Manager.GetPlugin("GUICreator");
        }

        private GUICreator guiCreator;

        private void UIPositionList(BasePlayer player)
        {
            GuiContainer container = new GuiContainer(this, "positionList");
            container.addButton("close", new Rectangle(800, 80, 320, 50, 1920, 1080, true), GuiContainer.Layer.hud, new GuiColor("red"), text: new GuiText("close"));
            Rectangle backgroundRect = new Rectangle(800, 130, 320, 790, 1920, 1080, true);
            container.addPlainPanel("background", backgroundRect, GuiContainer.Layer.hud, new GuiColor(0,0,0,0.3f), blur: GuiContainer.Blur.medium);
            int i = 0;
            foreach(Vector3 pos in StoredData.getList())
            {
                if (i == 12) break;
                Rectangle entryRect = new Rectangle(10, 10 + i * 60, 300, 50, 320, 790, true);
                GuiText text = new GuiText($"X:{(int)pos.x}, Y:{(int)pos.y}, Z:{(int)pos.z}", color: new GuiColor("white"));
                container.addPanel($"entry_{i}", entryRect, "background", new GuiColor(0, 0, 0, 0.5f), text: text);
                i++;
            }
            container.display(player);

        }
    }
}