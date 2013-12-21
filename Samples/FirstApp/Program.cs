using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using unvell.ReoScript;

namespace FirstApp
{
	/// <summary>
	/// Used for Car1 (Proxy mode) and Car3 (DirectAccess mode)
	/// </summary>
	public class Car
	{
		public void Forward()
		{
			MessageBox.Show("forwarding");
		}
	}

	/// <summary>
	/// Proxy class provided for script
	/// </summary>
	public class CarProxy : ObjectValue
	{
		public Car Car { get; set; }
		
		public CarProxy() {
			this.Car = new Car();

			this["forward"] = new NativeFunctionObject("forward", (ctx, owner, args) =>
			{
				this.Car.Forward();
				return null;
			});
		}
	}

	/// <summary>
	/// Use ScriptVisible Attribute to make .NET objects available in script
	/// </summary>
	[ScriptVisible]
	public class Car2
	{
		[ScriptVisible]
		public void Forward()
		{
			MessageBox.Show("forwarding");
		}
	}

	static class Program
	{
		[STAThread]
		static void Main()
		{

			var srm = new ScriptRunningMachine();

			srm.AllowDirectAccess = true;

			// proxy mode
			srm.ImportType(typeof(CarProxy), "Car1");

			// attribute mode
			srm.ImportType(typeof(Car2), "Car2");

			// DirectAccess mode
			srm.ImportType(typeof(Car), "Car3");

			srm.Run(@"

var car1 = new Car1();
car1.forward();

var car2 = new Car2();
car2.forward();

var car3 = new Car3();
car3.forward();

");


		}
	}
}
