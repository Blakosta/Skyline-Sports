using System.Collections.Generic;

namespace Skyline_Sports_Project.Engine;

public class ReleaseQuote
{
	public List<int> Amounts = new List<int>();

	public int Total;

	public int Saved;

	public string Label = "Расторжение";

	public string Verb = "освободили";

	public string Reason = "Освобождён";

	public string Short = "";

	public string Rule = "";

	public bool UsesBuyout;

	public bool CanStretch;
}
