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
		else if (line.Length >= 5 && line[0..5] == "line:")
		{
			output += $"\n<br />\n<hr />\n<br />\n";
		}
		else if (line.Length >= 8 && line[0..7] == "rating:")
		{
			output += ParseRating(line[7..]);
		}
		else if (line.Length >= 7 && line[0..6] == "quote:")
		{
			output += ParseQuote(line[6..]);
		}
		else if (line.Length >= 6 && line[0..5] == "list:")
		{
			if (line[5] == '^')
				output += ParseList(line[6..], true);
			else
				output += ParseList(line[5..]);
		}
		else
		{
			if (string.IsNullOrWhiteSpace(line))
				output += "<br />";
			else
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
	var filePath ="";
	//filePath = Console.ReadLine();
	filePath = @"C:\Users\rgay\Documents\LINQPad Queries\personal\blog-parser\BlogPosts\"; // FOR TESTING

	if (filePath[filePath.Length - 1] != '\\')
		filePath += "\\";
	
	Console.WriteLine($"\tinput path: {filePath}");
	
	return filePath;
}

private string GetFileName()
{
	Console.WriteLine("Enter the name of your blog file:");
	var fileName = "";
	//fileName = Console.ReadLine();
	fileName = @"DDIA.txt"; // FOR TESTING
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

	return  
		"<br />\n\n"
		
	    + "<div className=\"text-center\">"
			+ "\n\t<figure className=\"figure\">"
				+ $"\n\t\t<img className=\"img-fluid\" src=\"/blogAssets/img/2023/{url}\" alt=\"{alt}\" />"
				+ $"\n\t\t<figcaption className=\"figure-caption text-center\">{description}</ figcaption>"
			+ "\n\t</figure>"
		+ "\n</div>"
			
		+ "\n\n<br />";
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

	output += "<br>\n\n" + "<Carousel>\n";

	for (var i = 0; i < galleryItems.Count; i++)
	{
		var galleryItem = galleryItems[i];
		
		output +=
$"""
	<Carousel.Item>
	    <img className="d-block w-100" src=/"/blogAssets/img/2023/"{galleryItem.Url}" alt="{galleryItem.Alt}" />
	    <Carousel.Caption>
	        <p className="d-inline-flex px-2 mb-4 bg-dark rounded">{galleryItem.Description}</p>
	    </Carousel.Caption>
	</Carousel.Item>
""";
		if (i < galleryItems.Count - 1)
			output += "\n";
	}

	output += "\n</Carousel>" + "\n\n</br>";
	
	return output;
}

private string ParseRating(string rating)
{
	var output = "";
	
	int parsedRating;
	int.TryParse(rating, out parsedRating);
	
	output += "<p>\n\t<b><u>Rating</u></b>:\n</p>\n";
	
	output += "\n<p>\n\t";
	
	for (int i = 0; i < 5; i++)
	{
		if (parsedRating-- > 0)
			output += "&#11088; ";
		else
			output += "&#9733; ";
	}
	
	output += "\n</p>";
	
	return output;
}

private enum QuoteType
{
	Quote,
	Author,
	Title
}

public string ParseQuote(string input)
{
	QuoteType quoteType = QuoteType.Quote;
	var quote = "";
	var author = "";
	var title = "";
	
	for (int i = 0; i < input.Length; i++)
	{
		var item = input[i];

		if (item == '~')
		{
			quoteType = QuoteType.Author;
		}
		else if (item == '*')
		{
			quoteType = QuoteType.Title;
		}
		else if (quoteType == QuoteType.Author)
		{
			author += item;
		}
		else if (quoteType == QuoteType.Title)
		{
			title += item;
		}
		else
		{
			quote += item;
		}
	}

	var output =
$"""
<blockquote class= "blockquote">
	<p class="text-muted">
		<small>
			"{quote}"
		</ small>
	</ p>
""";

if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(author))
{
		output += "\n";
		output += 
$"""
	<footer class="blockquote-footer"> {author}, <cite title="{title}">{title}</ cite></ footer>
""";
}

output += "\n</ blockquote>";

	return output;
}

public string ParseList(string input, bool isOrdered = false)
{
	var result = "";

	if (isOrdered)
		result += "<ol><li>";
	else
		result += "<ul><li>";

	for (int i = 0; i < input.Length; i++)
	{
		var c = input[i];
		
		if (c == '*')
		{
			result += "</li>";
			
			var subList = "";
			
			while (input[++i] != '*')
			{
				c = input[i];
				
				subList += c;
			}
			
			result += ParseList(subList, isOrdered);
			result += "<li>";
		}
		else if (c == '~')
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
		
	return result;
}

private void PrintProtocol()
{
	Console.WriteLine("Blog Parser:");
	Console.WriteLine("");
	Console.WriteLine("To add a title \n\t-> title:Example Title");
	Console.WriteLine("To add an image (~ = alt text) \n\t-> image:path-url/image.jpg~alt text");
	Console.WriteLine("To add an image gallery (must end each item with ';', ~ = alt text, * = description) \n\t-> gallery:path-url/image1.jpg~alt text*description;path-url/image2.jpg;");
	Console.WriteLine("To add a rating (max of 5) for a book review\n\t-> rating:4 (4/5 stars)");
	Console.WriteLine("To add a line break \n\t-> line:");
	Console.WriteLine("To add a quote \n\t-> quote:my quote...~Author*Title");
	Console.WriteLine("To add an unordered list \n\t-> list:item~item~item");
	Console.WriteLine("To add an ordered list \n\t-> list:^item~item~item");
	Console.WriteLine("To add an sub lists \n\t-> seperate items via *item~item*");
	Console.WriteLine("To add a paragraph \n\t-> Just write some text and seperate it like a normal paragraph!");
	Console.WriteLine("");
}


