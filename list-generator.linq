<Query Kind="Program" />

void Main()
{
	Console.WriteLine("Enter your list as one line, with each item seperated by '~'");
	Console.WriteLine("Set 'isOrdered' to 'true' for ordered lists.\n");
	
	var isOrdered = false;
	
	var input = Console.ReadLine();
	var result = "";
	
	if (isOrdered)
		result += "<ol><li>";
	else
		result += "<ul><li>";
	
	foreach (var c in input)
	{
		if (c == '~')
		{
			result += "</li><li>";
		}
		else
			result += c;
	}

	if (isOrdered)
		result += "</li></ol>";
	else
		result += "</li></ul>";

	Console.WriteLine("\n---------------------------------------------------\n");

	result.Dump();
}

