using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ConsoleRPG.Services
{
    public static class JsonSerializingService<T>
    {
        public static void Save(T objectToSave,string fileName,Action actionBeforeSave = null)
        {
            actionBeforeSave?.Invoke();
            using (var stream = File.OpenWrite(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(new BsonWriter(stream), objectToSave);
            }
        }

        public static void SaveEntryInDictionary<TDict>(TDict list, string fileName) where TDict : IDictionary
        {
            using (var stream = File.OpenWrite(fileName))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(new BsonWriter(stream), list);
            }
        }


        public static T Load(string fileName)
        {
            try
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var serializer = new JsonSerializer();
                    var loadedObject = serializer.Deserialize<T>(new BsonReader(stream));
                    return loadedObject;
                }
            }
            catch (Exception e)
            {
                return default;
            }
        }

        public static void ClearSave(string fileName)
        {
            File.WriteAllText(fileName, string.Empty);
        }
    }
}
