public class SlashCommand : ICommand
{
    private Combatant combatant;
    private int x;
    private int y;

    public SlashCommand(Combatant combatant, int x, int y)
    {
        this.combatant = combatant;
        this.x = x;
        this.y = y;
    }

    public void Execute(GameManager gameManager)
    {
        gameManager.Slash(combatant, x, y);
    }
}