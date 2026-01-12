using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Stamina))]
public class PlayerUpgrades : MonoBehaviour {
    private PlayerController player;
    private PlayerCombat combat;
    private Health health;
    private Stamina stamina;
    private void Awake() {
        player = GetComponent<PlayerController>();
        combat = GetComponent<PlayerCombat>();
        health = GetComponent<Health>();
        stamina = GetComponent<Stamina>();
    }
    public void AddHealthFlat(float amount) { if (health == null) return; if (amount == 0f) return; health.SetMaxHealth(health.Max + amount, refill: true); }
    public void AddHealthPercent(float percent) { if (health == null) return; if (percent == 0f) return; health.SetMaxHealth(health.Max * (1f + percent), refill: true); }
    public void AddStaminaFlat(float amount) { if (stamina == null) return; if (amount == 0f) return; stamina.SetMaxStamina(stamina.Max + amount, refill: true); }
    public void AddStaminaPercent(float percent) { if (stamina == null) return; if (percent == 0f) return; stamina.SetMaxStamina(stamina.Max * (1f + percent), refill: true); }
    public void AddMoveSpeedFlat(float delta) { if (player == null) return; if (delta == 0f) return; player.AddMoveSpeed(delta); }
    public void AddMoveSpeedPercent(float percent) { if (player == null) return; if (percent == 0f) return; player.AddMoveSpeedPercent(percent); }
    public void AddDamageFlat(float delta) { if (combat == null) return; if (delta == 0f) return; combat.AddDamage(delta); }
    public void AddDamagePercent(float percent) { if (combat == null) return; if (percent == 0f) return; combat.AddDamagePercent(percent); }
    public void ModifyAttackCooldownPercent(float deltaPercent) { if (combat == null) return; if (deltaPercent == 0f) return; combat.ModifyAttackCooldownPercent(deltaPercent); }
}
