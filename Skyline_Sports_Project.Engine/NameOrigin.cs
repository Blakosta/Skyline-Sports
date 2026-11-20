namespace Skyline_Sports_Project.Engine;

public sealed class NameOrigin
{
	public string Code;

	public string[] First;

	public string[] Last;

	public NameOrigin(string code, string[] first, string[] last)
	{
		Code = code;
		First = first;
		Last = last;
	}
}
