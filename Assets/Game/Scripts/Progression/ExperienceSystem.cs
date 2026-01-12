using UnityEngine;
using UnityEngine.Events;

public class ExperienceSystem : MonoBehaviour {
    [SerializeField] private int level = 1;
    [SerializeField] private int currentXP = 0;
    private int totalXP = 0;
    private int xpToNext;
    private float growth;
    public UnityEvent<int, int, int> onXPChanged;
    public UnityEvent<int> onLevelUp;
    private void Start() {
        if (GameManager.Instance != null) {
            xpToNext = GameManager.Instance.BaseXpToLevel;
            growth = GameManager.Instance.XpLevelGrowth;
        }
        else {
            xpToNext = 10;
            growth = 1.5f;
        }
        for (int i = 1; i < level; i++) xpToNext = Mathf.RoundToInt(xpToNext * growth);
        onXPChanged?.Invoke(level, currentXP, xpToNext);
    }
    public void AddXP(int amount) {
        int gained = Mathf.Max(0, amount);
        currentXP += gained;
        totalXP += gained;
        while (currentXP >= xpToNext) {
            currentXP -= xpToNext;
            level++;
            xpToNext = Mathf.RoundToInt(xpToNext * growth);
            onLevelUp?.Invoke(level);
        }
        onXPChanged?.Invoke(level, currentXP, xpToNext);
    }
    public int Level => level;
    public int CurrentXP => currentXP;
    public int TotalXP => totalXP;
    public int XPToNext => xpToNext;
}
