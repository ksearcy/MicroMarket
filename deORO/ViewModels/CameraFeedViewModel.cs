using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.ViewModels
{
    public class CameraFeedViewModel : BaseViewModel
    {
        public string camera1Uri = deORO.Helpers.Global.Camera1;
        public string camera2Uri = deORO.Helpers.Global.Camera2;
        public string camera3Uri = deORO.Helpers.Global.Camera3;
        public string camera4Uri = deORO.Helpers.Global.Camera4;

        bool camera1Visible = false;
        bool camera2Visible =false;

        public bool Camera1Visible
        {
            get { return camera1Visible; }
            set { camera1Visible = value; RaisePropertyChanged(() => Camera1Visible); }
        }

        public bool Camera2Visible
        {
            get { return camera2Visible; }
            set { camera2Visible = value; RaisePropertyChanged(() => Camera2Visible); }
        }

        public CameraFeedViewModel()
        {
            if (!camera1Uri.Equals(""))
                Camera1Visible = true;

            if (!camera2Uri.Equals(""))
                Camera2Visible = true;

        }

        public override void Init()
        {
            base.Init();
        }
    }
}
