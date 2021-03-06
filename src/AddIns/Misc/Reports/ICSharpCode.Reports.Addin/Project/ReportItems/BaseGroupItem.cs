﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.BaseClasses.Printing;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of BaseGroupItem.
	/// </summary>
	/// 
	[Designer(typeof(ICSharpCode.Reports.Addin.Designer.GroupHeaderDesigner))]
	public class BaseGroupItem:BaseDataItem
	{
		public BaseGroupItem()
		{
		}
		
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
//			if (Text.CompareTo(ColumnName) != 0 ) {
//				Text = ColumnName;
//			}
			base.OnPaint(e);
			this.Draw(e.Graphics);
		}
		
		
		public override void Draw(Graphics graphics)
		{
			base.Draw (graphics);
		}
	}
}
