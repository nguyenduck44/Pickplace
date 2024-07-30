using NavigatorMachine.Commands;
using NavigatorMachine.Defines;
using NavigatorMachine.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace NavigatorMachine.MVVM.ViewModels
{
    
    public class RecipeChangeViewModel : ObservableObject
    {
        #region Properties
        public ObservableCollection<CRecipeInfo> ListRecipe
		{
			get { return _listRecipe; }
			set
			{ 
				_listRecipe = value;
				OnPropertyChanged();
			}
		}
                
        public CRecipeInfo SelectedRecipeItem
        {
            get { return _selectedRecipeItem; }
            set
            {
                 _selectedRecipeItem = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor (s)
        public RecipeChangeViewModel()
        {
        }
        #endregion

        #region Commands
        public RelayCommand RefeshButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    var ListRecipeName = Directory.GetDirectories(CRecipeFolder.RecipeFolder)
                                              .Select(Path.GetFileName)
                                              .ToList();

                    ListRecipe = new ObservableCollection<CRecipeInfo>();

                    foreach (string recipe in ListRecipeName)
                    {
                        ListRecipe.Add(new CRecipeInfo { Name = recipe });
                    }
                });
            }
        }

        public RelayCommand ChangeButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (SelectedRecipeItem == null)
                    {
                        MessageBox.Show("Select recipe to change!", caption: "Notice");
                        return;
                    }
                    if (SelectedRecipeItem.Name == CRecipeFolder.CurrentRecipe.Name)
                    {
                        MessageBox.Show("This recipe is being used!", caption: "Notice");
                        return;
                    }

                    var result = MessageBox.Show($"Do you want change recipe: \n {CRecipeFolder.CurrentRecipe.Name} => {SelectedRecipeItem.Name}", "Caption", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        UILog.Debug($"Changed recipe: {CRecipeFolder.CurrentRecipe.Name} => {SelectedRecipeItem.Name}");

                        CRecipeFolder.CurrentRecipe = SelectedRecipeItem;
                        _initializeVM.Initialize();
                    }
                });
            }
        }

        public RelayCommand CopyButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (SelectedRecipeItem == null)
                    {
                        MessageBox.Show("Please select Recipe! ");
                        return;
                    }

                    var result = MessageBox.Show($"Do you want Copy Recipe: \"{SelectedRecipeItem.Name}\" ?", "Caption", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        UILog.Debug($"Copied recipe: {SelectedRecipeItem.Name}");

                        string selectedRecipe = SelectedRecipeItem.Name;

                        string currentRecipePath = Path.Combine(CRecipeFolder.RecipeFolder, selectedRecipe);
                        string newRecipe = $"{selectedRecipe} - Copy";
                        int folderIndex = 2;
                        while (Directory.Exists(Path.Combine(CRecipeFolder.RecipeFolder, newRecipe)))
                        {
                            newRecipe = $"{selectedRecipe} - Copy ({folderIndex++})";
                        }
                        string newRecipePath = Path.Combine(CRecipeFolder.RecipeFolder, newRecipe);

                        CopyDirectory(sourceDir: currentRecipePath, destinationDir: newRecipePath, recursive: true);

                        RefeshButtonCommand.Execute(null);
                        SelectedRecipeItem = ListRecipe.First(r => r.Name == newRecipe);
                        ChangeButtonCommand.Execute(o);
                    }
                });
            }
        }

        public RelayCommand DeleteButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (SelectedRecipeItem == null) return;

                    if (SelectedRecipeItem.Name.Equals(CRecipeFolder.CurrentRecipe.Name))
                    {
                        MessageBox.Show("This recipe is being used!", caption: "Warning");
                        return;
                    }

                    if (ListRecipe.Count == 1) return;

                    var result = MessageBox.Show($"Do you want Delete recipe: \"{SelectedRecipeItem.Name}\"? ", "Caption", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        UILog.Debug($"Deleted recipe: \"{SelectedRecipeItem.Name}\"");

                        string pathDelete = Path.Combine(CRecipeFolder.RecipeFolder, SelectedRecipeItem.Name);
                        
                        try
                        {
                            Directory.Delete(pathDelete, true);
                            ListRecipe.Remove(SelectedRecipeItem);
                        }
                        catch
                        {
                            UILog.Debug($"Deleted recipe: \"{SelectedRecipeItem.Name}\" Fail!");
                        }

                    }
                });
            }
        }

        public RelayCommand OpenRecipeFolderButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    Process.Start(new ProcessStartInfo { FileName = CRecipeFolder.RecipeFolder, UseShellExecute = true });
                });
            }
        }
        #endregion

        #region Methods
        private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
        #endregion

        #region Privates
        private ObservableCollection<CRecipeInfo> _listRecipe;
        private CRecipeInfo _selectedRecipeItem = new CRecipeInfo();

        private InitializeViewModel _initializeVM => CDef.MainWindowVM.InitializeVM;
        #endregion
    }
}
