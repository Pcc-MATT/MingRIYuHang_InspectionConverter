using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calypsoInterface
{
    public class InputPoint
    {
        public string model { get; set; }
        public string partNo { get; set; }
        public string unit { get; set; }
        public string fileName { get; set; }
        public int pointNo { get; set; }
        public List<Point> pointList { get; set; }
    }
}
