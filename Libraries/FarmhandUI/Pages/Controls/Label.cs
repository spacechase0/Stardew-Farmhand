using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farmhand.UI.Pages.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace Farmhand.UI.Pages.Controls
{
    public class Label : IPageControl
    {
        public LabelParameters Parameters { get; set; }
        public IEnumerable<IComponent> Children { get; }

        public string Type => nameof(Label);

        public Label(LabelParameters @params)
        {
            Parameters = @params;
        }

        public void AddChild(IComponent child)
        {
            var children = Children as List<IComponent>;
            children?.Add(child);
        }

        public void RemoveChild(IComponent child)
        {
            var children = Children as List<IComponent>;
            children?.Remove(child);
        }

        public void Draw(SpriteBatch sb)
        {
            if (Parameters.DrawShadow)
            {
                Utility.drawTextWithShadow(sb, Parameters.Text, Game1.dialogueFont, 
                    Parameters.Position, Parameters.Colour);
            }
            else
            {
                SpriteText.drawString(sb, Parameters.Text, (int)Parameters.Position.X, (int)Parameters.Position.Y);
            }

            Page.DrawHierarchy(this, sb);
        }
    }

}
