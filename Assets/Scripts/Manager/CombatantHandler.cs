// Contain a Script to handle the combatant cooldowns and the combatant turns
using UnityEngine;

public class CombatantHandler
{
    public int id;
    public int side;
    public int x;
    public int y;
    public Combatant combatant;
    public int currentCooldown;
    public int health;

    public CombatantHandler(int id, int side, int x, int y, Combatant combatant, int currentCooldown, int health)
    {
        this.id = id;
        this.side = side;
        this.x = x;
        this.y = y;
        this.combatant = combatant;
        this.currentCooldown = currentCooldown;
        this.health = health;
    }
}