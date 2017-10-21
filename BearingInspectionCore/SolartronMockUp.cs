using com.oams.connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BearingInspectionCore
{
    public partial class SolartronMockUp : Form
    {
        
        public SolartronMockUp()
        {
            InitializeComponent();
        }

        private void SolartronMockUp_Load(object sender, EventArgs e)
        {

        }

        public DpValue dp{get; set;} = new DpValue();

        private void button1_Click(object sender, EventArgs e)
        {
            
         }
        long count = 0;
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            count++;
            button1.Text = "" + count;
            dp.value = numericUpDown1.Value;
            if (dp.value <= 0)
            {
                dp.status = SolartronGuage.UNDER_VLAUE;
            }
            else if (dp.value >= 50)
            {
                dp.status = SolartronGuage.OVER_VALUE ;
            }
            else
            {
                dp.status = SolartronGuage.OK_VALUE;
            }
        }
    }
}
