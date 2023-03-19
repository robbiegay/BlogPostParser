<Query Kind="Program" />

void Main()
{
	PrintProtocol();
	
	var filePath = GetFilePath();
	var fileName = GetFileName();
	
	var lines = System.IO.File.ReadAllLines(filePath + fileName);
	var output = "";
	
	foreach (var line in lines)
	{
		if (line.Length == 0)
			continue;
		if (line.Length >= 7 && line[0..6] == "title:") // Length must be 'title:' + at least 1 title char
		{
			output += ParseTitle(line[6..]);
		}
		else if (line.Length >= 7 && line[0..6] == "image:") // Length must be 'image:' + at least 1 title char
		{
			output += ParseImage(line[6..]);
		}
		else if (line.Length >= 9 && line[0..8] == "gallery:") // Length must be 'gallery:' + at least 1 title char
		{
			output += ParseGallery(line[8..]);
		}
		else
		{
			output += $"<p>\n\t{line}\n</p>";
		}
		
		output += "\n\n"; // Visual spacing
	}
	
	Console.WriteLine("\n---------------------------------------------------\n");
	output.Dump();

	System.IO.File.WriteAllText(GetOutputFileName(filePath, fileName), output);
}

private string GetFilePath()
{
	Console.WriteLine("Enter an input file path (without file name):");
	var filePath = Console.ReadLine();
	filePath = @"C:\Users\rgay\Documents\LINQPad Queries\personal\BlogPosts"; // FOR TESTING

	if (filePath[filePath.Length - 1] != '\\')
		filePath += "\\";
	
	Console.WriteLine($"\tinput path: {filePath}");
	
	return filePath;
}

private string GetFileName()
{
	Console.WriteLine("Enter the name of your blog file:");
	var fileName = Console.ReadLine();
	fileName = @"StartingToBlogAgain.txt"; // FOR TESTING
	Console.WriteLine($"\toutput path: {fileName}");

	return fileName;
}

private string GetOutputFileName(string filePath, string fileName)
{
	return filePath + "Parsed_" + fileName;
}

private string ParseTitle(string input)
{
	return $"<h3 className=\"text-center\">{input}</h3>";
}

private string ParseImage(string input)
{
	var imageItemType = ItemType.Url;

	var url = "";
	var alt = "";
	var description = "";

	for (int i = 0; i < input.Length; i++)
	{
		var item = input[i];
		
		if (item == '~')
		{
			imageItemType = ItemType.Alt;
		}
		else if (item == '*')
		{
			imageItemType = ItemType.Description;
		}
		else if (imageItemType == ItemType.Alt)
		{
			alt += item;
		}
		else if (imageItemType == ItemType.Description)
		{
			description += item;
		}
		else
		{
			url += item;
		}
	}

	return    "<div className=\"text-center\">"
				+ "\n\t<figure className=\"figure\" style={{ maxWidth:\"50%\", margin:\"auto\" }}>"
					+ $"\n\t\t<img className=\"img-fluid\" src=\"{url}\" alt=\"{alt}\" />"
					+ $"\n\t\t<figcaption className=\"figure-caption text-center\">{description}</ figcaption>\""
				+ "\n\t</figure>"
			+ "\n</div>";
}

private enum ItemType
{
	Url,
	Alt,
	Description
}

private class GalleryItem
{
	public string Url { get; init; }
	public string Alt { get; init; }
	public string Description { get; init; }
}

private string ParseGallery(string input)
{
	var output = "";
	var galleryItemType = ItemType.Url;
	
	List<GalleryItem> galleryItems = new();
	var url = "";
	var alt = "";
	var description = "";

	for (int i = 0; i < input.Length; i++)
	{
		var item = input[i];
		
		if (item == ';')
		{
			galleryItems.Add(new GalleryItem() 
			{
				Url = url, 
				Alt = alt, 
				Description = description
			});
			
			galleryItemType = ItemType.Url;
			url = "";
			alt = "";
			description = "";
		}
		else if (item == '~')
		{
			galleryItemType = ItemType.Alt;
		}
		else if (item == '*')
		{
			galleryItemType = ItemType.Description;
		}
		else
		{
			switch (galleryItemType)
			{
				case ItemType.Url:
					url += item;
					break;
				case ItemType.Alt:
					alt += item;
					break;
				case ItemType.Description:
					description += item;
					break;
			}
		}
	}

	output += "<Carousel>\n";

	foreach (var galleryItem in galleryItems)
	{
		output +=
$"""
	<Carousel.Item>
	    <img className="d-block w-100" src="{galleryItem.Url}" alt="{galleryItem.Alt}" />
	    <Carousel.Caption>
	        <p className="d-inline-flex px-2 mb-4 bg-dark rounded">{galleryItem.Description}</p>
	    </Carousel.Caption>
	</Carousel.Item>
""";
		output += "\n";
	}

	output += "\n</Carousel>";
	
	return output;
}

private void PrintProtocol()
{
	Console.WriteLine("Blog Parser:");
	Console.WriteLine("");
	Console.WriteLine("To add a title \n\t-> title:Example Title");
	Console.WriteLine("To add an image (~ = alt text) \n\t-> image:path-url/image.jpg~alt text");
	Console.WriteLine("To add an image gallery (must end each item with ';', ~ = alt text, * = description) \n\t-> gallery:path-url/image1.jpg~alt text*description;path-url/image2.jpg;");
	Console.WriteLine("To add a paragraph \n\t-> Just write some text and seperate it like a normal paragraph!");
	Console.WriteLine("");
}


