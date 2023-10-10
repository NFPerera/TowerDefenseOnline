using System.Collections.Generic;
using _Main.Scripts.DevelopmentUtilities;
using _Main.Scripts.Networking;
using UnityEngine;

namespace _Main.Scripts
{
    public static class Serializador
    {
        public static string SerializeDic(Dictionary<ulong, MasterManager.RoomData> p)
        {
            var data = new DictionarySerializer();

            foreach (var obj in p)
            {
                data.keys.Add(obj.Key);
                data.values.Add(obj.Value);
            }

            return JsonUtility.ToJson(data, true);
            
        }


        public static void DeSerializeDic(string json, Dictionary<ulong, MasterManager.RoomData> dic)
        {
            var data = JsonUtility.FromJson<DictionarySerializer>(json);
            Debug.Log(json);
            Debug.Log($"Keys Count = {data.keys.Count} ");
            Debug.Log($"Data Count = {data.values.Count} ");
            for (int i = 0; i < data.keys.Count; i++)
            {
                dic[data.keys[i]] = data.values[i];
            }
        }
    }
}