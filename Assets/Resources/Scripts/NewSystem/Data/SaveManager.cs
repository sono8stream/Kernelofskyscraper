using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager
{
    public static bool Save<T>(string prefKey, T serializableObject)
    {
        MemoryStream memoryStream = new MemoryStream();
#if UNITY_IPHONE || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(memoryStream, serializableObject);

        string tmp = System.Convert.ToBase64String(memoryStream.ToArray());
        try
        {
            PlayerPrefs.SetString(prefKey, tmp);
        }
        catch (PlayerPrefsException)
        {
            return false;
        }
        return true;
    }

    public static T Load<T>(string prefKey)
    {
        if (!PlayerPrefs.HasKey(prefKey)) return default(T);
#if UNITY_IPHONE || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        BinaryFormatter bf = new BinaryFormatter();
        string serializedData = PlayerPrefs.GetString(prefKey);

        MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serializedData));
        T deserializedObject = (T)bf.Deserialize(dataStream);

        return deserializedObject;
    }

    public void save(UserData data)
    {
        // 保存用クラスにデータを格納.
        Save("data.dat", data);
        PlayerPrefs.Save();
    }

    public UserData load()
    {
        UserData data_tmp = Load<UserData>("data.dat");
        if (data_tmp != null)
        {
            return data_tmp;
        }
        else
        {
            return null;
        }
    }
}
