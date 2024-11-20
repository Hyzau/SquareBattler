using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootClockwise : Combatant
{
    public int direction_x = 1;
    public int direction_y = 0;
    public EnemyShootClockwise(int side, int x, int y, int health, int gridWidth, int gridHeight) : base(side, x, y, health, gridWidth, gridHeight)
    {
    }

    public override ICommand TakeTurn(int[,] map)
    {
        return ShootClockwise();
    }

    ICommand ShootClockwise()
    {
        this.RotateClockwise();
        this.getValidDirection();
        return new ShootCommand(this, this.direction_x, this.direction_y);
    }

    void getValidDirection()
    {
        // if we are shooting in the direction of the edge of the grid, we need to rotate clockwise until not facing an edge
        int maximumRotations = 4;
        while (!isDirectionValid() && maximumRotations > 0)
        {
            this.RotateClockwise();
            maximumRotations--; // avoid infinite loop
        }
    }

    public bool isDirectionValid()
    {

        if (this.direction_x == 0 && this.direction_y == 1 && this.position_y >= this.gridHeight - 1)
        {
            return false;
        }
        else if (this.direction_x == 0 && this.direction_y == -1 && this.position_y <= 0)
        {
            return false;
        }
        else if (this.direction_y == 0 && this.direction_x == 1 && this.position_x >= this.gridWidth - 1)
        {
            return false;
        }
        else if (this.direction_y == 0 && this.direction_x == -1 && this.position_x <= 0)
        {
            return false;
        }

        return true;
    }

    void RotateClockwise()
    {
        if (this.direction_x == -1)
        {
            this.direction_x = 0;
            this.direction_y = 1;
        }
        else if (this.direction_y == 1)
        {
            this.direction_x = 1;
            this.direction_y = 0;
        }
        else if (this.direction_x == 1)
        {
            this.direction_x = 0;
            this.direction_y = -1;
        }
        else if (this.direction_y == -1)
        {
            this.direction_x = -1;
            this.direction_y = 0;
        }
    }
}
