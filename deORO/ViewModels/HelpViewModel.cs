using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class HelpViewModel : BaseViewModel
    {
        private HelpRepository repo = new HelpRepository();

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(() => Title); }
        }

        private string helpText;
        public string HelpText
        {
            get { return helpText; }
            set { helpText = value; RaisePropertyChanged(() => HelpText); }
        }

        public Uri videoPath;
        public Uri VideoPath
        {
            get { return videoPath; }
            set { videoPath = value; RaisePropertyChanged(() => VideoPath); }
        }

        private bool helpTextVisible;

        public bool HelpTextVisible
        {
            get { return helpTextVisible; }
            set { helpTextVisible = value; RaisePropertyChanged(() => HelpTextVisible); }
        }

        public override void Init()
        {
            LoadTitleHelpText(Helpers.Global.CurrentViewModel.ToString());
        }

        public void LoadTitleHelpText(string key)
        {
            var help = repo.GetHelp(key);

            if (help != null)
            {
                try
                {
                    Title = help.title;
                    HelpText = help.help_text;
                    VideoPath = new Uri(Helpers.Global.VideosPath + "\\" + help.video);

                    if (helpText == "")
                        HelpTextVisible = false;
                    else
                        HelpTextVisible = true;
                }
                catch{}
            }
            else
            {
                Title = "";
                HelpText = "";
                VideoPath = null;
                HelpTextVisible = false;
            }
        }

        public override void Dispose()
        {
        }
    }
}
