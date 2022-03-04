using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus
{
    public interface IZeusException
    {
        //Global exception catcher

        void SendException(Exception e);
       
    }
}
