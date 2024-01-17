using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using FluentAssertions;
using MudBlazor.Docs.Models;
using NUnit.Framework;
using MudBlazor.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Logging;
using static Bunit.ComponentParameterFactory;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using MudBlazor.UnitTests.TestComponents;
using Bunit;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class StaticRendererTests : BunitTest
    {
        [Test]
        public async Task RenderToHTML()
        {
            string ingoreAttribute = "__internal_stopPropagation_onclick";

            var renderer = new StaticRenderer(Context.Services, Context.Services.GetService<ILoggerFactory>());
            
            //Compare component render to bunit markup no parameters
            var comp = Context.RenderComponent<MudButton>();
            string html = (await renderer.GetHtml<MudButton>()).Replace(ingoreAttribute, "");
            html.MarkupMatches(comp.Markup);

            var paramDict = new Dictionary<string, object>
            {
                            { nameof(MudButton.Disabled), true }
            };
            var parameters = ParameterView.FromDictionary(paramDict);

            //Compare component render to bunit markup with parameters
            comp = Context.RenderComponent<MudButton>(pa => pa.Add(p => p.Disabled, true));
            html = (await renderer.GetHtml<MudButton>(parameters)).Replace(ingoreAttribute, "");
            html.MarkupMatches(comp.Markup);

            //Compare RenderFragment to bunit markup
            RenderFragment fragment = builder =>
            {
                builder.OpenComponent<MudButton>(1);
                builder.AddComponentParameter(2, nameof(MudButton.Disabled), true);
                builder.CloseComponent();
            };

            html = (await renderer.GetHtml(fragment)).Replace(ingoreAttribute, ""); ;
            html.MarkupMatches(comp.Markup);


            //Compare RenderFragment<RenderFragment> to bunit markup
            RenderFragment<RenderFragment> fragment2 = fragment =>
            {
                return fragment;        
            };

            html = (await renderer.GetHtml(fragment2, fragment)).Replace(ingoreAttribute, ""); ;
            html.MarkupMatches(comp.Markup);
        }
    }
}
