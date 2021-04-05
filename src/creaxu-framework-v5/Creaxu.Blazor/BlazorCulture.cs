using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Creaxu.Blazor
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class BlazorCulture : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

        public BlazorCulture(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/Creaxu.Blazor/blazorCulture.js").AsTask());
        }

        public async ValueTask SetBlazorCulture(string value)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("setBlazorCulture", value);
        }
        
        public async ValueTask<string> GetBlazorCulture()
        {
            var module = await _moduleTask.Value;
            
            return await module.InvokeAsync<string>("getBlazorCulture");
        }

        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
