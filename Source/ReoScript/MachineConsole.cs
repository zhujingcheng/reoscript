/*****************************************************************************
 * 
 * ReoScript - .NET Script Language Engine
 * 
 * http://www.unvell.com/ReoScript
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * This software released under LGPLv3 license.
 * Author: Jing Lu <dujid0 at gmail.com>
 * 
 * Copyright (c) 2012-2014 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace unvell.ReoScript
{
	public class MachineConsole
	{
		ScriptRunningMachine srm = new ScriptRunningMachine(CoreFeatures.FullFeatures);

		private List<string> fileList = new List<string>();

		public bool IsDebugMode { get; set; }

		public bool IsQuietMode { get; set; }

		public MachineConsole()
			: this(null)
		{
		}

		public MachineConsole(string[] args)
		{
			this.IsDebugMode = false;
			this.IsQuietMode = true;

			srm.AddStdOutputListener(new BuiltinConsoleOutputListener());

			QueryPerformanceFrequency(out freq);

			if (args != null)
			{
				foreach (string arg in args)
				{
					if (arg.StartsWith("-"))
					{
						string option = arg.Substring(1, arg.Length - 1);
						switch (option)
						{
							case "e":
								IsDebugMode = true;
								break;

							case "v":
								IsQuietMode = false;
								break;

							case "?":
							case "h":
							case "help":
								OutLn("usage: ReoScriptSheel.exe file0 file1 ... filen -[e|h]");
								break;

							default:
								OutLn("unknown option: " + arg);
								break;
						}
					}
					else
						fileList.Add(arg);
				}
			}
		}

		public void Run()
		{
			if (IsDebugMode)
			{
				OutLn("ReoScript Machine Console (ver1.1)");
				OutLn("type /help to see help topic.\n");
			}

			foreach (string file in fileList)
			{
				try
				{
					if(!IsQuietMode) Out("loading " + file +"... ");
					srm.Load(file);
					if(!IsQuietMode) OutLn("ok.");
				}
				catch(Exception ex) {
					if(!IsQuietMode) OutLn("failed: " + ex.Message);
				}
			}

			if (IsDebugMode)
			{
				if(!IsQuietMode) OutLn("\nReady.\n");

				bool isQuitRequired = false;

				while (!isQuitRequired)
				{
					Prompt();

					string line = In().Trim();
					if (line == null)
					{
						isQuitRequired = true;
						break;
					}
					else if (line.StartsWith("."))
					{
						srm.Load(line.Substring(1, line.Length - 1));
					}
					else if (line.StartsWith("/"))
					{
						string consoleCmd = line.Substring(1);

						switch (consoleCmd)
						{
							case "q":
							case "quit":
							case "exit":
								isQuitRequired = true;
								break;
							case "h":
							case "help":
								Help();
								break;
							default:
								break;
						}
					}
					else if (line.Equals("?"))
					{
						OutLn(srm.GlobalObject.DumpObject());
					}
					else if (line.StartsWith("?"))
					{
						string expression = line.Substring(1);
						try
						{
							object value = srm.CalcExpression(expression);
							OutLn(value == null ? "undefined" : value.ToString());
						}
						catch (Exception ex)
						{
							OutLn("error: " + ex.Message);
						}
					}
					else if (line.Length == 0)
					{
						continue;
					}
					else
					{
						try
						{
							var res = srm.Run(line);

							if (res is ObjectValue)
							{
								OutLn(((ObjectValue)res).DumpObject());
							}
							else
							{
								OutLn(ScriptRunningMachine.ConvertToString(res));
							}
						}
						catch (ReoScriptException ex)
						{
							Console.WriteLine("error: " + ex.Message + "\n");
						}
					}
				}

				OutLn("Bye.");
			}
		}

		private void Help()
		{
			OutLn(@"
ReoScript Machine Console Help

/<system command>       submit system command.
  quit | q              quit from console.
  help | h              show this topic.

?[experssion]           calculate an expression and output result.
                        if expression is null, list all varaibles in current 
                        global object.

<statement>;						run ReoScript statement.

.<path>                 load script resource from specified file.


 ");

		}
		void Prompt()
		{
			Out(">");
		}
		private void Out(string msg)
		{
			Console.Write(msg);
		}
		private void OutLn()
		{
			Console.WriteLine();
		}
		private void OutLn(string msg)
		{
			Console.WriteLine(msg);
		}
		private string In()
		{
			return Console.ReadLine();
		}

		#region Runtime Analysis
		//private static DateTime logStartTime;
		static long freq = 0;
		internal static void LogStart(string msg)
		{
			if (msg != null && msg.Length > 0) System.Console.Write(msg);
			QueryPerformanceCounter(out start);
		}
		static long start = 0;
		static long end = 0;
		internal static double LogEnd(string msg)
		{
			QueryPerformanceCounter(out end);
			double dur = Math.Round((double)(end - start) / (double)freq * 1000, 4);
			if (msg != null && msg.Length > 0)
			{
				msg += " (cost " + dur + " ms.)\n";
				Console.WriteLine(msg);
			}
			return dur;
		}
		[DllImport("Kernel32.dll")]
		internal static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[DllImport("Kernel32.dll")]
		internal static extern bool QueryPerformanceFrequency(out long lpFrequency);
		#endregion
	}
}
