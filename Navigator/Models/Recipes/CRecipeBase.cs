using NavigatorMachine.Core;
using NavigatorMachine.Defines;
using Newtonsoft.Json;
using System.IO;

namespace NavigatorMachine.Models
{
    public class CRecipeBase : ObservableObject, IRecipe
    {
        private readonly object recipeReadWriteLock = new object();

        public string Name { get; set; }

        public bool Initialized { get; set; }
        public CRecipeBase()
        {
            Name = this.GetType().Name;
        }

        protected string SerializeString
        {
            get
            {
                string recipeFile;

                recipeFile = Path.Combine(CRecipeFolder.CurrentRecipeFolder, Name);

                if (!File.Exists(recipeFile))
                {
                    using (StreamWriter sw = File.AppendText(recipeFile))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
                    }
                }

                return File.ReadAllText(recipeFile);
            }
        }

        public void Save()
        {
            if (!Initialized) return;

            lock (recipeReadWriteLock)
            {
                string recipeFile, recipeFileContent;

                recipeFile = Path.Combine(CRecipeFolder.CurrentRecipeFolder, Name);
                recipeFileContent = JsonConvert.SerializeObject(this, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });

                File.WriteAllText(recipeFile, recipeFileContent);
            }
        }

        public T Load<T>()
        {
            lock (recipeReadWriteLock)
            {
                T tmp = JsonConvert.DeserializeObject<T>(SerializeString);

                return tmp;
            }
        }
    }
}
