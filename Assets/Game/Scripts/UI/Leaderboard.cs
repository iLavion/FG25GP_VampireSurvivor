using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour {
    [SerializeField] private Transform container;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private int maxEntries = 10;
    private void OnEnable() => Refresh();
    public void Refresh() {
        if (container == null || itemPrefab == null) return;
        foreach (Transform child in container) Destroy(child.gameObject);
        var list = HighscoreStorage.Load();
        int rank = 1;
        foreach (var entry in list.entries) {
            if (rank > maxEntries) break;
            var row = Instantiate(itemPrefab, container);
            var usernameText = row.transform.Find("Username")?.GetComponent<TMP_Text>();
            var scoreText = row.transform.Find("Experience")?.GetComponent<TMP_Text>();
            if (usernameText != null) usernameText.text = $"{rank}. {entry.username}";
            if (scoreText != null) scoreText.text = entry.score.ToString();
            rank++;
        }
    }
}
