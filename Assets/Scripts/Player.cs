using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Combatant
{
    public int direction_x = 1;
    public int direction_y = 0;
    public Player(int side, int x, int y, int health, int gridWidth, int gridHeight) : base(side, x, y, health, gridWidth, gridHeight)
    {
    }

    /*public override ICommand TakeTurn(int[,] map)
    {
        return null; // Do nothing
    }*/

    public override ICommand TakeTurn(int[,] map)
    {
        return this.ManualControl();
    }

    ICommand ManualControl()
    {
        if (Input.GetMouseButton(0))
        {
            return new AttackCommand(this, this.direction_x, this.direction_y);
        }
        else if (Input.GetMouseButton(1))
        {
            return new ShootCommand(this, this.direction_x, this.direction_y);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            return new ThurstCommand(this, this.direction_x, this.direction_y);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            return new SlashCommand(this, this.direction_x, this.direction_y);
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            return new WhirlwindCommand(this, this.direction_x, this.direction_y);
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            this.position_y = this.position_y + 1;
            this.direction_x = 0;
            this.direction_y = 1;
            return new MoveCommand(this, 0, 1);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.position_x = this.position_x + 1;
            this.direction_x = 1;
            this.direction_y = 0;
            return new MoveCommand(this, 1, 0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            this.position_y = this.position_y - 1;
            this.direction_x = 0;
            this.direction_y = -1;
            return new MoveCommand(this, 0, -1);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            this.position_x = this.position_x - 1;
            this.direction_x = -1;
            this.direction_y = 0;
            return new MoveCommand(this, -1, 0);
        }
        return null;
    }
}