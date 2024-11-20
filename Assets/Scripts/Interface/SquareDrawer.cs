using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareDrawer : MonoBehaviour
{
    public ColorImage squarePrefab;
    public Sprite playerSprite;
    public Sprite enemySprite;
    public Sprite playerProjectileSprite;
    public Sprite enemyProjectileSprite;
    public Sprite holeSprite;
    public Sprite wallSprite;

    private List<ColorImage> squares = new List<ColorImage>();

    public void ClearGrid()
    {
        foreach (ColorImage square in squares)
        {
            Destroy(square.gameObject);
        }
        squares.Clear();
    }

    public void DrawGrid(int[,] grid, int gridWidth, int gridHeight)
    {
        ClearGrid();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] != GridConstant.EMPTY)
                {
                    //Debug.Log($"Square at {x}, {y} is not empty, value is {grid[x, y]}");
                    
                    if ((grid[x, y] & GridConstant.WALL) != 0)
                    {
                        ColorImage square = Instantiate(squarePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        square.SetSprite(wallSprite);
                        square.SetColor(Color.gray);
                        squares.Add(square);
                    }
                    else if ((grid[x, y] & GridConstant.HOLE) != 0)
                    {
                        ColorImage square = Instantiate(squarePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        square.SetSprite(holeSprite);
                        square.SetColor(Color.black);
                        squares.Add(square);
                    }
                    if ((grid[x, y] & GridConstant.PLAYER) != 0)
                    {
                        ColorImage square = Instantiate(squarePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        square.SetSprite(playerSprite);
                        squares.Add(square);
                    }
                    else if ((grid[x, y] & GridConstant.ENEMY) != 0)
                    {
                        ColorImage square = Instantiate(squarePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        square.SetSprite(enemySprite);
                        squares.Add(square);
                    }
                    if ((grid[x, y] & GridConstant.PLAYER_PROJECTILE) != 0)
                    {
                        ColorImage square = Instantiate(squarePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        square.SetSprite(playerProjectileSprite);
                        squares.Add(square);
                    }
                    if ((grid[x, y] & GridConstant.ENEMY_PROJECTILE) != 0)
                    {
                        ColorImage square = Instantiate(squarePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        square.SetSprite(enemyProjectileSprite);
                        squares.Add(square);
                    }
                    if ((grid[x, y] & GridConstant.PLAYER_ATTACK) != 0)
                    {
                        ColorImage square = Instantiate(squarePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        square.SetColor(Color.blue);
                        squares.Add(square);
                    }
                    if ((grid[x, y] & GridConstant.ENEMY_ATTACK) != 0)
                    {
                        ColorImage square = Instantiate(squarePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                        square.SetColor(Color.red);
                        squares.Add(square);
                    }
                    
                }
            }
        }
    }
}
