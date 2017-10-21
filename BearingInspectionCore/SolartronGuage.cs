using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.oams.connector
{
    public class SolartronGuage
    {
        public static string OVER_VALUE = "hl";
        public static string UNDER_VLAUE = "ul";
        public static string OK_VALUE = "ok";
        public static string NA_VALUE = "na";
    }

    public class DpValue
    {
        public Decimal value { get; set; }
        public Decimal stroke { get; set; }
        public string status { get; set; }
        public string serialId { get; set; }
        public string unit { get; set; }
        public int channelInd { get; set; }
        public Decimal preset { get; set; }
    }



}
