using System.Collections.Generic;
using Farmhand.UI.Pages;
using Farmhand.UI.Pages.Controls;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Farmhand.UI.PageDefinitions.Controls
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
