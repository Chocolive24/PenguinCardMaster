using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataSave : MonoBehaviour
{

    [SerializeField] private string _fileName; 
    [SerializeField] private List<ScriptableObject> _datas; 
    
    // Getters and Setters ---------------------------------------------------------------------------------------------
    public List<ScriptableObject> Datas => _datas;

    // Methods ---------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void OnEnable()
    {
        foreach (ScriptableObject data in _datas)
        {
            string fileName = Application.dataPath + string.Format("/{0}_{1}.pso", _fileName, data.name);

            if (File.Exists(fileName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(fileName, FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), data);
                file.Close();
            }
            else
            {
                Debug.LogWarning("No persistent previous datas");
            }
        }
    }

    // Update is called once per frame
    void OnDisable()
    {
        foreach (ScriptableObject data in _datas)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + string.Format("/{0}_{1}.pso", _fileName, data.name), FileMode.OpenOrCreate);
            var json = JsonUtility.ToJson(data);
            bf.Serialize(file, json);
            file.Close();
        }
    }
}
