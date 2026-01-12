using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class HighscoreEntry {
    public string username;
    public int score;
    public int wave;
    public int level;
    public string timestamp;
}
[Serializable]
public class HighscoreList {
    public List<HighscoreEntry> entries = new();
}
[Serializable]
internal class HighscoreWrapper {
    public HighscoreList list = new();
}
public static class HighscoreStorage {
    private const string FileName = "highscores.json";
    private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);
    public static HighscoreList Load() {
        try {
            if (!File.Exists(FilePath)) return new HighscoreList();
            string json = File.ReadAllText(FilePath);
            var wrapper = JsonUtility.FromJson<HighscoreWrapper>(json);
            return wrapper != null && wrapper.list != null ? wrapper.list : new HighscoreList();
        } catch (Exception ex) {
            Debug.LogError($"HighscoreStorage.Load failed: {ex.Message}");
            return new HighscoreList();
        }
    }

    public static void Save(HighscoreList list) {
        try {
            var wrapper = new HighscoreWrapper { list = list ?? new HighscoreList() };
            string json = JsonUtility.ToJson(wrapper, prettyPrint: true);
            File.WriteAllText(FilePath, json);
        } catch (Exception ex) {
            Debug.LogError($"HighscoreStorage.Save failed: {ex.Message}");
        }
    }
}
