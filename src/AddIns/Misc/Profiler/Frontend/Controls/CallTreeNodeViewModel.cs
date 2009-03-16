﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Profiler.Controller.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ICSharpCode.Profiler.Controller;

namespace ICSharpCode.Profiler.Controls
{
	public class CallTreeNodeViewModel : IViewModel<CallTreeNodeViewModel>, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Member

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion

		ReadOnlyCollection<CallTreeNodeViewModel> children;
		protected readonly CallTreeNode node;
		CallTreeNodeViewModel parent;
		protected int level;
		
		public CallTreeNode Node { get { return node; } }
		
		public event EventHandler<NodeEventArgs<CallTreeNodeViewModel>> VisibleChildExpandedChanged;
		
		protected virtual void OnVisibleChildExpandedChanged(NodeEventArgs<CallTreeNodeViewModel> e)
		{
			if (VisibleChildExpandedChanged != null) {
				VisibleChildExpandedChanged(this, e);
			}
		}
		
		public int Level
		{
			get { return level; }
			set { level = value; }
		}
		
		public CallTreeNodeViewModel Parent
		{
			get { return parent; }
		}

		public virtual long CpuCyclesSpent
		{
			get {
				return node.CpuCyclesSpent;
			}
		}
		
		public virtual double TimePercentageOfParent
		{
			get {
				if (this.parent == null)
					return 1;
				else
					return (double)this.node.CpuCyclesSpent / (double)GetNodeFromLevel(1).node.CpuCyclesSpent;
			}
		}
		
		CallTreeNodeViewModel GetNodeFromLevel(int level)
		{
			if (this.level > level) {
				return this.parent.GetNodeFromLevel(level);
			}
			
			return this;
		}
		
		public virtual string TimePercentageOfParentAsText
		{
			get {
				return (TimePercentageOfParent * 100).ToString("0.00") + "%";
			}
		}
		
		public virtual object ToolTip
		{
			get { return CreateShortToolTip(); }
		}
		
		object CreateShortToolTip()
		{
			if (level == 0)
				return null; // no tooltip for root
			if (level == 1)
				return Name;

			TextBlock text = new TextBlock {
				Inlines = {
					((!string.IsNullOrEmpty(node.ReturnType)) ? node.ReturnType + " " : ""),
					new Bold { Inlines = { node.Name } },
					"(" + ((node.Parameters.Count > 0) ? string.Join(", ", node.Parameters.ToArray()) : "") + ")"
				}
			};

			return text;
		}

		public object CreateToolTip()
		{
			if (node.IsThread)
				return Name; // only name for threads

			TextBlock text = new TextBlock
			{
				Inlines = {
					((!string.IsNullOrEmpty(node.ReturnType)) ? node.ReturnType + " " : ""),
					new Bold { Inlines = { node.Name } },
					"(" + ((node.Parameters.Count > 0) ? string.Join(", ", node.Parameters.ToArray()) : "") + ")\n",
					new Bold { Inlines = { "CPU Cycles:" } },
					" " + node.CpuCyclesSpent + "\n",
					new Bold { Inlines = { "Time:" } },
					" " + node.TimeSpent.ToString("f6") + "ms\n",
					new Bold { Inlines = { "Calls:" } },
					" " + node.CallCount.ToString()
				}
			};

			return text;
		}
		
		public string GetSignature()
		{
			if (node.IsThread)
				return Name;
			return node.Signature;
		}

		public ReadOnlyCollection<string> Parameters
		{
			get {
				return new ReadOnlyCollection<string>(this.node.Parameters);
			}
		}

		public CallTreeNodeViewModel(CallTreeNode node, CallTreeNodeViewModel parent)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			this.node = node;
			this.parent = parent;
			this.level = (this.parent == null) ? 1 : this.parent.level + 1;
		}

		public bool IsAncestorOf(CallTreeNodeViewModel descendant)
		{
			if (this != descendant)
			{
				if (descendant == null)
					return false;

				return IsAncestorOf(descendant.Parent);
			}
			else
				return true;
		}

		public virtual ReadOnlyCollection<CallTreeNodeViewModel> Children {
			get {
				lock (this) {
					if (this.children == null)
						this.children = this.node.Children
							.Select(c => new CallTreeNodeViewModel(c, this))
							.ToList()
							.AsReadOnly();
					
					return this.children;
				}
			}
		}

		public virtual string Name
		{
			get {
				return this.node.Name ?? string.Empty;
			}
		}

		public virtual string FullyQualifiedClassName
		{
			get {
				if (string.IsNullOrEmpty(this.node.Name))
					return null;
				if (!this.node.Name.Contains("."))
					return null;
				int index = this.node.Name.LastIndexOf('.');
				if (this.node.Name[index - 1] == '.')
					index--;
				return this.node.Name.Substring(0, index);
			}
		}

		public virtual string MethodName
		{
			get {
				if (string.IsNullOrEmpty(this.node.Name))
					return null;
				if (!this.node.Name.Contains("."))
					return null;
				int index = this.node.Name.LastIndexOf('.');
				if (this.node.Name[index - 1] == '.')
					index--;
				return this.node.Name.Substring(index + 1);
			}
		}
		
		bool isExpanded;
		
		public bool IsExpanded {
			get { return isExpanded; }
			set {
				if (isExpanded != value) {
					isExpanded = value;
					OnPropertyChanged("IsExpanded");
					this.IsExpandedChanged(new NodeEventArgs<CallTreeNodeViewModel>(this));
					
					if (!isExpanded) {
						DeselectChildren(children);
					}
				}
			}
		}

		void DeselectChildren(IList<CallTreeNodeViewModel> children)
		{
			foreach (CallTreeNodeViewModel item in children) {
				item.IsSelected = false;
				if (item.isExpanded)
					DeselectChildren(item.Children);
			}
		}
		
		void IsExpandedChanged(NodeEventArgs<CallTreeNodeViewModel> e)
		{
			this.visibleElementCount = 1 + (IsExpanded ? Children.Sum(c => c.VisibleElementCount) : 0);
			OnVisibleChildExpandedChanged(e);
			
			if (Parent != null && Parent.IsExpanded) {
				Parent.IsExpandedChanged(e);
			}
		}

		/*void IsSelectedChanged(CallTreeNodeViewModel item)
		{
			if (Parent != null)
				Parent.IsSelectedChanged(item);
			else
				OnChildSelectedChanged(new CallTreeNodeViewModelEventArgs(item));
		}

		protected virtual void OnChildSelectedChanged(CallTreeNodeViewModelEventArgs eventArgs)
		{
			if (ChildSelectedChanged != null)
				ChildSelectedChanged(this, eventArgs);
		}*/
		
		bool isSelected;
		
		public bool IsSelected {
			get { return isSelected; }
			set {
				if (isSelected != value) {
					isSelected = value;
					OnPropertyChanged("IsSelected");
					//this.IsSelectedChanged(this);
					
					if (isSelected) {
						CallTreeNodeViewModel node = this.Parent;
						while (node != null) {
							node.IsExpanded = true;
							node = node.Parent;
						}
					}
				}
			}
		}

		public bool Search(string search, bool recursive, out CallTreeNodeViewModel result)
		{
			if (recursive)
				return SearchRecursive(search, this, out result);
			else
				return SearchInternal(search, out result);
		}

		bool SearchInternal(string search, out CallTreeNodeViewModel result)
		{
			foreach (CallTreeNodeViewModel item in this.Children) {
				if (item.Name.IndexOf(search, StringComparison.Ordinal) > -1) {
					result = item;
					return true;
				}
			}

			result = null;
			return false;
		}

		bool SearchRecursive(string search, CallTreeNodeViewModel current, out CallTreeNodeViewModel result)
		{
			foreach (CallTreeNodeViewModel item in current.Children) {
				if (item.Name.IndexOf(search, StringComparison.Ordinal) > -1) {
					result = item;
					return true;
				}

				if (SearchRecursive(search, item, out result))
					return true;
			}

			result = null;
			return false;
		}
		
		public string TimeSpent {
			get {
				if (!node.IsThread)
					return node.TimeSpent.ToString("f6") + "ms";
				else
					return null;
			}
		}
		
		public string CallCount {
			get {
				if (!node.IsThread)
					return node.CallCount.ToString();
				else
					return null;
			}
		}
		
		public Visibility CheckBoxVisibility
		{
			get {
				if (this.Children.Count > 0)
					return Visibility.Visible;
				else
					return Visibility.Hidden;
			}
		}
		
		public override string ToString()
		{
			return "[" + Name + "]";
		}

		#region IViewModel<CallTreeNodeViewModel> Member

		int visibleElementCount = 1;

		public virtual int VisibleElementCount
		{
			get {
				return this.visibleElementCount;
			}
		}
		
		public virtual Thickness IndentationMargin {
			get {
				return new Thickness((level - 1) * 12, 0, 2, 0);
			}
		}

		#endregion
	}
}