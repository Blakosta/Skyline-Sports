namespace Skyline_Sports_Project.Engine;

public class CommandResult
{
	public bool Ok;

	public string Message = "";

	public static CommandResult Success(string message)
	{
		return new CommandResult
		{
			Ok = true,
			Message = (message ?? "")
		};
	}

	public static CommandResult Fail(string message)
	{
		return new CommandResult
		{
			Ok = false,
			Message = (message ?? "")
		};
	}
}
