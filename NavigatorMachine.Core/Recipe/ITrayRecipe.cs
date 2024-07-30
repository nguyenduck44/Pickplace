namespace NavigatorMachine.Core
{
    public delegate void EventHandler();

    public interface ITrayRecipe : IRecipe
    {
        int Columns { get; set; }
        int Rows { get; set; }

        event EventHandler ChangeColumnRowEvent;
    }
}