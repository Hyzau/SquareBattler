using UnityEngine;

public abstract class Combatant 
{
    protected int side; // Side of the combatant, 0 for player, 1 for enemy, 2 for neutral 
    protected int health; // Health of the combatant
    protected int position_x; // Position of the combatant
    protected int position_y; // Position of the combatant
    protected int gridWidth; // Width of the grid
    protected int gridHeight; // Height of the grid

    public Combatant(int side, int x, int y, int health, int gridWidth, int gridHeight)
    {
        this.side = side;
        this.position_x = x;
        this.position_y = y;
        this.health = health;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;
    }

    public int GetSide()
    {
        return this.side;
    }

    public int GetPositionX()
    {
        return this.position_x;
    }

    public int GetPositionY()
    {
        return this.position_y;
    }
    
    public void TakeDamage(int damage)
    {
        this.health -= damage;
        if (this.health <= 0)
        {
            Die();
        }
    }

    public virtual ICommand TakeTurn(int[,] map)
    {
        return null;
    }

    public void Die()
    {
        Debug.Log($"{this} died.");
    }

}
