public class AttackCommand : ICommand
{
    private Combatant combatant;
    private int x;
    private int y;

    public AttackCommand(Combatant combatant, int x, int y)
    {
        this.combatant = combatant;
        this.x = x;
        this.y = y;
    }

    public void Execute(GameManager gameManager)
    {
        gameManager.Attack(combatant, x, y);
    }
}