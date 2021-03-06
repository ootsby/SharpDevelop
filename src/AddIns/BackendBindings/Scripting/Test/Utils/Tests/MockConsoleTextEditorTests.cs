﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockConsoleTextEditorTests
	{
		FakeConsoleTextEditor textEditor;
		
		[SetUp]
		public void Init()
		{
			textEditor = new FakeConsoleTextEditor();
		}
		
		[Test]
		public void TextReturnsEmptyStringByDefault()
		{
			Assert.AreEqual(String.Empty, textEditor.Text);
		}
		
		[Test]
		public void TextReturnsTextWritten()
		{
			textEditor.Write("abc");
			Assert.AreEqual("abc", textEditor.Text);
		}
		
		[Test]
		public void ColumnReturnsPositionAfterTextWritten()
		{
			textEditor.Write("ab");
			Assert.AreEqual(2, textEditor.Column);
		}
		
		[Test]
		public void TextReturnsAllTextWritten()
		{
			textEditor.Write("a");
			textEditor.Write("b");
			Assert.AreEqual("ab", textEditor.Text);
		}
		
		[Test]
		public void ColumnReturnsPositionAfterTextWhenWriteCalledTwice()
		{
			textEditor.Write("a");
			textEditor.Write("bb");
			Assert.AreEqual(3, textEditor.Column);
		}
		
		[Test]
		public void IsWriteCalledReturnsTrueAfterWriteMethodCalled()
		{
			textEditor.Write("a");
			Assert.IsTrue(textEditor.IsWriteCalled);
		}
		
		[Test]
		public void SettingTextSetsColumnToPositionAfterText()
		{
			textEditor.Text = "abc";
			Assert.AreEqual(3, textEditor.Column);
		}
		
		[Test]
		public void SettingTextSetsSelectionStartToPositionAfterText()
		{
			textEditor.Text = "abc";
			Assert.AreEqual(3, textEditor.SelectionStart);
		}
		
		[Test]
		public void TotalLinesEqualsOneByDefault()
		{
			Assert.AreEqual(1, textEditor.TotalLines);
		}
		
		[Test]
		public void TotalLinesReturnsTwoWhenTextSetWithTwoLines()
		{
			textEditor.Text = 
				"ab\n" +
				"cd";
			Assert.AreEqual(2, textEditor.TotalLines);
		}
		
		[Test]
		public void ColumnReturnsPositionAfterLastCharacterOnLastLineWhenTextSetWithTwoLines()
		{
			textEditor.Text = 
				"ab\n" +
				"c";
			Assert.AreEqual(1, textEditor.Column);
		}
		
		[Test]
		public void SelectionStartReturnsPositionAfterLastCharacterOnLastLineWhenTextSetWithTwoLines()
		{
			textEditor.Text = 
				"ab\n" +
				"c";
			Assert.AreEqual(1, textEditor.SelectionStart);
		}
		
		[Test]
		public void NoTextAddedWhenPreviewKeyDownEventHandlerReturnsTrue()
		{
			textEditor.PreviewKeyDown += delegate(object source, ConsoleTextEditorKeyEventArgs e) { 
				e.Handled = true;
			};
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			Assert.AreEqual(String.Empty, textEditor.Text);
		}
		
		[Test]
		public void RaisePreviewKeyDownEventReturnsTrueWhenPreviewDownHandlerReturnsTrue()
		{
			textEditor.PreviewKeyDown += delegate(object source, ConsoleTextEditorKeyEventArgs e) { 
				e.Handled = true;
			};
			Assert.IsTrue(textEditor.RaisePreviewKeyDownEvent(System.Windows.Input.Key.A));
		}
		
		[Test]
		public void KeyPressWhenCursorInsideTextInsertsTextAtCursor()
		{
			textEditor.Text = "abcdef";
			textEditor.Column = 3;
			textEditor.RaisePreviewKeyDownEvent(Key.X);
			Assert.AreEqual("abcXdef", textEditor.Text);
		}
		
		[Test]
		public void GetLineWithIndex2ReturnsLastLineForThreeLinesOfText()
		{
			textEditor.Text = 
				"abc\n" +
				"def\n" +
				"ghi";
			Assert.AreEqual("ghi", textEditor.GetLine(2));
		}
		
		[Test]
		public void ReplaceMethodReplacesTextOnLastLine()
		{
			textEditor.Text =
				"1\n" +
				"2\n" +
				"abcdef";
			
			int index = 1;
			int lengthToRemove = 4;
			string newText = "123";
			textEditor.Replace(index, lengthToRemove, newText);
			
			string expectedText = 
				"1\n" +
				"2\n" +
				"a123f";

			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void LeftCursorDialogKeyPressMovesSelectionStartToLeft()
		{
			textEditor.Text = "abc";
			textEditor.SelectionStart = 3;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			Assert.AreEqual(2, textEditor.SelectionStart);
		}
		
		[Test]
		public void LeftCursorDialogKeyPressMovesCursorToLeft()
		{
			textEditor.Text = "abc";
			textEditor.SelectionStart = 3;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			Assert.AreEqual(2, textEditor.Column);
		}
		
		[Test]
		public void CursorPositionUnchangedWhenDialogKeyPressEventHandlerReturnsTrue()
		{
			textEditor.Text = "abc";
			textEditor.Column = 3;
			textEditor.PreviewKeyDown += delegate(object source, ConsoleTextEditorKeyEventArgs e) {
				e.Handled = true;
			};
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left);
			Assert.AreEqual(3, textEditor.Column);
		}
		
		[Test]
		public void RaiseDialogKeyPressEventReturnsTruedWhenDialogKeyPressEventHandlerReturnsTrue()
		{
			textEditor.Text = "abc";
			textEditor.Column = 3;
			textEditor.PreviewKeyDown += delegate(object source, ConsoleTextEditorKeyEventArgs e) {
				e.Handled = true;
			};
			Assert.IsTrue(textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Left));
		}
				
		[Test]
		public void RightCursorDialogKeyPressMovesCursorToRight()
		{
			textEditor.Text = "abc";
			textEditor.Column = 0;
			textEditor.SelectionStart = 0;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Right);
			Assert.AreEqual(1, textEditor.Column);
		}
		
		[Test]
		public void RightCursorDialogKeyPressMovesSelectionStartToRight()
		{
			textEditor.Text = "abc";
			textEditor.Column = 0;
			textEditor.SelectionStart = 0;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Right);
			Assert.AreEqual(1, textEditor.SelectionStart);
		}
		
		[Test]
		public void BackspaceDialogKeyPressRemovesLastCharacterFromLine()
		{
			textEditor.Text = "abc";
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			Assert.AreEqual("ab", textEditor.Text);
		}
		
		[Test]
		public void BackspaceDialogKeyPressRemovesSelectedTextFromLine()
		{
			textEditor.Text = "abcd";
			textEditor.SelectionStart = 1;
			textEditor.SelectionLength = 2;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Back);
			Assert.AreEqual("ad", textEditor.Text);
		}
		
		[Test]
		public void EnterDialogKeyPressCreatesNewLine()
		{
			textEditor.Text = "abc";
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			string expectedText = "abc\r\n";
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void ColumnResetToZeroAfterEnterDialogKeyPress()
		{
			textEditor.Text = "abc";
			textEditor.Column = 3;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			Assert.AreEqual(0, textEditor.Column);
		}
		
		[Test]
		public void SelectionStartResetToZeroAfterEnterDialogKeyPress()
		{
			textEditor.Text = "abc";
			textEditor.SelectionStart = 3;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			Assert.AreEqual(0, textEditor.Column);
		}
		
		[Test]
		public void EnterDialogKeyPressDoesNothingWhenKeyPressEventHandlerReturnsTrue()
		{
			textEditor.Text = "abc";
			textEditor.PreviewKeyDown += delegate(object source, ConsoleTextEditorKeyEventArgs e) { 
				if (e.Key == Key.Enter) { 
					e.Handled = true;
				} else {
					e.Handled = false;
				}
			};
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			Assert.AreEqual("abc", textEditor.Text);
		}
		
		[Test]
		public void EnterDialogKeyPressIncreasesTotalLineCountByOne()
		{
			textEditor.Text = "abc";
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			Assert.AreEqual(2, textEditor.TotalLines);
		}

		[Test]
		public void EnterDialogKeyPressIncreasesCurrentLineByOne()
		{
			textEditor.Text = "abc";
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			Assert.AreEqual(1, textEditor.Line);
		}
		
		[Test]
		public void EnterDialogKeyPressInMiddleOfLinePutsTextAfterCursorOnNewLine()
		{
			textEditor.Text = "abcd";
			textEditor.Column = 2;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			string expectedText = 
				"ab\r\n" +
				"cd";
			Assert.AreEqual(expectedText, textEditor.Text);
		}
		
		[Test]
		public void EnterDialogKeyPressInMiddleOfLinePutsSelectionStartAtEndOfNewLineGenerated()
		{
			textEditor.Text = "abcd";
			textEditor.Column = 3;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			Assert.AreEqual(1, textEditor.SelectionStart);
		}

		[Test]
		public void EnterDialogKeyPressInMiddleOfLinePutsCursorAtEndOfNewLineGenerated()
		{
			textEditor.Text = "abcd";
			textEditor.Column = 3;
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			Assert.AreEqual(1, textEditor.Column);
		}
	}
}
