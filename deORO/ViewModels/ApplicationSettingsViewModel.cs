using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp.Models;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class ApplicationSettingsViewModel : BaseViewModel
    {
        private Dictionary<string, List<KeyValue>> list = new Dictionary<string, List<KeyValue>>();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public ICommand SaveCommand { get { return new DelegateCommandWithParam(ExecuteSaveCommand); } }

        private void ExecuteSaveCommand(object parameter)
        {
            if (ConfigFile.SaveSettings(parameter.ToString(), list[parameter.ToString()]))
            {
                aggregator.GetEvent<EventAggregation.ConfigurationSettingsSaveSuccessfulEvent>().Publish(null);
            }
        }

        public Dictionary<string, List<KeyValue>> List
        {
            get { return list; }
            set
            {
                list = value;
                RaisePropertyChanged(() => List);
            }
        }

        //public ApplicationSettingsViewModel()
        //{
        //    this.List = Global.ApplicationSettings;
        //}

        public override void Init()
        {
            this.List = Global.ApplicationSettings;
            base.Init();
        }
    }
}
