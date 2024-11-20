public class MoveCommand : ICommand
{
    private Combatant combatant;
    private int x;
    private int y;

    public MoveCommand(Combatant combatant, int x, int y)
    {
        this.combatant = combatant;
        this.x = x;
        this.y = y;
    }

    public void Execute(GameManager gameManager)
    {
        gameManager.Move(combatant, x, y);
    }
}