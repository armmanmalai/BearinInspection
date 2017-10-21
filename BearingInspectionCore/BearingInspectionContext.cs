using BearingInspectionCore;
using log4net;
using Solartron.Orbit3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.oams.connector
{
    public class BearingInspectionContextMockUp : BaseBearingInspectionContextSolartron
    {
        private  SolartronMockUp form = new SolartronMockUp();

        public BearingInspectionContextMockUp()
        {
            form.Show();
            connected = true;
            ready = true;
        }

        public override void Connect()
        {
            log.Debug("Connected");
        }

        public override void disconnect()
        {
            log.Debug("Connected");
        }

        public override DpValue getDP(string modname)
        {
            

            return form.dp ;
        }

        string[] modList = { "001", "002" };
        public override string[] getModList()
        {
            return modList;
        }

        public override bool isConnected()
        {
            return true;
        }
    }

    public class BearingInspectionContext : BaseBearingInspectionContextSolartron
    {
        public override void Connect()
        {
            log.Info("Connect orbit Start");
            if (orbit == null)
            {
                log.Info("Instatinate orbit object");
                orbit = new OrbitServer();
            }

            log.Info("Open Orbit Server");
            connected = false;
            ready = false;

            try
            {
                log.Info("orbit.Connect()");
                orbit.Connect();
                connected = orbit.Connected;
                log.Info("connected = " + orbit.Connected);
                if (connected)
                {
                    log.Info("Orbit Server connected");
                    if (orbit.Networks.Count > 0)
                    {
                        log.Info("Open Orbit Server Found Network " + orbit.Networks.Count);
                        log.Info("Open Orbit Server Found Network " + orbit.Networks[0].Modules.Ping());
                        this.ready = true;
                        log.Info("this.ready = " + this.ready);
                    }
                    else
                    {
                        this.ready = false;
                        log.Info("Open Orbit Server Not Found Network ");
                    }
                }
                else
                {
                    log.Info("Open Orbit Server not connected");
                }
            }
            catch (Exception e1)
            {
                this.ready = false;
                log.Error(e1);
            }
        }

        public override void disconnect()
        {
            log.Info("Disconnect Orbit");
            read_error = false;
            try
            {
                log.Info("Disconnect Orbit Already");
                orbit.Disconnect();
                orbit.Dispose();
                connected = false;
                ready = false;
            }
            catch (Exception e)
            {
                log.Info(e);
            }
            connected = false;
            ready = false;
        }

        public override bool isConnected()
        {
            try
            {
                return this.orbit.Connected;
            }
            catch (Exception e3)
            {
                return false;
            }

        }

        public override string[] getModList()
        {
            log.Info("getModList");
            try
            {
                string[] ret = new string[orbit.Networks[0].Modules.Count];

                for (int indexer = 0; indexer < ret.Length; indexer++)
                {
                    log.Debug("Found Module:" + indexer + ":" + orbit.Networks[0].Modules[indexer].ModuleID);
                    ret[indexer] = orbit.Networks[0].Modules[indexer].ModuleID;
                }
                return ret;

            }
            catch (Exception emod)
            {
                log.Error(emod);
                return new string[0];
            }

        }

        public override DpValue getDP(string modname)
        {
            log.Debug("getDP");
            log.Debug("modname = " + modname);
            DpValue ret = new DpValue();
            ret.serialId = modname;
            if (String.IsNullOrEmpty(modname))
            {
                ret.value = 0;
                ret.status = "null";

            }

            try
            {
                read_error = false;
                if (this.ready)
                {
                    OrbitModule bmod = orbit.Networks[0].Modules.GetModuleByID(modname);

                    if (bmod == null)
                    {
                        ret.value = 0;
                        ret.status = "notfound";

                    }
                    log.Debug(bmod);

                    Decimal chval = (decimal)bmod.ReadingInUnits;
                    Decimal storke = (decimal)bmod.Stroke;
                    ret.stroke = storke;
                    ret.value = chval;
                    ret.unit = bmod.UnitsOfMeasure;
                    ret.channelInd = bmod.ChannelIndex;
                    if (bmod.ModuleStatus.Error == eOrbitErrors.OverRange)
                    {
                        ret.status = SolartronGuage.OVER_VALUE;
                    }
                    else if (bmod.ModuleStatus.Error == eOrbitErrors.UnderRange)
                    {
                        ret.status = SolartronGuage.UNDER_VLAUE;
                    }
                    else
                    {
                        ret.status = SolartronGuage.OK_VALUE;
                    }

                }
                else
                {
                    ret.value = 0;
                    ret.status = SolartronGuage.NA_VALUE;
                }
            }
            catch (Exception eread)
            {
                //read_error = true;
                log.Error(eread);
                ret.value = 0;
                ret.status = "err " + eread.Message + eread.GetType();
                throw eread;
            }
            return ret;
        }
    }

    public abstract class BaseBearingInspectionContextSolartron
    {
        public static readonly ILog log = LogManager.GetLogger("BICCON");

        public OrbitServer orbit { get; set; }
        public bool connected { get; set; }
        public bool ready { get; set; }

        public int netNumber = -1;
        public int moduleCount = -1;
        public int moduleErrCount = 0;
        public string errorMsg = "";
        public bool readFlag { get; set; }
        public bool read_error;

        public abstract void Connect();


        public abstract void disconnect();

        public abstract string[] getModList();

        public abstract bool isConnected();

        public abstract DpValue getDP(string modname);
        

    }
}
