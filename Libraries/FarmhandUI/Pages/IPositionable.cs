using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farmhand.UI.Pages.Properties;
using Microsoft.Xna.Framework;

namespace Farmhand.UI.Pages
{
    public interface IPositionable
    {
        Positioning Positioning { get; set; }

        Alignment PositionAlignment { get; set; }

        Vector2 Position { get; set; }
    }
}
