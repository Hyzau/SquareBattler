public class WhirlwindCommand : ICommand
{
    private Combatant combatant;
    private int x;
    private int y;

    public WhirlwindCommand(Combatant combatant, int x, int y)
    {
        this.combatant = combatant;
        this.x = x;
        this.y = y;
    }

    public void Execute(GameManager gameManager)
    {
        gameManager.Whirlwind(combatant, x, y);
    }
}