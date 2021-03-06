﻿using Contoso.Mobile.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Contoso.Mobile.UI.Views
{
    public partial class ShellView : Shell
    {
        public ShellView()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(FolderViewModel), typeof(FolderView));
            Routing.RegisterRoute(nameof(NoteViewModel), typeof(NoteView));
        }
    }
}