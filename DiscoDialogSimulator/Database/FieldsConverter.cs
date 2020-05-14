using DiscoDialogSimulator.Database.Assets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.Database
{
    public class FieldsConverter : JsonConverter<Fields>
    {
        public override Fields ReadJson(JsonReader reader, Type objectType, Fields existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var ret = hasExistingValue && existingValue != null ? existingValue : new Fields();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                    break;

                if (reader.TokenType != JsonToken.StartObject)
                    throw new JsonSerializationException($"Unexpected token {reader.TokenType}");

                string key = null, value = null;

                reader.Read();
                while (reader.TokenType != JsonToken.EndObject)
                {
                    string propertyName = reader.Value as string;
                    if (propertyName == "title")
                        key = reader.ReadAsString();
                    else if (propertyName == "value")
                        value = reader.ReadAsString();
                    else
                        reader.Skip();
                    reader.Read();
                }

                ret.Add(key, value);
            }

            return ret;
        }

        public override void WriteJson(JsonWriter writer, Fields value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
