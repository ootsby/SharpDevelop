﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportItem.
	/// </summary>
	/// 
	[TypeDescriptionProvider(typeof(AbstractItemTypeProvider))]
	public abstract class AbstractItem:System.Windows.Forms.Control
	{
		private Color frameColor = Color.Black;
		

		protected AbstractItem()
		{
			InitializeComponent();
			TypeDescriptor.AddProvider(new AbstractItemTypeProvider(), typeof(AbstractItem));
		}
		
		
		protected void DrawControl (Graphics graphics,Rectangle borderRectangle)
		{
			if (this.DrawBorder == true) {
				graphics.DrawRectangle(new Pen(this.frameColor),borderRectangle);
			} 
			System.Windows.Forms.ControlPaint.DrawBorder3D(graphics, this.ClientRectangle,
				                                               System.Windows.Forms.Border3DStyle.Etched);
		}
		
		
		#region Property's
		
		protected Rectangle DrawingRectangle {
			get {
				
				return new Rectangle(this.ClientRectangle.Left ,
				                            this.ClientRectangle.Top ,
				                            this.ClientRectangle.Width -1,
				                            this.ClientRectangle.Height -1);
			}
		}
		
		
		[Category("Border")]
		public Color FrameColor {
			get { return frameColor; }
			set {
				frameColor = value;
				this.Invalidate();
			}
		}
		
		
		[Category("Border"),
		Description("Draw a Border around the Item")]
		public bool DrawBorder {get;set;}
		
		
		protected new Size DefaultSize {get;set;}
			
		
		#endregion
		
		[System.ComponentModel.EditorBrowsableAttribute()]
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
		}
		
		public abstract void Draw(Graphics graphics);
		
		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.ResumeLayout(false);
		}
	}
	
	internal class AbstractItemTypeProvider : TypeDescriptionProvider {
		public AbstractItemTypeProvider() :  base(TypeDescriptor.GetProvider(typeof(AbstractItem)))
		{
		}
		
		public AbstractItemTypeProvider(TypeDescriptionProvider parent): base(parent)
		{
		}

		
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor td = base.GetTypeDescriptor(objectType, instance);
			return new AbstractItemTypeDescriptor(td, instance);
		}
	}
	
	internal class AbstractItemTypeDescriptor : CustomTypeDescriptor
	{
//		private AbstractItem _instance;
		
		public AbstractItemTypeDescriptor(ICustomTypeDescriptor parent, object instance)
			: base(parent)
		{
//			_instance = instance as AbstractItem;
		}

		

		public override PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}

		
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection props = base.GetProperties(attributes);
			List<PropertyDescriptor> allProperties = new List<PropertyDescriptor>();

			foreach (PropertyDescriptor p in props)
			{
				allProperties.Add(p);
			}
			return new PropertyDescriptorCollection(allProperties.ToArray());
		}
	}
	
}
