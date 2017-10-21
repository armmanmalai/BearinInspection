using BearingInspectionCore;
using com.oams.connector;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BearingInspectionAppTest
{
    public partial class Form1 : Form
    {

        private static readonly ILog log = LogManager.GetLogger("OAMS");
        BearingInspectionSequence seq = new BearingInspectionSequence(new BearingInspectionContextMockUp());
       // BearingInspectionSequence seq = new BearingInspectionSequence(new BearingInspectionContextMockUp());
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            seq.onComplete = new TestBearingInspectionCompletedEvent(this);
            seq.onStart = new TestBearingInspectionStartEvent(this);
            seq.onMeasure = new TestBearingInspectionProcessEvent(this);
            seq.onError = new TestBearingInspectionErrEvent(this);
            Task.Factory.StartNew(new Action(this.appthread));
        }

        public void appthread()
        {
            while (true)
            {
                
                seq.prg_init();
                seq.prg_main();
                //Thread.Sleep(1);
                //log.Info("run " + seq.prgSeq);
               // log.Info("last Value " + seq.lastValue);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = seq.lastValue + "";
            textBox2.Text = seq.loopCount + "";
            button1.Text = pt;
        }
        string pt = "";
        public void setButtonBoxX(string t)
        {
            pt = t;
        }
    }
}
