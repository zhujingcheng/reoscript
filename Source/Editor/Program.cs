using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using unvell.ReoScript.Editor;

namespace unvell.ReoScriptRunner
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var editor = new ReoScriptEditor();
			editor.Srm.WorkMode = ReoScript.MachineWorkMode.Full;

			Application.Run(editor);
		}
	}
}
