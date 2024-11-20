using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRandomMoveAndWhirl : Combatant
{
    public bool hasMoved = false;
    public EnemyRandomMoveAndWhirl(int side, int x, int y, int health, int gridWidth, int gridHeight) : base(side, x, y, health, gridWidth, gridHeight)
    {
    }

    public override ICommand TakeTurn(int[,] map)
    {
        if (!this.hasMoved)
        {
            this.hasMoved = true;
            return MoveRandom();
        }
        this.hasMoved = false;
        return Attack();
    }

    ICommand MoveRandom()
    {
        int direction = Random.Range(0, 4);
        int x = 0;
        int y = 0;
        switch (direction)
        {
            case 0:
                x = 1;
                break;
            case 1:
                x = -1;
                break;
            case 2:
                y = 1;
                break;
            case 3:
                y = -1;
                break;
        }
        return new MoveCommand(this, x, y);
    }

    ICommand Attack()
    {
        return new WhirlwindCommand(this, 0, 0);
    } 
}
