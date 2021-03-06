﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// A node in the variable tree.
	/// The node is imutable.
	/// </summary>
	public class TreeNode: IComparable<TreeNode>, ITreeNode
	{
		IImage iconImage = null;
		string name  = string.Empty;
		string text  = string.Empty;
		string type  = string.Empty;
		IEnumerable<TreeNode> childNodes = null;
		
		/// <summary>
		/// The image displayed for this node.
		/// </summary>
		public IImage IconImage {
			get { return iconImage; }
			protected set { iconImage = value; }
		} 
		
		/// <summary>
		/// System.Windows.Media.ImageSource version of <see cref="IconImage"/>.
		/// </summary>
		public ImageSource ImageSource {
			get { 
				return iconImage == null ? null : iconImage.ImageSource;
			}
		}
		
		/// <summary>
		/// System.Drawing.Image version of <see cref="IconImage"/>.
		/// </summary>
		public Image Image {
			get { 
				return iconImage == null ? null : iconImage.Bitmap;
			}
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public virtual string Text
		{
			get { return text; }
			set { text = value; }
		}
		
		public virtual string Type {
			get { return type; }
			protected set { type = value; }
		}
		
		public virtual IEnumerable<TreeNode> ChildNodes {
			get { return childNodes; }
			protected set { childNodes = value; }
		}
		
		IEnumerable<ITreeNode> ITreeNode.ChildNodes {
			get { return childNodes; }
		}
		
		public virtual bool HasChildNodes {
			get { return childNodes != null; }
		}
		
		public virtual bool CanSetText { 
			get { return false; }
		}
		
		public virtual IEnumerable<IVisualizerCommand> VisualizerCommands {
			get {
				return null;
			}
		}
		
		public virtual bool HasVisualizerCommands {
			get {
				return (VisualizerCommands != null) && (VisualizerCommands.Count() > 0);
			}
		}
		
		public TreeNode()
		{
		}
		
		public TreeNode(IImage iconImage, string name, string text, string type, IEnumerable<TreeNode> childNodes)
		{
			this.iconImage = iconImage;
			this.name = name;
			this.text = text;
			this.type = type;
			this.childNodes = childNodes;
		}
		
		public int CompareTo(TreeNode other)
		{
			return this.Name.CompareTo(other.Name);
		}
		
		public virtual bool SetText(string newValue) { 
			return false;
		}
	}
}
