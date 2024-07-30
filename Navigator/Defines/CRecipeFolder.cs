using NavigatorMachine.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace NavigatorMachine.Defines
{
    public static class CRecipeFolder
    {
        public static string DirectoryPGM = @"D:\PowerlogicsBaThien\History\";
        public static string DirectoryPGM1 = @"D:\PowerlogicsBaThien\";

        public static string RecipeFolder
        {
            get
            {
                string Recipepath = Path.Combine(DirectoryPGM1, "Recipe");
                if (!Directory.Exists(Recipepath))
                {
                    Directory.CreateDirectory(Recipepath);
                }
                return Recipepath;
            }
        }

        public static string DataFolder
        {
            get
            {
                string Datapath = Path.Combine(DirectoryPGM, "Data");
                if (!Directory.Exists(Datapath))
                {
                    Directory.CreateDirectory(Datapath);
                }
                return Datapath;
            }
        }
        public static string PictureFolder
        {
            get
            {
                string Picturepath = Path.Combine(DirectoryPGM, "Picture");
                if (!Directory.Exists(Picturepath))
                {
                    Directory.CreateDirectory(Picturepath);
                }
                return Picturepath;
            }
        }
        public static string LOGFolder
        {
            get
            {
                string Logpath = Path.Combine(DirectoryPGM, "LOG_file");
                if (!Directory.Exists(Logpath))
                {
                    Directory.CreateDirectory(Logpath);
                }
                return Logpath;
            }
        }

        public static string LogFileFolder
        {
            get
            {
                string date = DateTime.Now.ToString("yyyy_MM_dd");
                string LogFilepath = Path.Combine(LOGFolder, $"{date}_log.txt");
                if (!File.Exists(LogFilepath))
                {
                    File.Create(LogFilepath).Close();
                }
                return LogFilepath;
            }
        }
        public static string RecipeInfoFile = Path.Combine(RecipeFolder, "recipe.json");
		public static string CurrentRecipeFolder
		{
			get
			{
                if (!Directory.Exists(Path.Combine(RecipeFolder, CurrentRecipe.Name)))
                {
                    Directory.CreateDirectory(Path.Combine(RecipeFolder, CurrentRecipe.Name));
                }
                return Path.Combine(RecipeFolder, CurrentRecipe.Name);
            }
		}

        public static CRecipeInfo CurrentRecipe
        {
            get
            {
                string recipeInitPath = Path.Combine(RecipeFolder, RecipeInfoFile);

                if (!File.Exists(recipeInitPath))
                {
                    CRecipeInfo defaultRecipeInfo = new CRecipeInfo();

                    Directory.CreateDirectory(Path.GetDirectoryName(recipeInitPath));

                    using (StreamWriter sw = File.AppendText(recipeInitPath))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(defaultRecipeInfo, Formatting.Indented));
                    }
                }

                string currentRecipeInfoString = File.ReadAllText(recipeInitPath);

                CRecipeInfo currentRecipeInfo = JsonConvert.DeserializeObject<CRecipeInfo>(currentRecipeInfoString);

                if (currentRecipeInfo == null)
                {
                    currentRecipeInfo = new CRecipeInfo();

                    Directory.CreateDirectory(Path.GetDirectoryName(recipeInitPath));

                    using (StreamWriter sw = File.AppendText(recipeInitPath))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(currentRecipeInfo, Formatting.Indented));
                    }
                }

                return currentRecipeInfo;
            }
            set
            {
                string recipeInitPath = Path.Combine(RecipeFolder, RecipeInfoFile);

                if (!File.Exists(recipeInitPath))
                {
                    using (StreamWriter sw = File.AppendText(recipeInitPath))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(value, Formatting.Indented));
                    }
                }
                File.WriteAllText(recipeInitPath, JsonConvert.SerializeObject(value, Formatting.Indented));
            }
        }

    }
}
