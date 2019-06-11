using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lanymy.Common.Converters
{
    public class JsonIgnoreCaseStringEnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}
