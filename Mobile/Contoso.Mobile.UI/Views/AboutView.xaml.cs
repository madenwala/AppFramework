﻿using Contoso.Mobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Contoso.Mobile.UI.Views
{
    public partial class AboutView : BaseView
    {
        public AboutView()
        {
            InitializeComponent();
            this.ViewModel = new AboutViewModel();
        }
    }
}