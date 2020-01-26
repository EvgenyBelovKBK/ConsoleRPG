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
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto ,ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
        public static void Save(T objectToSave,string fileName,Action actionBeforeSave = null)
        {
            actionBeforeSave?.Invoke();
            var json = JsonConvert.SerializeObject(objectToSave, JsonSettings);
                File.WriteAllText(fileName, json);
        }

        public static void SaveEntryInDictionary<TDict>(TDict list, string fileName) where TDict : IDictionary
        {
            var json = JsonConvert.SerializeObject(list, JsonSettings);
            File.WriteAllText(fileName,json);
        }


        public static T Load(string fileName)
        {
            try
            {
                var jsonObject = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName), JsonSettings);
                return jsonObject;
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
