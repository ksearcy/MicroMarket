using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.Marshall
{
    public interface IState
    {
        void doAction(MarshallMessage message);
    }
    public class MachineContext : IState
    {

        private IState machineState;

        public void setState(IState state)
        {
            this.machineState = state;
        }

        public IState getState()
        {
            return this.machineState;
        }


        public void doAction(MarshallMessage message)
        {
            this.machineState.doAction(message);
        }
    }
}
