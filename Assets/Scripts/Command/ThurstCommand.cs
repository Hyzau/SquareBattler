public class ThurstCommand : ICommand
{
    private Combatant combatant;
    private int x;
    private int y;

    public ThurstCommand(Combatant combatant, int x, int y)
    {
        this.combatant = combatant;
        this.x = x;
        this.y = y;
    }

    public void Execute(GameManager gameManager)
    {
        gameManager.Thurst(combatant, x, y);
    }
}