﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Reports.Core.Interfaces;
using System;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
	/// <summary>
	/// Description of IExpressionEvaluatorFassade.
	/// </summary>
	public interface IExpressionEvaluatorFacade
	{
		string Evaluate (string expression);
		IPageInfo SinglePage {get;set;}
	}
}
