using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farmhand.UI.Pages.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace Farmhand.UI.Pages
{
    public interface IComponent
    {
        IEnumerable<IComponent> Children { get; }

        void AddChild(IComponent child);

        void RemoveChild(IComponent child);

        void Draw(SpriteBatch sb);
    }
}
