using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus;
using ULALA.Services.Contracts.Zeus;
using Unity;
using Windows.UI.Xaml.Controls;

namespace ULALA.Services.Zeus
{


    public enum ErrorCode
    {
        GeneralError = 200,
        ParseError = 1,
        InvalidRequest = 2,
        MethodNotFound = 3,
        InvalidParams = 4,
        InternalError = 5,
        ServerBusy = 6,
        UnknownOperationError = 100,
        InsuficientFunds = 101,
        BillrecyclerNotResponding = 102,
        CoinValidatorNotResponding = 103,
        BillJammed = 104,
        CoinStuck = 105,
        BillStackerFull = 106,
        BillStackerMissing = 107,
        BillrecyclerFailure = 1z,







    }
    internal class ZeusExceptionMaganer : IZeusException
    {
        [Dependency]
        public ILoggerManager logger { get; set; }
       
        public void SendException(Exception e)
        {
            string Date = DateTime.Now.ToString();
            string From = e.TargetSite.ToString();

            string jsonCode = ""; // Error mandado por la maquina // consumir servicio
            ErrorCode codeEnum = ErrorCode.GeneralError;
            string code = codeEnum.ToString();
            string Description = "Codigo general";


            if (jsonCode == "")
            {
                codeEnum = (ErrorCode)System.Enum.Parse(typeof(ErrorCode), jsonCode);
                Description = code + codeEnum;
                logger.WriteException(Date, code, Description);
            }
            else
            {
                logger.WriteException(Date, code, e.Message);
                post(code, Description);
            }
        }

        public async void post(string Title, string Description)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = new InfoBar()
            {
                IsOpen = true,
                IsIconVisible = true,
                IsClosable = false,
                Severity = InfoBarSeverity.Warning,
                Title = "Error: " + Title + "\n" + Description,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
            };
            dialog.PrimaryButtonText = "OK";

            await dialog.ShowAsync();
        }

        }

       
    }

