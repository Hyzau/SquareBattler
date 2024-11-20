using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRandomMove : Combatant
{
    public EnemyRandomMove(int side, int x, int y, int health, int gridWidth, int gridHeight) : base(side, x, y, health, gridWidth, gridHeight)
    {
    }

    public override ICommand TakeTurn(int[,] map)
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
}
