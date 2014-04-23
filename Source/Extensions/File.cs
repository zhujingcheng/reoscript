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
using System.IO;
using unvell.ReoScript;

namespace unvell.ReoScript.Extensions
{
	public class FileObject
	{
		public FileInfo FileInfo { get; set; }
	}

	public class DirectoryObject
	{
		public DirectoryInfo DirInfo { get; set; }
	}

	public class FileConstructorFunction : TypedNativeFunctionObject<FileObject>
	{
		public override object CreateObject(ScriptContext context, object[] args)
		{
			if (args.Length == 0)
			{
				return null;
			}
			
			if (args.Length == 1)
			{
				
			}

			return new FileObject();
		}
	}

	public enum FileCreateFlag : byte
	{
		Open = 0x1,
		Create = 0x2,
	}

	public class DirectoryConstructorFunction : TypedNativeFunctionObject<DirectoryObject>
	{

	}

	[ModuleLoader]
	public class FileModuleLoader : IModuleLoader
	{
		public void LoadModule(ScriptRunningMachine srm)
		{
			srm.ImportType(typeof(FileConstructorFunction), "File");
			srm.ImportType(typeof(DirectoryConstructorFunction), "Directory");
		}

		public void UnloadModule(ScriptRunningMachine srm)
		{
		}
	}


}
