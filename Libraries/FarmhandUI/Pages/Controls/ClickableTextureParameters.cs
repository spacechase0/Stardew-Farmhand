using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farmhand.UI.Pages.Parameters;
using Farmhand.UI.Pages.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Farmhand.UI.Pages.Controls
{
    public class ClickableTextureParameters : IPageParameters, IPositionable
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("bounds", Required = Required.Always)]
        [JsonConverter(typeof(Farmhand.Helpers.JsonConverters.RectangleConverter))]
        public Rectangle Bounds { get; set; }

        [JsonProperty("texture", Required = Required.Always)]
        public string TextureName { get; set; }

        [JsonProperty("textureSource", Required = Required.Always)]
        [JsonConverter(typeof(Farmhand.Helpers.JsonConverters.RectangleConverter))]
        public Rectangle TextureSourceRectangle { get; set; }

        [JsonProperty("position")]
        public Vector2 Position { get; set; } = Vector2.Zero;

        [JsonProperty("positioning")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Positioning Positioning { get; set; } = Positioning.Relative;

        [JsonProperty("alignment")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Alignment PositionAlignment { get; set; } = Alignment.Top | Alignment.Left;
        
        [JsonProperty("scale")]
        public float Scale { get; set; } = 1.0f;

        [JsonProperty("label")]
        public string Label { get; set; } = "";

        [JsonProperty("hoverText")]
        public string HoverText { get; set; } = "";

        [JsonProperty("drawShadow")]
        public bool DrawShadow { get; set; } = false;
        
        [JsonIgnore]
        public Texture2D Texture { get; } = null;

        
    }
}
