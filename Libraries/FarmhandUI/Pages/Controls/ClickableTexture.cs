using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farmhand.UI.Pages.Parameters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley.Menus;

namespace Farmhand.UI.Pages.Controls
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
