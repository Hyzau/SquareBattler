public class ShootCommand : ICommand
{
    private Combatant combatant;
    private int x;
    private int y;

    public ShootCommand(Combatant combatant, int x, int y)
    {
        this.combatant = combatant;
        this.x = x;
        this.y = y;
    }

    public void Execute(GameManager gameManager)
    {
        gameManager.Shoot(combatant, x, y);
    }
}