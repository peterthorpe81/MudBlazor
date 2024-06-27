using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Mudblazor.Tasks
{
    public sealed class MudCreateSCSS : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string TempFolder { get; set; }
        [Required]
        public string OutputCssFile { get; set; }
        [Required]
        public string MudBlazorDllPath { get; set; }
        [Required]
        public string SCSSFolder { get; set; }

        private string CurrentScssFile { get => Path.Combine(SCSSFolder, "MudBlazor.scss"); }
        private string OutputScssFile { get => Path.Combine(TempFolder, "MudBlazorOptimized.scss"); }
        private string OutputConfigFile { get => Path.Combine(TempFolder, "sasscompiler.json"); }

        private HashSet<string> _mudBlazorTypes { get; set; } = new HashSet<string>();

        //unsafe to remove
        //private HashSet<string> _excludedTypes { get; set; } = new HashSet<string>("icons';", "typography';");

        //a few component scss files don't match the pattern
        private Dictionary<string, string> _nameMap { get; set; } = new()
        {
            ["chart';"] = "charts';",
            ["datepicker';"] = "pickerdate';",
            ["timepicker';"] = "pickertime';",
            ["colorpicker';"] = "pickercolor';",
            ["snackbar';"] = "snackbarprovider';",
            ["icon';"] = "icons';",
            ["text';"] = "typography';"
        };

        public string GetCssName(Type t)
        {
            string name = t.Name;
            int index = name.IndexOf('`');
            string className = index == -1 ? $"{name.Substring(3).ToLowerInvariant()}';" : $"{name.Substring(3, index - 3).ToLowerInvariant()}';";

            if (_nameMap.ContainsKey(className))
                className = _nameMap[className];

            return className;
        }

        public override bool Execute()
        {
            if (!Debugger.IsAttached)
              Debugger.Launch();


            string[] runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            var paths = new List<string>(runtimeAssemblies);
            paths.Add(MudBlazorDllPath);
            var resolver = new PathAssemblyResolver(paths);
            var context = new MetadataLoadContext(resolver);

            using (context)
            {
                Assembly assembly = context.LoadFromAssemblyPath(MudBlazorDllPath);
                var mudComponentBase = assembly.GetType("MudBlazor.MudComponentBase");
                foreach (TypeInfo t in assembly.GetTypes())
                {
                    try
                    {
                        if (t.IsClass && t.IsSubclassOf(mudComponentBase))
                        {
                            _mudBlazorTypes.Add(GetCssName(t));
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        Console.WriteLine("FileNotFoundException: " + ex.Message);
                    }
                    catch (TypeLoadException ex)
                    {
                        Console.WriteLine("TypeLoadException: " + ex.Message);
                    }
                }
            }

            //write output scss file 
            var file = File.ReadLines(CurrentScssFile);
            string pattern = "@import 'components/_";
            int patternLength = pattern.Length;


            //StringBuilder sb = new StringBuilder();
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(OutputScssFile));
            using (StreamWriter writer = new StreamWriter(OutputScssFile))
            {
                foreach (var line in file)
                {
                    var idx = line.IndexOf(pattern);
                    if (idx > -1 && !_mudBlazorTypes.Contains(line.Substring(idx + patternLength)))
                    {
                        writer.WriteLine($"//{line}");
                    }
                    else
                    {
                        writer.WriteLine(line);
                    }
                }
            }

            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(OutputCssFile));

            string config = @$"{{
  ""Arguments"": ""--style=compressed --no-source-map"",
  ""IncludePaths"": [""{SCSSFolder.Replace("\\", "\\\\")}""],

  ""Compilations"": [
    {{ ""Source"":  ""{OutputScssFile.Replace("\\", "\\\\")}"", ""Target"":  ""{OutputCssFile.Replace("\\", "\\\\")}"" }}
  ]
}}";

            File.WriteAllText(OutputConfigFile, config);

            return true;
        }

    }
}
