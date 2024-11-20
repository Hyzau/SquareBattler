using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoNothing : Combatant
{
    public EnemyDoNothing(int side, int x, int y, int health, int gridWidth, int gridHeight) : base(side, x, y, health, gridWidth, gridHeight)
    {
    }

    public override ICommand TakeTurn(int[,] map)
    {
        return null; // Do nothing
    }
}
