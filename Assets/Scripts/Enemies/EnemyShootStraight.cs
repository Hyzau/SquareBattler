using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemyShootStraight : Combatant
{
    public int direction_x = 1;
    public int direction_y = 0;
    public EnemyShootStraight(int side, int x, int y, int health, int gridWidth, int gridHeight) : base(side, x, y, health, gridWidth, gridHeight)
    {
        this.getBestDirection();
    }

    public override ICommand TakeTurn(int[,] map)
    {
        return ShootClockwise();
    }

    ICommand ShootClockwise()
    {

        return new ShootCommand(this, this.direction_x, this.direction_y);
    }

    void getBestDirection()
    {
        if (this.position_x < this.position_y)
        {
            if (this.position_x < this.gridWidth / 2)
            {
                this.direction_x = 1;
                this.direction_y = 0;
            }
            else
            {
                this.direction_x = -1;
                this.direction_y = 0;
            }
        }
        else
        {
            if (this.position_y < this.gridHeight / 2)
            {
                this.direction_x = 0;
                this.direction_y = 1;
            }
            else
            {
                this.direction_x = 0;
                this.direction_y = -1;
            }
        }
    }

}
