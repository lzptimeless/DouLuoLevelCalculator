// See https://aka.ms/new-console-template for more information
using Microsoft.CodeAnalysis.Scripting;
using RoslynDemo;
using System.Diagnostics;

Console.WriteLine("Hello, World!");
var global = new ScriptGlobal();

Stopwatch watch = Stopwatch.StartNew();
_ = ScriptHelper.ExecuteScriptAsync("Console.WriteLine(\"Script 1\")").Result;
watch.Stop();
Console.WriteLine($"During: {watch.Elapsed}");

watch = Stopwatch.StartNew();
_ = ScriptHelper.ExecuteScriptAsync("Console.WriteLine(\"Script 2\" + Name); Name = Name + \"3\";", global).Result;
watch.Stop();
Console.WriteLine($"During: {watch.Elapsed}");

watch = Stopwatch.StartNew();
var result = ScriptHelper.ExecuteScriptAsync("Console.WriteLine(\"Script 3\" + Name); return Name + \"4\";", global).Result;
watch.Stop();
Console.WriteLine($"During: {watch.Elapsed}, {result}");