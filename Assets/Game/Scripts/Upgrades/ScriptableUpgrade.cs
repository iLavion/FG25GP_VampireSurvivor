using UnityEngine;

[CreateAssetMenu(menuName = "VS/Upgrade", fileName = "Upgrade_", order = 0)]
public class ScriptableUpgrade : ScriptableObject, IUpgrade {
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [Header("Rarity")]
    [SerializeField] private UpgradeRarity rarity = UpgradeRarity.Common;
    [SerializeField, Range(1, 100)] private int weight = 50;
    [Header("Effects")]
    [SerializeField] private float addMaxHealth;
    [SerializeField] private float addMaxHealthPercent;
    [SerializeField] private float addDamage;
    [SerializeField] private float addDamagePercent;
    [SerializeField] private float addMoveSpeed;
    [SerializeField] private float addMoveSpeedPercent;
    [SerializeField] private float attackCooldownPercent;
    public string Id => id;
    public string DisplayName => displayName;
    public string Description => description;
    public UpgradeRarity Rarity => rarity;
    public int Weight => weight;
    public void Apply(UpgradeContext ctx) {
        if (ctx.Upgrades == null) return;
        ctx.Upgrades.AddHealthFlat(addMaxHealth);
        ctx.Upgrades.AddHealthPercent(addMaxHealthPercent);
        ctx.Upgrades.AddMoveSpeedFlat(addMoveSpeed);
        ctx.Upgrades.AddMoveSpeedPercent(addMoveSpeedPercent);
        ctx.Upgrades.AddDamageFlat(addDamage);
        ctx.Upgrades.AddDamagePercent(addDamagePercent);
        ctx.Upgrades.ModifyAttackCooldownPercent(attackCooldownPercent);
    }
}
