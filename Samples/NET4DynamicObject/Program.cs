using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using unvell.ReoScript;

namespace NET4DynamicObject
{
	class Program
	{
		static void Main(string[] args)
		{


dynamic myobj = new ExpandoObject();

myobj.name = "hello";
myobj.remark = "world";
myobj.add = (Func<int, int, int>)((a, b) =>
{
	return a + b;
});

var srm = new ScriptRunningMachine();

srm["myobj"] = myobj;

srm.Run(@"

var str1 = myobj.name + ' ' + myobj.remark;
var str2 = myobj.add(1, 2);
alert(str1 + ' ' + str2);

");


		}
	}
}
