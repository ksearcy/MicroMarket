using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class FileTransferViewModel : BaseViewModel
    {
        public ICommand CopyCommand { get { return new DelegateCommand(ExecuteCopyCommand); } }
        public ICommand SourceRefreshCommand { get { return new DelegateCommand(ExecuteSourceRefreshCommand); } }
        public ICommand DestinationRefreshCommand { get { return new DelegateCommand(ExecuteDestinationRefreshCommand); } }
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        List<Drive> drives = new List<Drive>();
        Drive selectedDrive = new Drive();
        List<string> errors = new List<string>();

        public Drive SelectedDrive
        {
            get { return selectedDrive; }
            set { selectedDrive = value; RaisePropertyChanged(() => SelectedDrive); }
        }

        ObservableCollection<FileSystemObject> destinationList = new ObservableCollection<FileSystemObject>();

        public ObservableCollection<FileSystemObject> DestinationList
        {
            get { return destinationList; }
            set { destinationList = value; RaisePropertyChanged(() => DestinationList); }
        }

        private string source;

        public string Source
        {
            get { return source; }
            set { source = value; RaisePropertyChanged(() => Source); }
        }

        ObservableCollection<FileSystemObject> sourceList = new ObservableCollection<FileSystemObject>();

        public ObservableCollection<FileSystemObject> SourceList
        {
            get { return sourceList; }
            set { sourceList = value; RaisePropertyChanged(() => SourceList); }
        }

        public List<Drive> Drives
        {
            get { return drives; }
            set { drives = value; RaisePropertyChanged(() => Drives); }
        }

        public override void Init()
        {
            LoadDrives();

            try
            {
                Source = Global.CopyFrom;
            }
            catch { }
            base.Init();
        }

        private void LoadDrives()
        {
            DriveInfo.GetDrives().ToList().ForEach(x =>
                {
                    Drives.Add(new Drive { IsSelected = false, Name = x.Name, Type = x.DriveType.ToString() });
                });
        }

        private void ExecuteDestinationRefreshCommand()
        {
            DestinationList.Clear();
            try
            {
                System.IO.Directory.GetDirectories(selectedDrive.Name).ToList().ForEach(x =>
                    {
                        DestinationList.Add(new FileSystemObject { Name = Path.GetFileName(x), Type = "Directory", FullPath = x });
                    });

                System.IO.Directory.GetFiles(selectedDrive.Name).ToList().ForEach(x =>
                    {
                        DestinationList.Add(new FileSystemObject { Name = Path.GetFileName(x), Type = "File", FullPath = x, Size = (new FileInfo(x).Length / 1024).ToString("N") + " KB" });
                    });
            }
            catch { }
        }

        private void ExecuteSourceRefreshCommand()
        {
            SourceList.Clear();

            try
            {
                System.IO.Directory.GetDirectories(source.ToString()).ToList().ForEach(x =>
                    {
                        SourceList.Add(new FileSystemObject { Name = Path.GetFileName(x), Type = "Directory", FullPath = x, IsSelected = true });
                    });

                System.IO.Directory.GetFiles(source.ToString()).ToList().ForEach(x =>
                    {
                        SourceList.Add(new FileSystemObject { Name = Path.GetFileName(x), Type = "File", FullPath = x, IsSelected = true, Size = (new FileInfo(x).Length / 1024).ToString("N") + " KB" });
                    });
            }
            catch { }

        }

        private void ExecuteCopyCommand()
        {
            if (selectedDrive.Name == null)
                return;

            if (SourceList.Where(x => x.IsSelected).Count() == 0)
                return;

            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.RunWorkerAsync();

            BusyViewModel vm = new BusyViewModel();
            vm.Message = "Copying files. Please wait...";
            DialogViewService.ShowDialog(vm, 225, 100);

        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);

            App.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var o in SourceList.Where(x => x.IsSelected))
                    {
                        try
                        {
                            if (o.Type == "Directory")
                            {
                                DirectoryCopy(o.FullPath, selectedDrive.Name + "\\" + o.Name, true);
                            }
                            else if (o.Type == "File")
                            {
                                System.IO.File.Copy(o.FullPath, selectedDrive.Name + "\\" + o.Name);
                            }
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex.Message.ToString());
                        }
                    }

                    ExecuteDestinationRefreshCommand();
                });
        }

        void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

            if (errors.Count > 0)
            {
                StringBuilder builder = new StringBuilder();

                foreach (var error in errors)
                {
                    builder.AppendLine(error);
                }

                DialogViewService.ShowAutoCloseDialog("Errors", builder.ToString());
            }
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                try
                {
                    file.CopyTo(temppath, false);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message.ToString());
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    try
                    {
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message.ToString());
                    }
                }
            }
        }

        public override void Dispose()
        {
            drives.Clear();
            errors.Clear();
            sourceList.Clear();
            destinationList.Clear();
            SelectedDrive = null;

            base.Dispose();
        }
    }

    public class FileSystemObject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string FullPath { get; set; }
        public bool IsSelected { get; set; }
        public string Size { get; set; }
    }

    public class Drive
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsSelected { get; set; }
    }
}
