﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Models;
using ULALA.UI.Core.MVVM;

namespace ULALA.ViewModels
{
    public class WithdrawStackerViewModel : ViewModelBase
    {
        public WithdrawStackerViewModel()
        {

        }

        protected override void OnActivated()
        {
            this.PageIcon = "../Assets/Icons/Cobrar.png";
        }
    }
}
