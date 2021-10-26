/// <summary>
/// Returns the available commands
/// </summary>
public class HelpCommand : ICommand
{
	private readonly string[] commands;

	public HelpCommand(string[] commands)
	{
		this.commands = commands;
	}

	public string GetString(params string[] parameters)
	{
		return string.Join(",\n", commands);
	}
}
