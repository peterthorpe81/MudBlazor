using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        //$([System.IO.Path]::GetFullPath('$(MSBuildThisFileDirectory)\your\path'))


        //<MudCreateScss NugetFolder="$(PkgMudblazor)" TempFolder="$(IntermediateOutputPath)"
        // MudBlazorDllPath="$(OutputPath)/Mudblazor.dll"
        // OutputCssFile="$(OutputPath)/wwwroot/css/MudBlazorOptimized.scss" />

        [Required]
        public string TempFolder { get; set; }
        [Required]
        public string OutputCssFile { get; set; }
        [Required]
        public string MudBlazorDllPath { get; set; }
        [Required]
        public string NugetFolder { get; set; }


        private string NugetCssFolder { get => Path.Combine(NugetFolder, "tools/Styles/MudBlazor.scss"); }
        private string OutputScssFile { get => Path.Combine(NugetFolder, "MudBlazorOptimized.scss"); }
        private string OutputConfigFile { get => Path.Combine(NugetFolder, "sasscompiler.config"); }
        
        private HashSet<string> _mudBlazorTypes { get; set; }
               

        // [Output]
        //public ITaskItem[] GeneratedFiles { get; set; } = Array.Empty<ITaskItem>();

        public override bool Execute()
        {
            if (!Debugger.IsAttached)
                Debugger.Launch();

            //get mudblazor components in trimmed assembly
            var assembly = Assembly.LoadFile(MudBlazorDllPath);

            if (assembly == null)
                return false;

            var mudComponentBase = assembly.GetType("MudBlazor.MudComponentBase");
            
            foreach (var t in assembly.GetTypes())
            {
                if (t.IsSubclassOf(mudComponentBase))
                    _mudBlazorTypes.Add($"{t.Name.Substring(3).ToLowerInvariant()};");
            }

            Log.LogMessage(MessageImportance.High, "Aloha");
            //write output scss file 
            var file = File.ReadLines(NugetCssFolder);
            string pattern = "@import 'components/_";
            int patternLength = pattern.Length;


            //StringBuilder sb = new StringBuilder();
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

            //string scssString = sb.ToString();
            //ITaskItem scssFile = new TaskItem(scssString);
            //scssFile.SetMetadata("val", scssString);

            //write output cofnig file 
            //var file = File.ReadLines(Path.Combine(NugetFolder, "tools/Styles/MudBlazor.scss"));
            //string pattern = "@import 'components/_";
            //int patternLength = pattern.Length;

            string config = @$"{{
  ""Arguments"": ""--style=compressed"",
  ""IncludePaths"": [{NugetCssFolder}],

  ""Compilations"": [
    {{ ""Source"":  ""{OutputScssFile}"", ""Target"":  ""{OutputCssFile}"" }}
  ]
}}";

            File.WriteAllText(OutputConfigFile, config);


            //GeneratedFiles = [scssFile];

            return true;
        }
    }
}
