using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ICSharpCode.AvalonEdit.Indentation.CSharp;

internal sealed class IndentationReformatter
{
	private struct Block
	{
		public string OuterIndent;

		public string InnerIndent;

		public string LastWord;

		public char Bracket;

		public bool Continuation;

		public int OneLineBlock;

		public int PreviousOneLineBlock;

		public int StartLine;

		public void ResetOneLineBlock()
		{
			PreviousOneLineBlock = OneLineBlock;
			OneLineBlock = 0;
		}

		public void Indent(IndentationSettings set)
		{
			Indent(set.IndentString);
		}

		public void Indent(string indentationString)
		{
			OuterIndent = InnerIndent;
			InnerIndent += indentationString;
			Continuation = false;
			ResetOneLineBlock();
			LastWord = "";
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[Block StartLine={0}, LastWord='{1}', Continuation={2}, OneLineBlock={3}, PreviousOneLineBlock={4}]", StartLine, LastWord, Continuation, OneLineBlock, PreviousOneLineBlock);
		}
	}

	private StringBuilder wordBuilder;

	private Stack<Block> blocks;

	private Block block;

	private bool inString;

	private bool inChar;

	private bool verbatim;

	private bool escape;

	private bool lineComment;

	private bool blockComment;

	private char lastRealChar;

	public void Reformat(IDocumentAccessor doc, IndentationSettings set)
	{
		Init();
		while (doc.MoveNext())
		{
			Step(doc, set);
		}
	}

	public void Init()
	{
		wordBuilder = new StringBuilder();
		blocks = new Stack<Block>();
		block = default(Block);
		block.InnerIndent = "";
		block.OuterIndent = "";
		block.Bracket = '{';
		block.Continuation = false;
		block.LastWord = "";
		block.OneLineBlock = 0;
		block.PreviousOneLineBlock = 0;
		block.StartLine = 0;
		inString = false;
		inChar = false;
		verbatim = false;
		escape = false;
		lineComment = false;
		blockComment = false;
		lastRealChar = ' ';
	}

	public void Step(IDocumentAccessor doc, IndentationSettings set)
	{
		string text = doc.Text;
		if (set.LeaveEmptyLines && text.Length == 0)
		{
			return;
		}
		text = text.TrimStart();
		StringBuilder stringBuilder = new StringBuilder();
		if (text.Length == 0)
		{
			if (!blockComment && (!inString || !verbatim))
			{
				stringBuilder.Append(this.block.InnerIndent);
				stringBuilder.Append(Repeat(set.IndentString, this.block.OneLineBlock));
				if (this.block.Continuation)
				{
					stringBuilder.Append(set.IndentString);
				}
				if (doc.Text != stringBuilder.ToString())
				{
					doc.Text = stringBuilder.ToString();
				}
			}
			return;
		}
		if (TrimEnd(doc))
		{
			text = doc.Text.TrimStart();
		}
		Block block = this.block;
		bool flag = blockComment;
		bool flag2 = inString && verbatim;
		lineComment = false;
		inChar = false;
		escape = false;
		if (!verbatim)
		{
			inString = false;
		}
		lastRealChar = '\n';
		char c = ' ';
		char c2 = text[0];
		for (int i = 0; i < text.Length; i++)
		{
			if (lineComment)
			{
				break;
			}
			char c3 = c;
			c = c2;
			c2 = ((i + 1 >= text.Length) ? '\n' : text[i + 1]);
			if (escape)
			{
				escape = false;
				continue;
			}
			switch (c)
			{
			case '/':
				if (blockComment && c3 == '*')
				{
					blockComment = false;
				}
				if (!inString && !inChar)
				{
					if (!blockComment && c2 == '/')
					{
						lineComment = true;
					}
					if (!lineComment && c2 == '*')
					{
						blockComment = true;
					}
				}
				break;
			case '#':
				if (!inChar && !blockComment && !inString)
				{
					lineComment = true;
				}
				break;
			case '"':
				if (inChar || lineComment || blockComment)
				{
					break;
				}
				inString = !inString;
				if (!inString && verbatim)
				{
					if (c2 == '"')
					{
						escape = true;
						inString = true;
					}
					else
					{
						verbatim = false;
					}
				}
				else if (inString && c3 == '@')
				{
					verbatim = true;
				}
				break;
			case '\'':
				if (!inString && !lineComment && !blockComment)
				{
					inChar = !inChar;
				}
				break;
			case '\\':
				if ((inString && !verbatim) || inChar)
				{
					escape = true;
				}
				break;
			}
			if (lineComment || blockComment || inString || inChar)
			{
				if (wordBuilder.Length > 0)
				{
					this.block.LastWord = wordBuilder.ToString();
				}
				wordBuilder.Length = 0;
				continue;
			}
			if (!char.IsWhiteSpace(c) && c != '[' && c != '/' && this.block.Bracket == '{')
			{
				this.block.Continuation = true;
			}
			if (char.IsLetterOrDigit(c))
			{
				wordBuilder.Append(c);
			}
			else
			{
				if (wordBuilder.Length > 0)
				{
					this.block.LastWord = wordBuilder.ToString();
				}
				wordBuilder.Length = 0;
			}
			switch (c)
			{
			case '{':
				this.block.ResetOneLineBlock();
				blocks.Push(this.block);
				this.block.StartLine = doc.LineNumber;
				if (this.block.LastWord == "switch")
				{
					this.block.Indent(set.IndentString + set.IndentString);
				}
				else
				{
					this.block.Indent(set);
				}
				this.block.Bracket = '{';
				break;
			case '}':
				while (this.block.Bracket != '{' && blocks.Count != 0)
				{
					this.block = blocks.Pop();
				}
				if (blocks.Count != 0)
				{
					this.block = blocks.Pop();
					this.block.Continuation = false;
					this.block.ResetOneLineBlock();
				}
				break;
			case '(':
			case '[':
				blocks.Push(this.block);
				if (this.block.StartLine == doc.LineNumber)
				{
					this.block.InnerIndent = this.block.OuterIndent;
				}
				else
				{
					this.block.StartLine = doc.LineNumber;
				}
				this.block.Indent(Repeat(set.IndentString, block.OneLineBlock) + (block.Continuation ? set.IndentString : "") + ((i == text.Length - 1) ? set.IndentString : new string(' ', i + 1)));
				this.block.Bracket = c;
				break;
			case ')':
				if (blocks.Count != 0 && this.block.Bracket == '(')
				{
					this.block = blocks.Pop();
					if (IsSingleStatementKeyword(this.block.LastWord))
					{
						this.block.Continuation = false;
					}
				}
				break;
			case ']':
				if (blocks.Count != 0 && this.block.Bracket == '[')
				{
					this.block = blocks.Pop();
				}
				break;
			case ',':
			case ';':
				this.block.Continuation = false;
				this.block.ResetOneLineBlock();
				break;
			case ':':
				if (this.block.LastWord == "case" || text.StartsWith("case ", StringComparison.Ordinal) || text.StartsWith(this.block.LastWord + ":", StringComparison.Ordinal))
				{
					this.block.Continuation = false;
					this.block.ResetOneLineBlock();
				}
				break;
			}
			if (!char.IsWhiteSpace(c))
			{
				lastRealChar = c;
			}
		}
		if (wordBuilder.Length > 0)
		{
			this.block.LastWord = wordBuilder.ToString();
		}
		wordBuilder.Length = 0;
		if (flag2 || (flag && text[0] != '*') || doc.Text.StartsWith("//\t", StringComparison.Ordinal) || doc.Text == "//")
		{
			return;
		}
		if (text[0] == '}')
		{
			stringBuilder.Append(block.OuterIndent);
			block.ResetOneLineBlock();
			block.Continuation = false;
		}
		else
		{
			stringBuilder.Append(block.InnerIndent);
		}
		if (stringBuilder.Length > 0 && block.Bracket == '(' && text[0] == ')')
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		else if (stringBuilder.Length > 0 && block.Bracket == '[' && text[0] == ']')
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		if (text[0] == ':')
		{
			block.Continuation = true;
		}
		else if (lastRealChar == ':' && stringBuilder.Length >= set.IndentString.Length)
		{
			if (this.block.LastWord == "case" || text.StartsWith("case ", StringComparison.Ordinal) || text.StartsWith(this.block.LastWord + ":", StringComparison.Ordinal))
			{
				stringBuilder.Remove(stringBuilder.Length - set.IndentString.Length, set.IndentString.Length);
			}
		}
		else if (lastRealChar == ')')
		{
			if (IsSingleStatementKeyword(this.block.LastWord))
			{
				this.block.OneLineBlock++;
			}
		}
		else if (lastRealChar == 'e' && this.block.LastWord == "else")
		{
			this.block.OneLineBlock = Math.Max(1, this.block.PreviousOneLineBlock);
			this.block.Continuation = false;
			block.OneLineBlock = this.block.OneLineBlock - 1;
		}
		if (doc.IsReadOnly)
		{
			if (!block.Continuation && block.OneLineBlock == 0 && block.StartLine == this.block.StartLine && this.block.StartLine < doc.LineNumber && lastRealChar != ':')
			{
				stringBuilder.Length = 0;
				text = doc.Text;
				for (int j = 0; j < text.Length && char.IsWhiteSpace(text[j]); j++)
				{
					stringBuilder.Append(text[j]);
				}
				if (flag && stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ' ')
				{
					stringBuilder.Length--;
				}
				this.block.InnerIndent = stringBuilder.ToString();
			}
			return;
		}
		if (text[0] != '{')
		{
			if (text[0] != ')' && block.Continuation && block.Bracket == '{')
			{
				stringBuilder.Append(set.IndentString);
			}
			stringBuilder.Append(Repeat(set.IndentString, block.OneLineBlock));
		}
		if (flag)
		{
			stringBuilder.Append(' ');
		}
		if (stringBuilder.Length != doc.Text.Length - text.Length || !doc.Text.StartsWith(stringBuilder.ToString(), StringComparison.Ordinal) || char.IsWhiteSpace(doc.Text[stringBuilder.Length]))
		{
			doc.Text = stringBuilder.ToString() + text;
		}
	}

	private static string Repeat(string text, int count)
	{
		switch (count)
		{
		case 0:
			return string.Empty;
		case 1:
			return text;
		default:
		{
			StringBuilder stringBuilder = new StringBuilder(text.Length * count);
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}
		}
	}

	private static bool IsSingleStatementKeyword(string keyword)
	{
		switch (keyword)
		{
		case "do":
		case "if":
		case "using":
		case "while":
		case "for":
		case "foreach":
		case "lock":
			return true;
		default:
			return false;
		}
	}

	private static bool TrimEnd(IDocumentAccessor doc)
	{
		string text = doc.Text;
		if (!char.IsWhiteSpace(text[text.Length - 1]))
		{
			return false;
		}
		if (text.EndsWith("// ", StringComparison.Ordinal) || text.EndsWith("* ", StringComparison.Ordinal))
		{
			return false;
		}
		doc.Text = text.TrimEnd();
		return true;
	}
}
