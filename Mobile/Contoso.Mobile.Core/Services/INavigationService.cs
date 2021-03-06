﻿using Contoso.Mobile.Core.Models;
using Contoso.Mobile.Core.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Contoso.Mobile.Core.Services
{
    public interface INavigationService
    {
        INavigation Navigation { get; }
        Task NavigateToAsync<T>(BaseModel model = null) where T : BaseViewModel;
        Task AccountSignInAsync(string id = null);
        Task AccountSignUpAsync(string id = null);
        Task HomeAsync();
    }
}