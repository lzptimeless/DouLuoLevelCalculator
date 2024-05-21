using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDemo
{
    public interface IPlugin
    {
        string Name { get; }
        public void Print()
        {

        }
    }
}
