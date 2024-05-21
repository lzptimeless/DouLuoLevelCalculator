using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDemo
{
    internal class ScriptHelper
    {
        private static readonly ScriptOptions _scriptOptions;

        static ScriptHelper()
        {
            var allSemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assemblies = allSemblies.Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location)).ToArray();

            _scriptOptions = ScriptOptions.Default
                .WithReferences(assemblies)
                .WithImports("System", "System.Linq", "System.Collections.Generic", "RoslynDemo");
        }

        public static async Task<object> ExecuteScriptAsync(string code, object? globals = null)
        {
            return await CSharpScript.EvaluateAsync(code, _scriptOptions, globals: globals);
        }
    }
}
