using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Farmhand.Helpers.JsonConverters
{
    public class RectangleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rectangle = (Rectangle)value;

            var x = rectangle.X;
            var y = rectangle.Y;
            var width = rectangle.Width;
            var height = rectangle.Height;

            var o = JObject.FromObject(new { x, y, width, height });

            o.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var o = JObject.Load(reader);

            var x = GetTokenValue(o, "X") ?? 0;
            var y = GetTokenValue(o, "X") ?? 0;
            var width = GetTokenValue(o, "Width") ?? 0;
            var height = GetTokenValue(o, "Height") ?? 0;

            return new Rectangle(x, y, width, height);
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        private static int? GetTokenValue(JObject o, string tokenName)
        {
            var val = o.GetValue(tokenName);
            var retInt = 0;
            if (int.TryParse(val.ToString(), out retInt))
            {
                return retInt;
            }
            else
            {
                return null;
            }

        }
    }
}
