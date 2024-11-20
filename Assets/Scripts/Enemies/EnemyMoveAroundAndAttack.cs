using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveAroundAndAttack : Combatant
{
    public bool hasMoved = false;
    public EnemyMoveAroundAndAttack(int side, int x, int y, int health, int gridWidth, int gridHeight) : base(side, x, y, health, gridWidth, gridHeight)
    {
    }

    public override ICommand TakeTurn(int[,] map)
    {
        if (!this.hasMoved)
        {
            this.hasMoved = true;
            return MoveAround();
        }
        this.hasMoved = false;
        return Attack();

    }

    ICommand MoveAround()
    {
        if (this.position_x == 0 && this.position_y < this.gridHeight - 1)
        {
            //Debug.Log($"Moving Up at {this.position_x}, {this.position_y} because {this.position_y} < {this.gridHeight}");
            this.position_y = this.position_y + 1;
            return new MoveCommand(this, 0, 1);
        }
        else if (this.position_y == this.gridHeight - 1 && this.position_x < this.gridWidth - 1)
        {
            //Debug.Log($"Moving Right at {this.position_x}, {this.position_y} because {this.position_x} < {this.gridWidth}");
            this.position_x = this.position_x + 1;
            return new MoveCommand(this, 1, 0);
        }
        else if (this.position_x == this.gridWidth - 1 && this.position_y > 0)
        {
            //Debug.Log($"Moving Down at {this.position_x}, {this.position_y} because {this.position_y} > 0");
            this.position_y = this.position_y - 1;
            return new MoveCommand(this, 0, -1);
        }
        //else if (this.position_y == 0 && this.position_x > 0)
        //Debug.Log($"Moving Left at {this.position_x}, {this.position_y} because {this.position_x} > 0");
        this.position_x = this.position_x - 1;
        return new MoveCommand(this, -1, 0);
    }

    ICommand Attack()
    {
        // based on our position, we always attack the center of the grid
        int x = 0;
        int y = 0;
        if (this.position_x == 0 && this.position_y < this.gridHeight - 1)
        {
            x = 1;
        }
        else if (this.position_y == this.gridHeight - 1 && this.position_x < this.gridWidth - 1)
        {
            y = -1;
        }
        else if (this.position_x == this.gridWidth - 1 && this.position_y > 0)
        {
            x = -1;
        }
        else if (this.position_y == 0 && this.position_x > 0)
        {
            y = 1;
        }

        return new ThurstCommand(this, x, y);
    } 
}
