using BearingInspectionCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.oams.connector;

namespace BearingInspectionAppTest
{
    public class TestBearingInspectionErrEvent : BearingInspectionEvent
    {
        Form1 form;

        public TestBearingInspectionErrEvent(Form1 pform)
        {
            form = pform;
        }

        public override void eventOccur(DpValue dp, decimal lastValue)
        {
            form.setButtonBoxX("Error: "+ dp.value );
        }
    }

    public class TestBearingInspectionStartEvent : BearingInspectionEvent
    {
        Form1 form;

        public TestBearingInspectionStartEvent(Form1 pform)
        {
            form = pform;
        }

        public override void eventOccur(DpValue dp, decimal lastValue)
        {
            form.setButtonBoxX("start");
        }
    }

    public class TestBearingInspectionProcessEvent : BearingInspectionEvent
    {
        Form1 form;

        public TestBearingInspectionProcessEvent(Form1 pform)
        {
            form = pform;
        }

        public override void eventOccur(DpValue dp, decimal lastValue)
        {
            form.setButtonBoxX(dp.value+"");
        }
    }

    public class TestBearingInspectionCompletedEvent : BearingInspectionEvent
    {
        Form1 form;

        public TestBearingInspectionCompletedEvent(Form1 pform)
        {
            form = pform;
        }

        public override void eventOccur(DpValue dp, decimal lastValue)
        {
            form.setButtonBoxX( "value=" + dp.value + ",final=" + lastValue);
        }
    }
}
