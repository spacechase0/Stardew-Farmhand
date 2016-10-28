using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farmhand.UI.Pages.Parameters;
using Farmhand.UI.Pages.Properties;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Farmhand.UI.Pages.Controls
{
    public class LabelParameters : IPageParameters, IPositionable
    {
        [JsonProperty("drawShadow")]
        public bool DrawShadow { get; set; } = false;

        [JsonProperty("text")]
        public string Text { get; set; } = "";

        [JsonProperty("positioning")]
        public Vector2 Position { get; set; } = Vector2.Zero;

        [JsonProperty("positioning")]
        public Positioning Positioning { get; set; }

        [JsonProperty("alignment")]
        public Alignment PositionAlignment { get; set; }

        // Colour property is only respected when DrawShadow is enabled
        [JsonProperty("colour")]
        public Color Colour { get; set; }
    }
}
