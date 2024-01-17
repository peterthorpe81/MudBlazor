// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MudBlazor.Utilities
{
#nullable enable
    /// <summary>
    /// Allows controls to be rendered to HTML for things like export .NET8+
    /// </summary>
    public class StaticRenderer : IAsyncDisposable, IDisposable
    {
#if NET8_0_OR_GREATER
        HtmlRenderer _htmlRenderer;

        public StaticRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _htmlRenderer = new HtmlRenderer(serviceProvider, loggerFactory);
        }

        public async Task<string> GetHtml<T>(RenderFragment<T> fragment, T value)
        {
            return await GetHtml(fragment.Invoke(value));
        }

        public async Task<string> GetHtml(RenderFragment fragment)
        {
            return await _htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var dictionary = new Dictionary<string, object?>
                        {
                            { nameof(BaseComponentRenderer.ChildContent), fragment }
                        };
                var parameters = ParameterView.FromDictionary(dictionary);
                var output = await _htmlRenderer.RenderComponentAsync<BaseComponentRenderer>(parameters);
                return output.ToHtmlString();
            });
        }

        public async Task<string> GetHtml<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>() where T : IComponent
        {
            return await GetHtml<T>(ParameterView.Empty);
        }

        public async Task<string> GetHtml<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(ParameterView parameters) where T : IComponent
        {
            return await _htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var output = await _htmlRenderer.RenderComponentAsync<T>(parameters);
                return output.ToHtmlString();
            });
        }

        private class BaseComponentRenderer : ComponentBase
        {
            [Parameter]
            public RenderFragment? ChildContent { get; set; }

            protected override void BuildRenderTree(RenderTreeBuilder __builder)
            {
                __builder.AddContent(0, ChildContent);
                base.BuildRenderTree(__builder);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _htmlRenderer.DisposeAsync();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _htmlRenderer?.Dispose();
            }
        }
#else
        public StaticRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) 
        {
        
        }

        public async Task<string> GetHtml<T>(RenderFragment<T> fragment, T value) 
        {
            return await Task.FromResult("Not Supported Until .NET 8");
        }

        public async Task<string> GetHtml(RenderFragment fragment)
        {
            return await Task.FromResult("Not Supported Until .NET 8");
        }
    
        public async Task<string> GetHtml(IComponent component, ParameterView parameters)
        {
            return await Task.FromResult("Not Supported Until .NET 8");
        }        

        public async ValueTask DisposeAsync()
        {
            await Task.Yield();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

#endif

    }
}
