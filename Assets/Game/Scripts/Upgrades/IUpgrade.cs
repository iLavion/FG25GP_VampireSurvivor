public interface IUpgrade {
    string Id { get; }
    string DisplayName { get; }
    string Description { get; }
    void Apply(UpgradeContext ctx);
}

public class UpgradeContext {
    public PlayerController Player;
    public PlayerCombat Combat;
    public Health Health;
    public ExperienceSystem XP;
    public PlayerUpgrades Upgrades;
}
