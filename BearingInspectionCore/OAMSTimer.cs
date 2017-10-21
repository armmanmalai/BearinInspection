using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingInspectionCore
{
    public class OAMSTimer
    {
        long t_start = 0;
        long t_elapased = 0;
        long t_preset = 0;

        public OAMSTimer(long preset)
        {
            t_preset = preset*10*1000;
        }

        public void start()
        {
            t_start = DateTime.Now.Ticks;
        }

        public bool isOver()
        {
            t_elapased = DateTime.Now.Ticks - t_start ;

            return t_elapased >= t_preset;

        }

    }
}
