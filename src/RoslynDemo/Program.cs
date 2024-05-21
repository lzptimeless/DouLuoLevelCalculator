// See https://aka.ms/new-console-template for more information
using Microsoft.CodeAnalysis.Scripting;
using RoslynDemo;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

Stopwatch watch = Stopwatch.StartNew();
_ = ScriptHelper.ExecuteScriptAsync("Console.WriteLine(\"Script 1\")").Result;
watch.Stop();
Console.WriteLine($"During: {watch.Elapsed}");

watch = Stopwatch.StartNew();
_ = ScriptHelper.ExecuteScriptAsync("Console.WriteLine(\"Script 2\" + Name)", new ScriptGlobal()).Result;
watch.Stop();
Console.WriteLine($"During: {watch.Elapsed}");

watch = Stopwatch.StartNew();
_ = ScriptHelper.ExecuteScriptAsync("Console.WriteLine(\"Script 3\")").Result;
watch.Stop();
Console.WriteLine($"During: {watch.Elapsed}");