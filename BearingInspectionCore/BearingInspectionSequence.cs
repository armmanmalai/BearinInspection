using com.oams.connector;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingInspectionCore
{
    public class BearingInspectionSequence
    {
        public BearingInspectionEvent onStart {get; set;} = new DefaultBearingInspectionEvent("onStart");
        public BearingInspectionEvent onError { get; set; } = new DefaultBearingInspectionEvent("onError");
        public BearingInspectionEvent onComplete { get; set; } = new DefaultBearingInspectionEvent("onComplete");
        public BearingInspectionEvent onMeasure { get; set; } = new DefaultBearingInspectionEvent("onMeasure");

        private BaseBearingInspectionContextSolartron ctx;
        private static readonly ILog log = LogManager.GetLogger("BICSEQ");

        public BearingInspectionSequence(BaseBearingInspectionContextSolartron pctx)
        {
            ctx = pctx;
        }

        bool ctxReady = false;

        public int prgSeq { get; set; } = 0;
        public string prgMsg { get; set; } = "";

        string moduleFound = "";
        DpValue dpValue = new DpValue();
        public void prg_init()
        {
            if (ctxReady && ctx.ready)
            {
                try
                {
                    log.Debug("Orbit Connected");
                    dpValue = ctx.getDP(moduleFound);
                    log.Debug("Read Value = " + dpValue.value);
                }
                catch (Exception e)
                {
                    ctxReady = false;
                    log.Error(e);
                }

            }
            else
            {
                try {
                    ctx.disconnect();
                    log.Info("Try to connect Orbit");
                    ctx.Connect();
                    log.Info("Get First Module to Read");
                    moduleFound = ctx.getModList()[0];
                    log.Info("moduleFound=" + moduleFound);
                    ctxReady = ctx.isConnected();
                }
                catch (Exception e)
                {
                    ctxReady = false;
                    log.Error(e);
                }
            }
        }

        OAMSTimer timer100 = new OAMSTimer(500);
        OAMSTimer timer101 = new OAMSTimer(100);

        public long loopCount { get; set; } = 0;
        public decimal lastValue { get; set; } = new decimal();
        public decimal oldValue { get; set; } = new decimal();
        public void prg_main()
        {
            loopCount++;
            if (ctxReady)
            {

                log.Debug("ctxReady is false");
                switch (prgSeq)
                {
                    case 0:
                        prgMsg = "Waiting for program ready";
                        prgSeq = 100;
                        break;
                    case 100:
                        prgMsg = "Waiting for OK Value";

                        if (dpValue.status == SolartronGuage.OK_VALUE)
                        {
                            onStart.eventOccur(dpValue, lastValue);
                            if (timer100.isOver())
                            {
                                prgSeq = 101;
                                
                            }
                            lastValue = 0;
                        }
                        else {
                            timer100.start();
                        }
                        break;
                    case 101:
                        
                        prgMsg = "Critical Point Detection";
                        if (dpValue.value > lastValue)
                        {
                            lastValue = dpValue.value;                            
                        }
                        if (dpValue.status == SolartronGuage.UNDER_VLAUE)
                        {
                            prgSeq = 102;
                            timer101.start();
                            onComplete.eventOccur(dpValue, lastValue);
                        }
                        else if (dpValue.status == SolartronGuage.OVER_VALUE)
                        {
                            prgSeq = 200;
                            onError.eventOccur(dpValue, lastValue);
                        }
                        else if (dpValue.value != oldValue)
                        {
                            onMeasure.eventOccur(dpValue, lastValue);
                        }
                        oldValue = dpValue.value;
                        break;
                    case 102:                       
                        prgMsg = "Delay for End Detection";
                        if (timer101.isOver())
                        {
                            prgSeq = 100;
                        }
                        break;
                    case 200:
                        if (dpValue.status == SolartronGuage.UNDER_VLAUE)
                        {
                            prgSeq = 100;
                        }
                        break;
                    default:
                        prgSeq = 100;
                        break;
                }
            }
            else {
                log.Debug("ctxReady is false");
                prgSeq = 0;
                prgMsg = "Waiting for program ready";
            }

        }
    }

    public abstract class BearingInspectionEvent
    {
        public string name { get; set; } 
        public abstract void eventOccur(DpValue dp ,decimal lastValue);
    }

    public class DefaultBearingInspectionEvent : BearingInspectionEvent
    {
        private static readonly ILog log = LogManager.GetLogger("DefaultBearingInspectionEvent");
        public DefaultBearingInspectionEvent()
        {

        }
        public DefaultBearingInspectionEvent(string n)
        {
            name = n;
        }
        public override void eventOccur(DpValue dp , decimal lastValue)
        {
            log.Info(name + ": " + dp.value + ":"+ lastValue);
        }
    }
}
