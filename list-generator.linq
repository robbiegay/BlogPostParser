<Query Kind="Program" />

void Main()
{
	Console.WriteLine("Enter your list as one line, with each item seperated by '~'");
	Console.WriteLine("Set 'isOrdered' to 'true' for ordered lists.\n");
	
	var isOrdered = false;

	while (true)
	{
		Console.WriteLine($"\n\nIsOrdered: {isOrdered}");
		
		var input = Console.ReadLine();
		//Console.Clear();

		if (input == "set:t")
		{
			isOrdered = true;
			continue;
		}
		else if (input == "set:f")
		{
			isOrdered = false;
			continue;
		}
		
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
}

