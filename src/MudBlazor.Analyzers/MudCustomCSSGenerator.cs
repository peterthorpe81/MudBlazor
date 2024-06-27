// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MudBlazor.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MudCustomCSSGenerator : DiagnosticAnalyzer
    {
        public const string DiagnosticId1 = "MUD0003";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(Resources.MUD0001Title), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _parameterMessageFormat = new LocalizableResourceString(nameof(Resources.MUD0001MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _attributeMessageFormat = new LocalizableResourceString(nameof(Resources.MUD0002MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(Resources.MUD0001Description), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableResourceString _url = new(nameof(Resources.MUD0001Url), Resources.ResourceManager, typeof(Resources));

        private const string Category = "CSS";
        public const string CustomCSSBuildPathProperty = "build_property.mudcustomcssbuildpath";
        //public const string CustomCSSOutputPathProperty = "build_property.mudcustomcssoutputpath";

        public static readonly DiagnosticDescriptor ParameterDescriptor = new(DiagnosticId1, _title, _parameterMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: _description, helpLinkUri: _url.ToString());
    
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get => [ParameterDescriptor]; }

        
        private readonly HashSet<string> UsedTypes = new HashSet<string>();

        public int IndexOfAny(string test, IEnumerable<string> values)
        {
            foreach (string item in values)
            {
                int i = test.IndexOf(item);
                if (i >= 0)
                    return i;
            }
            return -1;
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(ctx =>
            {
                var global = ctx.Options.AnalyzerConfigOptionsProvider.GlobalOptions;

                if (global.TryGetValue(MudComponentUnknownParametersAnalyzer.DebugAnalyzerProperty, out var debugValue) &&
                    bool.TryParse(debugValue, out var shouldDebug) && shouldDebug)
                {
                    Debugger.Launch();
                }

                if (global.TryGetValue(CustomCSSBuildPathProperty, out string? customCSSValue) && string.IsNullOrWhiteSpace(customCSSValue))
                {
                    return;
                }

                /*if (global.TryGetValue(CustomCSSOutputPathProperty, out string? customCSSOutputPath) && string.IsNullOrWhiteSpace(customCSSOutputPath))
                {
                    return;
                }*/

                var analyzerContext = new AnalyzerContext(ctx.Compilation, UsedTypes);

                if (analyzerContext.IsValid)
                {
                    ctx.RegisterOperationAction(analyzerContext.AnalyzeBlockOptions, OperationKind.Block);
                    ctx.RegisterCompilationEndAction(ctx => {
                        var files = ctx.Options.AdditionalFiles.Where(x => x.Path.Contains("MudBlazor.scss"));
                        var file = files.FirstOrDefault();
                        if (file is null)
                            return;

                        //PrepareResources

                        Console.WriteLine(UsedTypes.Count);
                        // Write the string array to a new file named "WriteLines.txt".
                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(customCSSValue, "MudBlazorOptimized.scss")))
                        {
                            string componentImportString = "@import 'components/_";
                            int componentImportStringLength = "@import 'components/_".Length;

                            outputFile.WriteLine($"//{DateTime.Now}");
                            foreach (var line in file.GetText()!.Lines)
                            {
                                string ln = line.ToString();
                                if (ln.Contains(componentImportString))
                                {
                                    // var tmp = ln.IndexOf(componentImportString) + componentImportStringLength;
                                    //ln.IndexOfAny()
                                    if (IndexOfAny(ln, UsedTypes) > -1)
                                        outputFile.WriteLine(line);
                                    else
                                        outputFile.WriteLine($"//{line}");
                                }
                                else
                                {
                                    outputFile.WriteLine(line);
                                }
                            }
                            // foreach (string line in lines)
                            //   outputFile.WriteLine(line);
                        }
                    });
                }

            });
        }

        private sealed class AnalyzerContext
        {
            private readonly IEqualityComparer<ISymbol?> _symbolComparer = new MetadataSymbolComparer();
            private readonly ConcurrentDictionary<ITypeSymbol, ComponentDescriptor> _componentDescriptors = new(SymbolEqualityComparer.Default);
            private readonly HashSet<string> _usedTypes;
            private readonly INamedTypeSymbol? _componentBaseSymbol;
            private readonly INamedTypeSymbol? _parameterSymbol;
            private readonly INamedTypeSymbol? _renderTreeBuilderSymbol;
            private readonly INamedTypeSymbol? _mudComponentBaseType;

            public AnalyzerContext(Compilation compilation, HashSet<string> usedTypes)
            {
                _usedTypes = usedTypes;
                _componentBaseSymbol = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.ComponentBase");
                _parameterSymbol = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.ParameterAttribute");
                _renderTreeBuilderSymbol = compilation.GetBestTypeByMetadataName("Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder");
                _mudComponentBaseType = compilation.GetBestTypeByMetadataName("MudBlazor.MudComponentBase");
            }

            public bool IsValid => _componentBaseSymbol is not null && _parameterSymbol is not null && _renderTreeBuilderSymbol is not null && _mudComponentBaseType is not null;

            public void AnalyzeBlockOptions(OperationAnalysisContext context)
            {
                try
                {
                    var classSymbol = context.Operation.GetClassSymbol(context);
                    if (classSymbol is not null && classSymbol.IsOrInheritFrom(_componentBaseSymbol, _symbolComparer))
                        TraverseTree(context, (IBlockOperation)context.Operation, classSymbol.ToDisplayString());
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            public void TraverseTree(OperationAnalysisContext context, IBlockOperation operations, string className)
            {
                ITypeSymbol? currentComponent = null;

                foreach (var operation in operations.Operations)
                {
                    if (operation is IExpressionStatementOperation expressionStatement)
                    {
                        if (expressionStatement.Operation is IInvocationOperation invocation)
                        {
                            var targetMethod = invocation.TargetMethod;

                            if (targetMethod.ContainingType.IsEqualTo(_renderTreeBuilderSymbol))
                            {
                                if (string.Equals(targetMethod.Name, "OpenComponent", StringComparison.Ordinal) && targetMethod.TypeArguments.Length == 1)
                                {
                                    var componentType = targetMethod.TypeArguments[0];
                                    if (componentType.IsOrInheritFrom(_mudComponentBaseType))
                                    {
                                        currentComponent = componentType;

                                        if (currentComponent is INamedTypeSymbol namedSymbol)
                                        {
                                            INamedTypeSymbol? currentType = namedSymbol;

                                            do
                                            {
                                                _usedTypes.Add(currentType.Name.Substring(3).ToLowerInvariant());
                                                currentType = currentType?.BaseType;
                                            } while (currentType is not null);
                                        }
                                       // 
                                      // _usedTypes.GetOrAdd(currentComponent, ComponentDescriptor.GetComponentDescriptor(componentType, _parameterSymbol));
                                    }
                                }
                                else if (string.Equals(targetMethod.Name, "CloseComponent", StringComparison.Ordinal))
                                {
                                    currentComponent = null;
                                }
                            }
                            else if (string.Equals(targetMethod.ContainingType.MetadataName, "TypeInference", StringComparison.Ordinal))
                            {
                                var methods = context.FilterTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();
                                var method = methods?.Where(x => x.Identifier.ValueText == targetMethod.MetadataName).SingleOrDefault();

                                if (method is not null)
                                {
                                    var op = context.Compilation.GetSemanticModel(method.SyntaxTree).GetOperation(method);
                                    if (op is not null)
                                    {
                                        var blockOperation = op.ChildOperations.OfType<IBlockOperation>().Single();
                                        TraverseTree(context, blockOperation, className);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
