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
 * Author: Jing Lu <dujid0@gmail.com>
 * 
 * Copyright (c) 2012-2013 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;

namespace unvell.ReoScript.TestCase
{
	[TestSuite]
	class ReoScriptTestSuite
	{
		public ScriptRunningMachine SRM { get; set; }
	}

	#region Wrapper
	[TestSuite("Wrapper Interface")]
	class WrapperTest : ReoScriptTestSuite
	{
		public int Add(int a, int b)
		{
			return a + b;
		}

		[TestCase]
		public void WrapperObject()
		{


var srm = SRM;

var myobj = new ObjectValue();

string externalProperty = string.Empty;

myobj["external"] = new ExternalProperty(
	() => externalProperty,
	(v) => { externalProperty = System.Convert.ToString(v); }
	);

myobj["add"] = new NativeFunctionObject("add", (ctx, owner, args) =>
{
	return (double)args[0] + (double)args[1];
});

srm["myobj"] = myobj;

			srm.Run(@"

myobj.name = 'hello';
myobj.external = 'world';
var result = myobj.add(1,2);

debug.assert(myobj.name, 'hello');
debug.assert(result, 3);

");



		}
	}

	[TestSuite]
	class BasicAccess : ReoScriptTestSuite
	{
		[TestCase]
		void DictionaryAccess()
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();

			dic["name"] = "hello";

			SRM["myobj"] = dic;

			SRM.Run(@" debug.assert( myobj.name, 'hello' ); ");
		}

		[TestCase]
		public void FuncCall()
		{
			SRM["myfunc"] = (System.Func<int, int, int>)((a, b) =>
			{
				return a + b;
			});

			SRM.Run(@"debug.assert( myfunc(1, 2), 3 );");
		}
	}

	#endregion

	#region DirectAccess

	class Friut {
		public string Name { get; set; }
		public string Color { get; set; }
		public float Price { get; set; }

		public float cost = 2.95f;
		public string sid = "510010";

		private bool isShippedOut = false;

		public void ShipOut()
		{
			isShippedOut = true;
		}

		public bool IsShippedOut { get { return isShippedOut; } }

		public int methodInLowercase(int a, int b) { return a + b; }
	}

	class Stuff : IDictionary<string, object>
	{
		private Dictionary<string, object> stuff = new Dictionary<string, object>();

		#region IDictionary<string,object> Members

		public void Add(string key, object value) {	stuff.Add(key, value); }

		public bool ContainsKey(string key) { return stuff.ContainsKey(key); }

		public ICollection<string> Keys { get { return stuff.Keys; } }

		public bool Remove(string key) { return stuff.Remove(key); }

		public bool TryGetValue(string key, out object value) { return stuff.TryGetValue(key, out value); }

		public ICollection<object> Values { get { return stuff.Values; } }

		public object this[string key] { get { return stuff[key]; } set { stuff[key] = value; } }

		#endregion

		#region ICollection<KeyValuePair<string,object>> Members

		public void Add(KeyValuePair<string, object> item) {
			stuff.Add(item.Key, item.Value);
		}

		public void Clear()
		{
			stuff.Clear();
		}

		public bool Contains(KeyValuePair<string, object> item)
		{
			object o;
			if(!(stuff.TryGetValue(item.Key, out o))) return false;
			return o == item.Value;
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		public int Count
		{
			get { return stuff.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<string, object> item)
		{
			if (stuff.ContainsKey(item.Key))
			{
				return stuff.Remove(item.Key);
			}
			else
				return false;
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,object>> Members

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return stuff.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return stuff.GetEnumerator();
		}

		#endregion
	}

	[TestSuite("DirectAccess")]
	class DirectAccessTests : ReoScriptTestSuite
	{
		public const MachineWorkMode FullWorkMode =
		 MachineWorkMode.AllowCLREventBind | MachineWorkMode.AllowDirectAccess
		 | MachineWorkMode.AllowImportTypeInScript | MachineWorkMode.AutoImportRelationType
		 | MachineWorkMode.AutoUppercaseWhenCLRCalling | MachineWorkMode.IgnoreCLRExceptions;

		public const MachineWorkMode DirectAccessWithoutAutoUppercase =
			FullWorkMode & ~(MachineWorkMode.AutoUppercaseWhenCLRCalling);

		[TestCase("import and create instance", WorkMode = FullWorkMode)]
		public void ImportAndCreate()
		{
			SRM.ImportType(typeof(Friut));

			SRM.Run(@"

var t = debug.assert;

var apple = new Friut();

t(typeof apple, 'native object');
t(apple instanceof Friut);

");
		}

		[TestCase("create instance with alias", WorkMode = FullWorkMode)]
		public void ImportAndCreate2()
		{
			// import using alias
			SRM.ImportType(typeof(Friut), "MyClass");

			SRM.Run(@"

var t = debug.assert;

var apple = new MyClass();

t(typeof apple, 'native object');
t(apple instanceof MyClass);

");
		}

		[TestCase("access property", WorkMode = FullWorkMode)]
		public void AccessProperty()
		{
			// import using alias
			SRM.ImportType(typeof(Friut), "MyClass");

			SRM.Run(@"

var t = debug.assert;

var apple = new MyClass();

apple.name = 'apple';
t(apple.name, 'apple');

apple.Color = 'red';
apple.Price = 1.95;

t(apple.color, 'red');
t(apple.price, 1.95);

t(apple.color + ' ' + apple.price, 'red 1.95');

");
		}

		[TestCase("access method", WorkMode = FullWorkMode)]
		public void AccessMethod()
		{
			// import using alias
			SRM.ImportType(typeof(Friut), "MyClass");

			SRM.Run(@"

var t = debug.assert;

var apple = new MyClass();
apple.shipOut();

t(apple.isShippedOut, true);

");
		}

		[TestCase("access lowercase method", WorkMode = DirectAccessWithoutAutoUppercase)]
		public void AccessLowercaseMethod()
		{
			// import using alias
			SRM.ImportType(typeof(Friut), "MyClass");

			SRM.Run(@"

var t = debug.assert;

var apple = new MyClass();
var c = apple.methodInLowercase(5, 6);

t(c, 11);

");
		}

		[TestCase("comparing operators", WorkMode = FullWorkMode)]
		public void ComparingOperators()
		{
			// import using alias
			SRM.ImportType(typeof(Friut), "MyClass");

			SRM.Run(@"

var t = debug.assert;

var apple = new MyClass();
apple.price = 1.95;

t(apple.price <= 1.95);
t(apple.price >= 1.95);
t(apple.price == 1.95);

t(apple.price == '1.95');         // compare to string

t(apple.price === 1.95);
apple.price = 2.1;
t(apple.price === 2.1);    

");
		}

		[TestCase("IDictionary interface support", WorkMode = FullWorkMode)]
		public void IDictionarySupport()
		{
			// import using alias
			SRM["obj"] = new Stuff();

			SRM.Run(@"

var t = debug.assert;

var date = new Date();

obj.name = 'red alert';
obj.startTime = date;

t(obj.name, 'red alert');
t(obj.startTime, date);

");
		}

		[TestCase("IDictionary enumeration", WorkMode = FullWorkMode)]
		public void IDictionaryEnumeration()
		{
			// import using alias
			SRM["obj"] = new Stuff();

			SRM.Run(@"

var t = debug.assert;

var date = new Date();

obj.name = 'red alert';
obj.startTime = date;

var o = '';
for(name in obj) {
  o+=name + ' ';
}

t(o, 'name startTime ');

");
		}

		[TestCase("CLR Attribute Property")]
		public void CLRAttributeProperty()
		{

		}

		[TestCase("CLR Attribute Method")]
		public void CLRAttributeMethod()
		{

		}

		[TestCase("TestCase Template", Disabled=true)]
		public void TestCaseTemplate()
		{
		}
	}
	#endregion // DirectAccess

	#region ScriptVisible
	[ScriptVisible]
	class Contact
	{
		[ScriptVisible] 
		public string Name { get; set; }
		
		[ScriptVisible] 
		public List<string> PhoneNumbers { get; set; }

		public string Remark { get; set; }

		public Contact()
		{
			this.PhoneNumbers = new List<string>();
		}
	}

	[TestSuite("AttributeVisible")]
	class AttributeExtensionTests : ReoScriptTestSuite
	{
		[TestCase(WorkMode=MachineWorkMode.Default)]
		public void CallCLR()
		{
			SRM["contact"] = new Contact();

			string script = @"

var t = debug.assert;

contact.name = 'reo';

t( contact.phoneNumbers != null );

contact.phoneNumbers.add('01-234-567');

t( contact.name, 'reo' );
t( contact.phoneNumbers.length, 1 );
t( contact.phoneNumbers[0], '01-234-567' );

// access invisible property
contact.remark = 'comment';
t( contact.remark, null );

";

			SRM.Run(script);
		}

		[TestCase(WorkMode = MachineWorkMode.Default)]
		public void ImportType()
		{
			SRM.ImportType(typeof(Contact));

			string script = @"

var t = debug.assert;

var contact = new Contact();

contact.name = 'reo';

t( contact.phoneNumbers != null );

contact.phoneNumbers.add('01-234-567');

t( contact.name, 'reo' );
t( contact.phoneNumbers.length, 1 );
t( contact.phoneNumbers[0], '01-234-567' );

// access invisible property
contact.remark = 'comment';
t( contact.remark, null );

";

			SRM.Run(script);
		}

	}
	#endregion // ScriptVisible
}