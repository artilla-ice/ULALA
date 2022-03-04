using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus;
using ULALA.Services.Contracts.Zeus;
using Unity;

namespace ULALA.Services.Zeus
{


    public enum ErrorCode
    {
       



    }
    internal class ZeusExceptionMaganer : IZeusException
    {
        [Dependency]
        public ILoggerManager logger { get; set; }
        public ZeusExceptionMaganer()
        {

        }
        public void SendToLogger(Exception e)
        {
            string Date = DateTime.Now.ToString();
           string From = e.TargetSite.ToString();

            string Description = e.Message;
            string result = Date + " " + From + " " + Description;

            string code = "";


            logger.WriteException(Date, code, Description);
        }

       
    }
}
