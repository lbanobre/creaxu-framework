using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Creaxu.Blazor.Extensions;

namespace Creaxu.Blazor
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject] private NavigationManager _navigationManager { get; set; }

        [Parameter]
        public string Target { get; set; }
        
        [Parameter]
        public string LoginPage { get; set; }
        
        [Parameter]
        public string ReturnUrl { get; set; }
        
        protected override void OnInitialized()
        {
            if (!_navigationManager.Uri.Contains(Target)) 
                return;
            
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                var returnUrlBase64 = ReturnUrl.EncodeToBase64();
                _navigationManager.NavigateTo($"{LoginPage}/{returnUrlBase64}");
            }
            else
            {
                _navigationManager.NavigateTo($"{LoginPage}");
            }
        }
    }
}
