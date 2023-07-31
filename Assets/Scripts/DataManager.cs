using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

/// <summary>
/// This class is a singleton manager that 
/// </summary>
public class DataManager : Singleton<DataManager>
{
    private Dictionary<string, StatusEffectData> statusEffects = new Dictionary<string, StatusEffectData>();
    private Dictionary<string, UnitData> units = new Dictionary<string, UnitData>();
    private string unitDataPath = "Assets/Data/UnitData";
    private string statusEffectDataPath = "Assets/Data/StatusEffectData";

    private void Start()
    {
        LoadUnitData();
        LoadStatusEffectData();
    }

    private void LoadUnitData()
    {
        string[] jsonFiles = Directory.GetFiles(unitDataPath, "*.json");

        foreach (string filePath in jsonFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string json = File.ReadAllText(filePath);
            UnitData data = JsonConvert.DeserializeObject<UnitData>(json);

            units.Add(data.name, data);
        }
    }

    private void LoadStatusEffectData()
    {
        string[] jsonFiles = Directory.GetFiles(statusEffectDataPath, "*.json");

        foreach (string filePath in jsonFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string json = File.ReadAllText(filePath);
            StatusEffectData data = JsonConvert.DeserializeObject<StatusEffectData>(json);

            statusEffects.Add(data.name, data);
        }
    }

    public UnitData GetUnitData(string unitName)
    {
        return units[unitName];
    }

    public StatusEffectData GetStatusEffectData(string effectName)
    {
        return statusEffects[effectName];
    }
}
