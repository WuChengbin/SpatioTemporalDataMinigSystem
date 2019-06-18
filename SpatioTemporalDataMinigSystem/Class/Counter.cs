using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDMS.Class
{
    class Counter
    {
        private readonly object _syncRoot = new Object();

        public int Count { get; private set; }

        public  void Increment()
        {
            lock (_syncRoot)
            {               
                Count++;
                //Console.WriteLine("当前值:{0}", Count);
            }
        }

        public void Decrement()
        {
            lock (_syncRoot)
            {
                Count--;
            }
        }
    }
}
