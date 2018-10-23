using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;

namespace deORO.ViewModels
{
    class AdministationViewModel : BaseViewModel
    {

        public bool IsAdminTabVisible
        {
            get
            {
                if (Global.User != null)
                {
                    if (Global.User.IsAdmin)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }

            }
        }

        public bool IsFingerprintTabVisible
        {
            get
            {
                if (Global.EnableFingerprintAuthentication)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public ICommand ShutdownCommand { get { return new DelegateCommandWithParam((x) => App.Current.Shutdown()); } }
    }
}
