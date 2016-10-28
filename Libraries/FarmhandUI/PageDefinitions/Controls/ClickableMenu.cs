using System;
using System.Collections.Generic;
using Farmhand.UI.Pages;
using Farmhand.UI.Pages.Components;
using Farmhand.UI.Pages.Controls;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace Farmhand.UI.PageDefinitions.Controls
{
    class ClickableMenu : IClickableMenu, IPageComponent
    {
        private List<IPageControl> _children { get; set; } = new List<IPageControl>();
        
        public IEnumerable<IComponent> Children => _children;

        private ClickableMenuParameters Params { get; set; }

        public ClickableMenu(ClickableMenuParameters @params)
            : base(@params.X,
                  @params.Y,
                  @params.Width >= 0 ? @params.Width : Game1.viewport.Width - @params.X,
                  @params.Height >= 0 ? @params.Height : Game1.viewport.Height - @params.Y,
                  false)
        {
            Params = @params;
        }

        public Type GetParamsType()
        {
            return typeof(ClickableMenuParameters);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            
        }
        
        public void AddChild(IComponent child)
        {
            var control = child as IPageControl;
            if (control != null)
            {
                _children.Add(control);
            }
            else
            {
                throw new Exception("ClickableMenu can only accept controls inheriting from IPageControl as children");
            }
        }

        public void RemoveChild(IComponent child)
        {
            var control = child as IPageControl;
            if (control != null)
            {
                _children.Remove(control);
            }
        }

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            Draw(sb);
        }

        public void Draw(SpriteBatch sb)
        {
            Page.DrawHierarchy(this, sb);
        }

        
    }
}
