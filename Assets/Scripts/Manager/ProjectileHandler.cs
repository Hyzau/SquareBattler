// Contain a Script to handle the combatant cooldowns and the combatant turns
using UnityEngine;

public class ProjectileHandler
{
    // the side of the shooter
    private int side;
    // current position
    private int x;
    private int y;
    private Vector2Int direction;
    private int mapWidth;
    private int mapHeight;
    public bool toDestroy = false;

    public ProjectileHandler(int side, int x, int y, Vector2Int direction, int mapWidth, int mapHeight)
    {
        this.side = side;
        this.x = x;
        this.y = y;
        this.direction = direction;
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
    }

    public void Move(int[,] map)
    {
        int nextX = this.x + this.direction.x;
        int nextY = this.y + this.direction.y;

        map[this.x, this.y] &= ~side;
        if (nextX < 0 || nextX >= this.mapWidth || nextY < 0 || nextY >= this.mapHeight)
        {
            toDestroy = true;
            return;
        }
        if ((map[nextX, nextY] & GridConstant.WALL) != 0)
        {
            toDestroy = true;
            return;
        }
        map[nextX, nextY] |= side;
        this.x = nextX;
        this.y = nextY;
    }
}