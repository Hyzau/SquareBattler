using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    public int gridSize = 15;     // Number of cells (grid lines will be one more).
    public float cellSize = 1.0f; // Size of each cell.
    public SquareDrawer squareDrawer;
    public int level = 1;
    public TMP_Text levelText;
    public float turnTime = 0.25f;
    private bool isChangingLevel = false;

    private float timeBeforeNextTurn = 0.0f;

    private int[,] grid;
    private List<CombatantHandler> combatants;
    private List<ProjectileHandler> projectiles;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize the grid with empty cells.
        this.grid = new int[this.gridSize, this.gridSize];
        InitializeLevel();
    }

    private void ClearGrid()
    {
        for (int x = 0; x < this.gridSize; x++)
        {
            for (int y = 0; y < this.gridSize; y++)
            {
                this.grid[x, y] = GridConstant.EMPTY;
            }
        }
    }

    private bool isPlayerDead()
    {
        foreach (CombatantHandler handler in this.combatants)
        {
            if (handler.side == SideConstant.PLAYER && handler.health > 0)
            {
                return false;
            }
        }
        return true;
    }
    // Update is called once per frame
    void Update()
    {
        if (this.timeBeforeNextTurn > 0)
        {
            this.timeBeforeNextTurn -= Time.deltaTime;
            return; // Do nothing until the time is up
        }
        if (isPlayerDead())
        {
            Debug.Log("Game Over");
            Application.Quit(); // Quit the game
            return;
        }
        if (this.combatants.Count == 1)
        {
            if (this.isChangingLevel == false)
            {
                this.isChangingLevel = true;
                StartCoroutine(NextLevel());
            }    
            return;
        }
        // clear the attacks from the grid
        this.ClearAttacks();
        this.combatants.RemoveAll(handler => handler.health <= 0);
        // Update the projectiles before the combatants because combatant can instantiate new projectiles
        foreach (ProjectileHandler projectile in this.projectiles)
        {
            projectile.Move(this.grid);
        }
        this.projectiles.RemoveAll(projectile => projectile.toDestroy);
        // Update the combatants
        foreach (CombatantHandler handler in this.combatants)
        {
            if (handler.currentCooldown > 0)
            {
                handler.currentCooldown -= 1;
            }
            else
            {
                ICommand command = handler.combatant.TakeTurn(this.grid);
                if (command != null)
                {
                    command.Execute(this);
                }
            }
        }
        // clear the projectiles that are marked to destroy
        this.detectAttackAndProjectileCollisions();
        // Draw what happened during the turn
        this.squareDrawer.DrawGrid(this.grid, this.gridSize, this.gridSize);
        this.timeBeforeNextTurn = this.turnTime;
    }

    private void detectAttackAndProjectileCollisions()
    {
        // Detect if player or enemies are hit by projectiles or attacks
        foreach (CombatantHandler handler in this.combatants)
        {
            int side = handler.side;
            if (side == SideConstant.PLAYER)
            {
                if ((this.grid[handler.x, handler.y] & GridConstant.ENEMY_ATTACK) != 0 || (this.grid[handler.x, handler.y] & GridConstant.ENEMY_PROJECTILE) != 0)
                {
                    handler.health -= 1;
                    handler.combatant.TakeDamage(1);
                    if (handler.health <= 0)
                    {
                        this.grid[handler.x, handler.y] &= ~GridConstant.PLAYER;
                        Debug.Log($"Removing player from the game");
                    }
                }
            }
            else if (side == SideConstant.ENEMY)
            {
                if ((this.grid[handler.x, handler.y] & GridConstant.PLAYER_ATTACK) != 0 || (this.grid[handler.x, handler.y] & GridConstant.PLAYER_PROJECTILE) != 0)
                {
                    handler.health -= 1;
                    handler.combatant.TakeDamage(1);
                    if (handler.health <= 0)
                    {
                        this.grid[handler.x, handler.y] &= ~GridConstant.ENEMY;
                    }
                }
            }
        }
    }

    private void ClearAttacks()
    {
        for (int x = 0; x < this.gridSize; x++)
        {
            for (int y = 0; y < this.gridSize; y++)
            {
                this.grid[x, y] &= ~(GridConstant.PLAYER_ATTACK | GridConstant.ENEMY_ATTACK);
            }
        }
    }

 

    private CombatantHandler FindHandlerFromCombatant(Combatant combatant)
    {
        foreach (CombatantHandler handler in this.combatants)
        {
            if (handler.combatant == combatant)
            {
                return handler;
            }
        }
        return null;
    }

    private CombatantHandler FindHandlerFromPosition(int x, int y)
    {
        foreach (CombatantHandler handler in this.combatants)
        {
            if (handler.x == x && handler.y == y)
            {
                return handler;
            }
        }
        return null;
    }

    public bool isWithinBounds(int x, int y)
    {
        if (x < 0 || x >= this.gridSize)
        {
            Debug.LogWarning($"Error: {x}*, {y}, invalid command, outside the grid");
            return false;
        }
        if (y < 0 || y >= this.gridSize)
        {
            Debug.LogWarning($"Error: {x}, {y}*, invalid command, outside the grid");
            return false;
        }
        return true;
    }

    public bool isCommandValid(int x, int y, int new_x, int new_y)
    {
        if (Math.Abs(x) > 1)
        {
            Debug.LogWarning($"Error: {x}*, {y}, invalid command, we can only excute on adjacent square");
            return false;
        }
        if (Math.Abs(y) > 1)
        {
            Debug.LogWarning($"Error: {x}, {y}*, invalid command, we can only excute on adjacent square");
            return false;
        }
        if (Math.Abs(x) + Math.Abs(y) > 1)
        {
            Debug.LogWarning($"Error: {x}*, {y}*, invalid command, we can only excute on adjacent square");
            return false; 
        }
        if (!isWithinBounds(new_x, new_y))
        {
            return false;
        }
        return true;
    }

    public int Move(Combatant fighter, int x, int y)
    {
        CombatantHandler handler = this.FindHandlerFromCombatant(fighter);
        if (handler == null)
        {
            Debug.LogError("Invalid combatant in Move");
            return -1; // Invalid combatant
        }
        int old_x = handler.x;
        int old_y = handler.y;
        int new_x = old_x + x;
        int new_y = old_y + y;
        int side = GridConstant.PLAYER;

        if (handler.side == SideConstant.ENEMY)
        {
            side = GridConstant.ENEMY;
        }
        // Even if an error occurs, the Combatant will be on cooldown
        handler.currentCooldown = 2;
        if (!isCommandValid(x, y, new_x, new_y))
        {
            return -1; // Invalid move
        }
        if ((this.grid[new_x, new_y] & GridConstant.WALL) != 0 || (this.grid[new_x, new_y] & GridConstant.HOLE) != 0)
        {
            Debug.LogWarning($"Error: {x}, {y}, invalid move, the cell is an obstacle");
            return -2; // Invalid move, the cell is an obstacle
        }
        if ((this.grid[new_x, new_y] & GridConstant.ENEMY) != 0 || (this.grid[new_x, new_y] & GridConstant.PLAYER) != 0)
        {
            Debug.LogWarning($"Error: {x}, {y}, invalid move, the cell is occupied");
            return -3; // Invalid move, the cell is occupied
        }
        handler.x = new_x;
        handler.y = new_y;
        this.grid[old_x, old_y] = this.grid[old_x, old_y] & ~side; // Remove the fighter from the old cell
        this.grid[new_x, new_y] =  this.grid[new_x, new_y] | side; // Add the fighter to the new cell
        //Debug.Log($"Moved FINISHED {fighter} from {old_x}, {old_y} to {new_x}, {new_y}. Values are {this.grid[old_x, old_y]} and {this.grid[new_x, new_y]}");
        return 0;
    }

    public int Attack(Combatant fighter, int x, int y)
    {
        CombatantHandler handler = this.FindHandlerFromCombatant(fighter);
        if (handler == null)
        {
            Debug.LogError("Invalid combatant in Attack");
            return -1; // Invalid combatant
        }
        // Even if an error occurs, the Combatant will be on cooldown
        handler.currentCooldown = 1;

        int old_x = handler.x;
        int old_y = handler.y;
        int new_x = old_x + x;
        int new_y = old_y + y;
        int toDisplay = GridConstant.PLAYER_ATTACK;
        if (handler.side == SideConstant.ENEMY)
        {
            toDisplay = GridConstant.ENEMY_ATTACK;
        }
        if (!isCommandValid(x, y, new_x, new_y))
        {
            return -2; // Invalid move
        }
        this.grid[new_x, new_y] |= toDisplay;
        return 0;
    }

    public int Slash(Combatant fighter, int x, int y)
    {
        CombatantHandler handler = this.FindHandlerFromCombatant(fighter);
        if (handler == null)
        {
            Debug.LogError("Invalid combatant in Attack");
            return -1; // Invalid combatant
        }
        // Even if an error occurs, the Combatant will be on cooldown
        handler.currentCooldown = 3;

        int old_x = handler.x;
        int old_y = handler.y;
        int new_x = old_x + x;
        int new_y = old_y + y;
        int toDisplay = GridConstant.PLAYER_ATTACK;
        if (handler.side == SideConstant.ENEMY)
        {
            toDisplay = GridConstant.ENEMY_ATTACK;
        }
        if (!isCommandValid(x, y, new_x, new_y))
        {
            return -2; // Invalid move
        }
        this.grid[new_x, new_y] |= toDisplay;
        // Slash also attacks the adjacent cells of new_x, new_y based on direction
        if (x == 1)
        {
            if (new_y + 1 < this.gridSize)
            {
                this.grid[new_x, new_y + 1] |= toDisplay;
            }
            if (new_y - 1 >= 0)
            {
                this.grid[new_x, new_y - 1] |= toDisplay;
            }
        }
        else if (x == -1)
        {
            if (new_y + 1 < this.gridSize)
            {
                this.grid[new_x, new_y + 1] |= toDisplay;
            }
            if (new_y - 1 >= 0)
            {
                this.grid[new_x, new_y - 1] |= toDisplay;
            }
        }
        else if (y == 1)
        {
            if (new_x + 1 < this.gridSize)
            {
                this.grid[new_x + 1, new_y] |= toDisplay;
            }
            if (new_x - 1 >= 0)
            {
                this.grid[new_x - 1, new_y] |= toDisplay;
            }
        }
        else if (y == -1)
        {
            if (new_x + 1 < this.gridSize)
            {
                this.grid[new_x + 1, new_y] |= toDisplay;
            }
            if (new_x - 1 >= 0)
            {
                this.grid[new_x - 1, new_y] |= toDisplay;
            }
        }
        return 0;
    }

    public int Thurst(Combatant fighter, int x, int y)
    {
        CombatantHandler handler = this.FindHandlerFromCombatant(fighter);
        if (handler == null)
        {
            Debug.LogError("Invalid combatant in Attack");
            return -1; // Invalid combatant
        }
        // Even if an error occurs, the Combatant will be on cooldown
        handler.currentCooldown = 3;

        int old_x = handler.x;
        int old_y = handler.y;
        int new_x = old_x + x;
        int new_y = old_y + y;
        int toDisplay = GridConstant.PLAYER_ATTACK;
        if (handler.side == SideConstant.ENEMY)
        {
            toDisplay = GridConstant.ENEMY_ATTACK;
        }
        if (!isCommandValid(x, y, new_x, new_y))
        {
            return -2; // Invalid move
        }
        this.grid[new_x, new_y] |= toDisplay;
        // Thurst also attacks the cells in the direction of x, y based on direction
        if (x == 1)
        {
            if (new_x + 1 < this.gridSize)
            {
                this.grid[new_x + 1 , new_y] |= toDisplay;
            }
        }
        else if (x == -1)
        {
            if (new_x - 1 >= 0)
            {
                this.grid[new_x - 1, new_y] |= toDisplay;
            }
        }
        else if (y == 1)
        {
            if (new_y + 1 < this.gridSize)
            {
                this.grid[new_x, new_y + 1] |= toDisplay;
            }
        }
        else if (y == -1)
        {
            if (new_y - 1 >= 0)
            {
                this.grid[new_x, new_y - 1] |= toDisplay;
            }
        }
        return 0;
    }

    public int Whirlwind(Combatant fighter, int x, int y)
    {
        CombatantHandler handler = this.FindHandlerFromCombatant(fighter);
        if (handler == null)
        {
            Debug.LogError("Invalid combatant in Attack");
            return -1; // Invalid combatant
        }
        // Even if an error occurs, the Combatant will be on cooldown
        handler.currentCooldown = 6;

        
        int toDisplay = GridConstant.PLAYER_ATTACK;
        if (handler.side == SideConstant.ENEMY)
        {
            toDisplay = GridConstant.ENEMY_ATTACK;
        }
        // Whirlwind attacks all adjacent cells
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // skip current cell 
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int new_x = handler.x + i;
                int new_y = handler.y + j;
                if (!isWithinBounds(new_x, new_y))
                {
                    continue;
                }
                this.grid[new_x, new_y] |= toDisplay;
            }
        }
        return 0;
    }


    public int Shoot(Combatant fighter, int x, int y)
    {
        CombatantHandler handler = this.FindHandlerFromCombatant(fighter);
        if (handler == null)
        {
            Debug.LogError("Invalid combatant in Attack");
            return -1; // Invalid combatant
        }
        // Even if an error occurs, the Combatant will be on cooldown
        handler.currentCooldown = 8;

        int old_x = handler.x;
        int old_y = handler.y;
        int new_x = old_x + x;
        int new_y = old_y + y;
        int sideToDisplay = GridConstant.PLAYER_PROJECTILE;
        if (handler.side == SideConstant.ENEMY)
        {
            sideToDisplay = GridConstant.ENEMY_PROJECTILE;
        }
        if (!isCommandValid(x, y, new_x, new_y))
        {
            return -2; // Invalid move
        }
        Vector2Int direction = new Vector2Int(x, y);
        this.grid[new_x, new_y] |= sideToDisplay; // Put the projectile in the new cell
        ProjectileHandler projectile = new ProjectileHandler(sideToDisplay, new_x, new_y, direction, this.gridSize, this.gridSize);
        this.projectiles.Add(projectile);
        return 0;
    }

    IEnumerator NextLevel()
    {
        if (this.level == 10)
        {
            this.levelText.text = $"You have completed all levels!";
            yield return new WaitForSeconds(2.0f);
            Application.Quit(); // Quit the game
            yield break;
        }
        else 
        {
            this.levelText.text = $"Level {this.level} completed!";
            yield return new WaitForSeconds(2.0f);
            this.levelText.text = "";
            this.level += 1;
            InitializeLevel();
        }
        
    }


    private void InitializeLevel()
    {
        if (this.combatants != null && this.combatants.Count > 0)
        {
            this.combatants.Clear();
        }
        if (this.projectiles != null && this.projectiles.Count > 0)
        {
            this.projectiles.Clear();
        }
        this.ClearGrid();
        this.combatants = new List<CombatantHandler>();
        this.projectiles = new List<ProjectileHandler>();
        // Initialize the player
        Combatant player = new Player(0, this.gridSize / 2, this.gridSize / 2, 2, this.gridSize, this.gridSize);
        CombatantHandler playerHandler = new CombatantHandler(0, SideConstant.PLAYER, this.gridSize / 2, this.gridSize / 2, player, 0, 2);
        this.combatants.Add(playerHandler); // Player with ID 0
        this.grid[this.gridSize / 2, this.gridSize / 2] |= GridConstant.PLAYER;
        // Now intialize the enemies based on the level
        switch (this.level)
        {
            case 1:
                this.level1();
                break;
            case 2:
                this.level2();
                break;
            case 3:
                this.level3();
                break;
            case 4:
                this.level4();
                break;
            case 5:
                this.level5();
                break;
            case 6:
                this.level6();
                break;
            case 7:
                this.level7();
                break;
            case 8:
                this.level8();
                break;
            case 9:
                this.level9();
                break;
            case 10:
                this.level10();
                break;
            default:
                Debug.LogError("Invalid level "+ this.level);
                break;
        }
        this.isChangingLevel = false;
    }

    private void level1()
    {
        Debug.Log("Initialize Level 1");
        // A simple enemy that does nothing
        Combatant enemy1 = new EnemyDoNothing(SideConstant.ENEMY, 0, 0, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler1 = new CombatantHandler(1, SideConstant.ENEMY, 0, 0, enemy1, 0, 1);
        this.combatants.Add(enemyHandler1); // Enemy with ID 1
        this.grid[0, 0] |= GridConstant.ENEMY;
    }

    private void level2()
    {
        // A three enemy at random postions that do nothing
        for (int i = 0; i < 3; i++)
        {
            int x = UnityEngine.Random.Range(0, this.gridSize);
            int y = UnityEngine.Random.Range(0, this.gridSize);
            if (x == this.gridSize / 2 && y == this.gridSize / 2)
            {
                i--;
                continue; // retry other position
            }
            Combatant enemy1 = new EnemyDoNothing(SideConstant.ENEMY, x, y, 1, this.gridSize, this.gridSize);
            CombatantHandler enemyHandler1 = new CombatantHandler(i + 1, SideConstant.ENEMY, x, y, enemy1, 0, 1);
            this.combatants.Add(enemyHandler1); // Enemy with ID 1
            this.grid[x, y] |= GridConstant.ENEMY;
        }
    }

    private void level3()
    {
        // One enemy that moves around
        Combatant enemy1 = new EnemyMoveAround(SideConstant.ENEMY, 0, 0, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler1 = new CombatantHandler(1, SideConstant.ENEMY, 0, 0, enemy1, 0, 1);
        this.combatants.Add(enemyHandler1); // Enemy with ID 1
        this.grid[0, 0] |= GridConstant.ENEMY;
    }

    private void level4()
    {
        // One enemy that moves around
        Combatant enemy1 = new EnemyMoveAroundAndAttack(SideConstant.ENEMY, 0, 0, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler1 = new CombatantHandler(1, SideConstant.ENEMY, 0, 0, enemy1, 0, 1);
        this.combatants.Add(enemyHandler1); // Enemy with ID 1
        this.grid[0, 0] |= GridConstant.ENEMY;
    }

    private void level5()
    {
        // four enemies that move around, one at each corner
        for (int i = 0; i < 4; i++)
        {
            int x = 0;
            int y = 0;
            if (i == 1)
            {
                x = this.gridSize - 1;
            }
            else if (i == 2)
            {
                y = this.gridSize - 1;
            }
            else if (i == 3)
            {
                x = this.gridSize - 1;
                y = this.gridSize - 1;
            }
            Combatant enemy1 = new EnemyMoveAround(SideConstant.ENEMY, x, y, 1, this.gridSize, this.gridSize);
            CombatantHandler enemyHandler1 = new CombatantHandler(i + 1, SideConstant.ENEMY, x, y, enemy1, 0, 1);
            this.combatants.Add(enemyHandler1); // Enemy with ID 1
            this.grid[x, y] |= GridConstant.ENEMY;
        }
    }

    private void level6()
    {
        // Four enemies that move around and attack
        for (int i = 0; i < 4; i++)
        {
            int x = 0;
            int y = 0;
            if (i == 1)
            {
                x = this.gridSize - 1;
            }
            else if (i == 2)
            {
                y = this.gridSize - 1;
            }
            else if (i == 3)
            {
                x = this.gridSize - 1;
                y = this.gridSize - 1;
            }
            Combatant enemy1 = new EnemyMoveAroundAndAttack(SideConstant.ENEMY, x, y, 1, this.gridSize, this.gridSize);
            CombatantHandler enemyHandler1 = new CombatantHandler(i + 1, SideConstant.ENEMY, x, y, enemy1, 0, 1);
            this.combatants.Add(enemyHandler1); // Enemy with ID 1
            this.grid[x, y] |= GridConstant.ENEMY;
        }
    }

    private void level7()
    {
        // One enemy that shoots projectiles
        int x = 2;
        int y = 1;
        Combatant enemy1 = new EnemyShootStraight(SideConstant.ENEMY, x, y, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler1 = new CombatantHandler(1, SideConstant.ENEMY, x, y, enemy1, 0, 1);
        this.combatants.Add(enemyHandler1); // Enemy with ID 1
        this.grid[x, y] |= GridConstant.ENEMY;
        // Another enemy that moves around and attacks
        int x2 = 0;
        int y2 = 0;
        Combatant enemy2 = new EnemyMoveAroundAndAttack(SideConstant.ENEMY, x2, y2, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler2 = new CombatantHandler(2, SideConstant.ENEMY, x2, y2, enemy2, 0, 1);
        this.combatants.Add(enemyHandler2); // Enemy with ID 2
        this.grid[x2, y2] |= GridConstant.ENEMY;
        // Another enemy that moves randomly
        int x3 = 12;
        int y3 = 12;
        Combatant enemy3 = new EnemyRandomMove(SideConstant.ENEMY, x3, y3, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler3 = new CombatantHandler(3, SideConstant.ENEMY, x3, y3, enemy3, 0, 1);
        this.combatants.Add(enemyHandler3); // Enemy with ID 3
        this.grid[x3, y3] |= GridConstant.ENEMY;
        int x4 = 12;
        int y4 = 2;
        Combatant enemy4 = new EnemyRandomMove(SideConstant.ENEMY, x4, y4, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler4 = new CombatantHandler(3, SideConstant.ENEMY, x4, y4, enemy4, 0, 1);
        this.combatants.Add(enemyHandler4); // Enemy with ID 3
        this.grid[x4, y4] |= GridConstant.ENEMY;
    }

    private void level8()
    {
        // Four enemies that moves around and whirl
        for (int i = 0; i < 4; i++)
        {
            int x = 0;
            int y = 0;
            if (i == 1)
            {
                x = this.gridSize - 1;
            }
            else if (i == 2)
            {
                y = this.gridSize - 1;
            }
            else if (i == 3)
            {
                x = this.gridSize - 1;
                y = this.gridSize - 1;
            }
            Combatant enemy1 = new EnemyRandomMoveAndWhirl(SideConstant.ENEMY, x, y, 1, this.gridSize, this.gridSize);
            CombatantHandler enemyHandler1 = new CombatantHandler(i + 1, SideConstant.ENEMY, x, y, enemy1, 0, 1);
            this.combatants.Add(enemyHandler1); // Enemy with ID 1
            this.grid[x, y] |= GridConstant.ENEMY;
        }
    }

    private void level9()
    {
        // An enemy that shoots projectiles almost surrounded by holes
        int x = 5;
        int y = 5;
        Combatant enemy1 = new EnemyShootClockwise(SideConstant.ENEMY, x, y, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler1 = new CombatantHandler(1, SideConstant.ENEMY, x, y, enemy1, 0, 1);
        this.combatants.Add(enemyHandler1); // Enemy with ID 1
        this.grid[x, y] |= GridConstant.ENEMY;
        // Surround the enemy with holes
        this.grid[x + 1, y] |= GridConstant.HOLE;
        this.grid[x - 1, y] |= GridConstant.HOLE;
        this.grid[x, y + 1] |= GridConstant.HOLE;
        this.grid[x - 1, y - 1] |= GridConstant.HOLE;
        this.grid[x + 1, y + 1] |= GridConstant.HOLE;
        this.grid[x - 1, y + 1] |= GridConstant.HOLE;
        this.grid[x + 1, y - 1] |= GridConstant.HOLE;
    }

    private void level10()
    {
        // An enemy that shoots projectiles almost surrounded by walls
        int x = 8;
        int y = 11;
        Combatant enemy1 = new EnemyShootClockwise(SideConstant.ENEMY, x, y, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler1 = new CombatantHandler(1, SideConstant.ENEMY, x, y, enemy1, 0, 1);
        this.combatants.Add(enemyHandler1); // Enemy with ID 1
        this.grid[x, y] |= GridConstant.ENEMY;
        // Surround the the 3x3 square around the enemy with walls
        // East wall
        this.grid[x + 2, y - 2] |= GridConstant.WALL;
        this.grid[x + 2, y - 1] |= GridConstant.WALL;
        this.grid[x + 2, y] |= GridConstant.WALL;
        this.grid[x + 2, y + 1] |= GridConstant.WALL;
        this.grid[x + 2, y + 2] |= GridConstant.WALL;
        // North wall
        this.grid[x - 2, y + 2] |= GridConstant.WALL;
        this.grid[x - 1, y + 2] |= GridConstant.WALL;
        this.grid[x, y + 2] |= GridConstant.WALL;
        this.grid[x + 1, y + 2] |= GridConstant.WALL;
        //this.grid[x + 2, y + 2] |= GridConstant.WALL;
        // West wall
        this.grid[x - 2, y - 2] |= GridConstant.WALL;
        this.grid[x - 2, y - 1] |= GridConstant.WALL;
        this.grid[x - 2, y] |= GridConstant.WALL;
        this.grid[x - 2, y + 1] |= GridConstant.WALL;
        this.grid[x - 2, y + 2] |= GridConstant.WALL;
        // Leave the south wall open

        // Put another enmy behind the wall
        int x2 = 8;
        int y2 = 14;
        Combatant enemy2 = new EnemyDoNothing(SideConstant.ENEMY, x2, y2, 1, this.gridSize, this.gridSize);
        CombatantHandler enemyHandler2 = new CombatantHandler(2, SideConstant.ENEMY, x2, y2, enemy2, 0, 1);
        this.combatants.Add(enemyHandler2); // Enemy with ID 2
        this.grid[x2, y2] |= GridConstant.ENEMY;
    }

}
