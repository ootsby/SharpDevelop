﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Debugger.AddIn.Visualizers.Utils;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// ObjectProperty used in ObjectGraph. Has TargetNode.
	/// Support on-demand evaluation of Value property.
	/// </summary>
	public class ObjectGraphProperty : ObjectProperty, IEvaluate
	{
		/// <summary>
		/// Node that this property points to. Can be null. Always null if <see cref="IsAtomic"/> is true.
		/// </summary>
		public ObjectGraphNode TargetNode { get; set; }
		
		//private Expressions.Expression containingObjectExpression;
		
		/*public ObjectGraphProperty(Expressions.Expression containingObjectExpression)
		{
			if (containingObjectExpression == null)
				throw new ArgumentNullException("containingObjectExpression");
			this.containingObjectExpression = containingObjectExpression;
		}*/
		
		/// <summary>
        /// MemberInfo used for obtaining value of this property
        /// </summary>
        public MemberInfo PropInfo { get; set; }
        
		
		bool evaluateCalled = false;
		public bool IsEvaluated
		{
			get { return this.evaluateCalled; }
		}
		
		public void Evaluate()
		{
			/*if (this.PropInfo == null)
			{
				throw new DebuggerVisualizerException("Cannot evaluate property with missing MemberInfo");
			}
			Value debuggerVal = this.containingObjectExpression.Evaluate(WindowsDebugger.CurrentProcess).GetMemberValue(this.PropInfo);*/
			
			if (this.Expression == null)
			{
				throw new DebuggerVisualizerException("Cannot evaluate property with missing Expression");
			}
			Value debuggerVal = this.Expression.Evaluate(WindowsDebugger.CurrentProcess);
			
			this.IsAtomic = debuggerVal.Type.IsAtomic();
			this.IsNull = debuggerVal.IsNull;
			// null and complex properties will show empty string
			this.Value = debuggerVal.IsNull || (!this.IsAtomic) ? string.Empty : debuggerVal.InvokeToString();
			this.evaluateCalled = true;
		}
	}
}
