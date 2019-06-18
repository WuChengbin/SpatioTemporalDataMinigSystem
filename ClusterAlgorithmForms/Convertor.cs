using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithmForms
{
    class Convertor
    {
       


        public static byte[] ConvertWKTToWKB(string wkt)
        {
            WKBWriter writer = new WKBWriter();
            WKTReader reader = new WKTReader();
            return writer.Write(reader.Read(wkt));
        }

        public static string ConvertWKBToWKT(byte[] wkb)
        {
            WKTWriter writer = new WKTWriter();
            WKBReader reader = new WKBReader();
            return writer.Write(reader.Read(wkb));
        }
  

    }
}
