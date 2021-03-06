﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of RowConverter.
	/// </summary>
	/// 
	
	public class GroupedRowConverter:BaseConverter
	{

		private BaseReportItem parent;
		
		public GroupedRowConverter(IDataNavigator dataNavigator,
		                           ExporterPage singlePage, ILayouter layouter):base(dataNavigator,singlePage,layouter)
		{
		}
		
		
		public override ExporterCollection Convert(BaseReportItem parent, BaseReportItem item)
		{
			if (parent == null) {
				throw new ArgumentNullException("parent");
			}
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			
			ISimpleContainer simpleContainer = item as ISimpleContainer;
			this.parent = parent;
			
			simpleContainer.Parent = parent;
			
			PrintHelper.AdjustParent(parent,simpleContainer.Items);
			if (PrintHelper.IsTextOnlyRow(simpleContainer)) {
				ExporterCollection myList = new ExporterCollection();

				BaseConverter.BaseConvert (myList,simpleContainer,parent.Location.X,
				                           new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y));
				
				return myList;
			} else {
				return this.ConvertDataRow(simpleContainer);
			}
		}
		
		
		private ExporterCollection ConvertDataRow (ISimpleContainer simpleContainer)
		{
			ExporterCollection exporterCollection = new ExporterCollection();
			base.CurrentPosition = new Point(base.SectionBounds.DetailStart.X,base.SectionBounds.DetailStart.Y);
			BaseSection section = parent as BaseSection;
			
			int defaultLeftPos = parent.Location.X;
			
			Size groupSize = Size.Empty;
			Size childSize = Size.Empty;
			
			Console.WriteLine("-------------------START");

			Console.WriteLine ("section {0}",section.Size);
		
			Console.WriteLine();
			
			
			if (section.Items.IsGrouped)
			{
				groupSize = section.Items[0].Size;
				childSize  = section.Items[1].Size;
				Console.WriteLine ("group {0}",section.Items[0].Size);
				Console.WriteLine ("detail {0}",section.Items[1].Size);
			}
			
			Rectangle pageBreakRect = Rectangle.Empty;
			
			do {
				base.SaveSectionSize(section.Size);
				PrintHelper.AdjustSectionLocation (section);
				section.Size = this.SectionBounds.DetailSectionRectangle.Size;
				
				// did we have GroupedItems at all
				if (section.Items.IsGrouped)
				{
					// GetType child navigator
					IDataNavigator childNavigator = base.DataNavigator.GetChildNavigator();
					
					base.Evaluator.SinglePage.IDataNavigator = childNavigator;
					
					base.CurrentPosition = ConvertGroupHeader(exporterCollection,section,defaultLeftPos,base.CurrentPosition);
					
					section.Size = base.RestoreSize;
					section.Items[0].Size = groupSize;
					section.Items[1].Size = childSize;
					
					childNavigator.Reset();
					childNavigator.MoveNext();
					
					Console.WriteLine("-------------------after group");

			Console.WriteLine ("section {0}",section.Size);
			Console.WriteLine ("group {0}",section.Items[0].Size);
			Console.WriteLine ("detail {0}",section.Items[1].Size);
			Console.WriteLine();
					
					//Convert children
					if (childNavigator != null) {
						StandardPrinter.AdjustBackColor(simpleContainer,GlobalValues.DefaultBackColor);
						do
						{
							Console.WriteLine("-----------------childs");
			Console.WriteLine ("section {0}",section.Size);
			Console.WriteLine ("group {0}",section.Items[0].Size);
			Console.WriteLine ("detail {0}",section.Items[1].Size);
			Console.WriteLine();
							section.Size = base.RestoreSize;
							section.Items[0].Size = groupSize;
							section.Items[1].Size = childSize;
							
							
							childNavigator.Fill(simpleContainer.Items);
							
							base.CurrentPosition = ConvertGroupChilds (exporterCollection,section,
							                                      simpleContainer,defaultLeftPos,base.CurrentPosition);
							pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)section.Items[1],base.CurrentPosition);
//							section.Items[1].Size = base.RestoreSize;
							if (PrintHelper.IsPageFull(pageBreakRect,base.SectionBounds )) {
								base.CurrentPosition = ForcePageBreak (exporterCollection,section);
							}
						}
						while ( childNavigator.MoveNext());
						
						if (PageBreakAfterGroupChange(section) ) {
							
							if ( base.DataNavigator.HasMoreData)
							{
								base.CurrentPosition = ForcePageBreak (exporterCollection,section);
							}
						}
						
						base.Evaluator.SinglePage.IDataNavigator = base.DataNavigator;
					}
				}
				else
				{
					// No Grouping at all
					Size dd = section.Items[0].Size;
					Console.WriteLine("---------NoGrouping");
					Console.WriteLine ("section {0}",section.Size);
					Console.WriteLine ("row {0}",dd);
					base.CurrentPosition = ConvertStandardRow (exporterCollection,section,simpleContainer,defaultLeftPos,base.CurrentPosition);
					section.Size = base.RestoreSize;
					section.Items[0].Size = dd;
				}
				
				pageBreakRect = PrintHelper.CalculatePageBreakRectangle((BaseReportItem)section.Items[0],base.CurrentPosition);
				
				if (PrintHelper.IsPageFull(pageBreakRect,base.SectionBounds)) {
					base.CurrentPosition = ForcePageBreak (exporterCollection,section);
				}
				
				ShouldDrawBorder (section,exporterCollection);
			}
			while (base.DataNavigator.MoveNext());
			
			SectionBounds.ReportFooterRectangle =  new Rectangle(SectionBounds.ReportFooterRectangle.Left,
			                                                     section.Location.Y + section.Size.Height,
			                                                     SectionBounds.ReportFooterRectangle.Width,
			                                                     SectionBounds.ReportFooterRectangle.Height);
		
			return exporterCollection;
		}
		
		
		protected override Point ForcePageBreak(ExporterCollection exporterCollection, BaseSection section)
		{
			base.ForcePageBreak(exporterCollection,section);
			return CalculateStartPosition();
		}
		
		
		private Point CalculateStartPosition()
		{
			return new Point(base.SectionBounds.PageHeaderRectangle.X,base.SectionBounds.PageHeaderRectangle.Y);
		}
		
		
		private Point ConvertGroupHeader(ExporterCollection exportList,BaseSection section,int leftPos,Point offset)
		{
			var retVal = Point.Empty;
			ReportItemCollection groupCollection = null;
			var groupedRows  = BaseConverter.FindGroups(section);
			if (groupedRows.Count == 0) {
				groupCollection = section.Items.ExtractGroupedColumns();
				base.DataNavigator.Fill(groupCollection);
				base.FireSectionRendering(section);
				ExporterCollection list = StandardPrinter.ConvertPlainCollection(groupCollection,offset);
				
				StandardPrinter.EvaluateRow(base.Evaluator,list);
				
				exportList.AddRange(list);
				AfterConverting (section,list);
				retVal =  new Point (leftPos,offset.Y + groupCollection[0].Size.Height + 20  + (3 *GlobalValues.GapBetweenContainer));
			} else {
				retVal = ConvertStandardRow(exportList,section,groupedRows[0],leftPos,offset);
			}
			return retVal;
		}
		
		
		private static void ShouldDrawBorder (BaseSection section,ExporterCollection list)
		{
			if (section.DrawBorder == true) {
				BaseRectangleItem br = BasePager.CreateDebugItem (section);
				BaseExportColumn bec = br.CreateExportColumn();
				bec.StyleDecorator.Location = section.Location;
				list.Insert(0,bec);
			}
		}
	}
}
