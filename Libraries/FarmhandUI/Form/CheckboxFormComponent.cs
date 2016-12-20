﻿using Farmhand.UI.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Farmhand.UI.Form
{
    public class CheckboxFormComponent : BaseFormComponent
    {
        protected static readonly Rectangle SourceRectUnchecked = new Rectangle(227, 425, 9, 9);
        protected static readonly Rectangle SourceRectChecked = new Rectangle(236, 425, 9, 9);

        public event ValueChanged<bool> Handler;
        public bool Value { get; set; }

        protected string Label;
        public CheckboxFormComponent(Point offset, string label, ValueChanged<bool> handler=null)
        {
            SetScaledArea(new Rectangle(offset.X, offset.Y, 9 + GetStringWidth(label, Game1.smallFont), 9));
            Value = false;
            Label = label;
            if(handler!=null)
                Handler += handler;
        }
        public override void LeftClick(Point p, Point o)
        {
            if (Disabled)
                return;
            Game1.playSound("drumkit6");
            Value = !Value;
            Handler?.Invoke(this, Parent, Parent.GetAttachedMenu(), Value);
        }
        public override void Draw(SpriteBatch b, Point o)
        {
            if (!Visible)
                return;
            b.Draw(Game1.mouseCursors, new Vector2(o.X + Area.X, o.Y + Area.Y), Value ? SourceRectChecked : SourceRectUnchecked, Color.White * (Disabled ? 0.33f : 1f), 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.4f);
            Utility.drawTextWithShadow(b, Label, Game1.smallFont, new Vector2(o.X + Area.X + Zoom10, o.Y + Area.Y+Zoom2), Game1.textColor * (Disabled ? 0.33f : 1f), 1f, 0.1f);
        }
    }
}
