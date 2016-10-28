using System.Collections.Generic;
using Farmhand.UI.Pages;
using Farmhand.UI.Pages.Controls;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Farmhand.UI.PageDefinitions.Controls
{
    public class ClickableTexture : ClickableTextureComponent, IPageControl
    {
        public IEnumerable<IComponent> Children { get; } = new List<IComponent>();

        public string Type => nameof(ClickableTexture);

        public ClickableTexture(ClickableTextureParameters @params) 
            : base(@params.Name, @params.Bounds, @params.Label ?? "", @params.HoverText ?? "",
                  @params.Texture, @params.TextureSourceRectangle, @params.Scale, @params.DrawShadow)
        {
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
            base.draw(sb);

            Page.DrawHierarchy(this, sb);
        }
    }
    
}
