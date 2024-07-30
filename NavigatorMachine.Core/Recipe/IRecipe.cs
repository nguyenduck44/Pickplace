namespace NavigatorMachine.Core
{
    public interface IRecipe
    {
        string Name { get; set; }
        bool Initialized { get; set; }

        T Load<T>();
        void Save();
    }
}