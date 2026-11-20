using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Skyline_Sports_Project.Engine;

public class JsonBuilder
{
	private readonly StringBuilder _sb = new StringBuilder();

	private readonly List<bool> _needsComma = new List<bool>();

	private bool _afterName;

	public JsonBuilder BeginObject()
	{
		ValueStart();
		_sb.Append('{');
		_needsComma.Add(item: false);
		return this;
	}

	public JsonBuilder EndObject()
	{
		Pop();
		_sb.Append('}');
		return this;
	}

	public JsonBuilder BeginArray()
	{
		ValueStart();
		_sb.Append('[');
		_needsComma.Add(item: false);
		return this;
	}

	public JsonBuilder EndArray()
	{
		Pop();
		_sb.Append(']');
		return this;
	}

	private void Pop()
	{
		if (_needsComma.Count > 0)
		{
			_needsComma.RemoveAt(_needsComma.Count - 1);
		}
	}

	public JsonBuilder Name(string name)
	{
		Comma();
		AppendString(name);
		_sb.Append(':');
		_afterName = true;
		return this;
	}

	public JsonBuilder Value(string value)
	{
		ValueStart();
		if (value == null)
		{
			_sb.Append("null");
		}
		else
		{
			AppendString(value);
		}
		return this;
	}

	public JsonBuilder Value(int value)
	{
		ValueStart();
		_sb.Append(value.ToString(CultureInfo.InvariantCulture));
		return this;
	}

	public JsonBuilder Value(bool value)
	{
		ValueStart();
		_sb.Append(value ? "true" : "false");
		return this;
	}

	public JsonBuilder Value(double value)
	{
		ValueStart();
		_sb.Append(value.ToString("0.###", CultureInfo.InvariantCulture));
		return this;
	}

	public JsonBuilder Prop(string name, string value)
	{
		return Name(name).Value(value);
	}

	public JsonBuilder Prop(string name, int value)
	{
		return Name(name).Value(value);
	}

	public JsonBuilder Prop(string name, bool value)
	{
		return Name(name).Value(value);
	}

	public override string ToString()
	{
		return _sb.ToString();
	}

	private void ValueStart()
	{
		if (_afterName)
		{
			_afterName = false;
		}
		else
		{
			Comma();
		}
	}

	private void Comma()
	{
		int top = _needsComma.Count - 1;
		if (top >= 0)
		{
			if (_needsComma[top])
			{
				_sb.Append(',');
			}
			_needsComma[top] = true;
		}
	}

	private void AppendString(string s)
	{
		_sb.Append('"');
		foreach (char c in s)
		{
			switch (c)
			{
			case '"':
				_sb.Append("\\\"");
				continue;
			case '\\':
				_sb.Append("\\\\");
				continue;
			case '\n':
				_sb.Append("\\n");
				continue;
			case '\r':
				_sb.Append("\\r");
				continue;
			case '\t':
				_sb.Append("\\t");
				continue;
			}
			if (c < ' ')
			{
				StringBuilder stringBuilder = _sb.Append("\\u");
				int num = c;
				stringBuilder.Append(num.ToString("x4"));
			}
			else
			{
				_sb.Append(c);
			}
		}
		_sb.Append('"');
	}
}
