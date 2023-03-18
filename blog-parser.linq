<Query Kind="Program" />

void Main()
{
	PrintProtocol();
	
	Console.WriteLine("Enter an input file path:");
	var inputPath = Console.ReadLine();
	inputPath = @"C:\Users\rgay\Documents\LINQPad Queries\personal\blog-parser\test.txt"; // FOR TESTING
	Console.WriteLine($"\tinput path: {inputPath}");

	Console.WriteLine("Enter an output file path:");
	var outputPath = Console.ReadLine();
	outputPath = @"C:\Users\rgay\Documents\LINQPad Queries\personal\blog-parser\testResult.txt"; // FOR TESTING
	Console.WriteLine($"\toutput path: {outputPath}");
	// TODO: write to a text file
	
	var lines = System.IO.File.ReadAllLines(inputPath);
	
	var output = "";
	
	foreach (var line in lines)
	{
		if (line.Length == 0)
			continue;
		if (line.Length >= 7 && line[0..6] == "title:") // Length must be 'title:' + at least 1 title char
		{
			var title = line[6..];
			output += $"<h3 className=\"text-center\">{title}</h3>";
		}
		else if (line.Length >= 7 && line[0..6] == "image:") // Length must be 'image:' + at least 1 title char
		{
			var imagePath = line[6..];
			output += "<div className=\"text-center\">\n\t<figure className=\"figure\" "
			+ "style={{ maxWidth:\"50%\", margin:\"auto\" }}>\n\t\t<img className=\"img-fluid\" "
			+ $"src=\"{imagePath}\" alt=\"\" />" // TODO: add the alt text?
			+ "\n\t</ figure>\n</ div>";
		}
		else if (line.Length >= 9 && line[0..8] == "gallery:") // Length must be 'gallery:' + at least 1 title char
		{
			var urls = line[8..];
			List<string> galleryImages = new();
			var url = "";
			
			for (int i = 0; i < urls.Length; i++)
			{
				if (urls[i] == ';')
				{
					galleryImages.Add(url);
					url = "";
				}
				else
				{
					url += urls[i];
				}
			}
			if (url != "") // Allows you to not have to end with ';'
				galleryImages.Add(url);
						
			output += "<Carousel>\n";
			
			foreach (var image in galleryImages)
			{
				output +=
$"""
	<Carousel.Item>
	    <img className="d-block w-100" src="{image}" alt="" />
	    <Carousel.Caption>
	        <p className="d-inline-flex px-2 mb-4 bg-dark rounded"></p>
	    </Carousel.Caption>
	</Carousel.Item>
"""; // TODO: Add alt text and caption?
			output += "\n";
			}

			output += "\n</Carousel>";
		}
		else
		{
			output += $"<p>\n\t{line}\n</p>";
		}
		
		output += "\n\n"; // Visual spacing
	}
	
	Console.WriteLine("\n---------------------------------------------------\n");
	output.Dump();

	System.IO.File.WriteAllText(outputPath, output);
}

private void PrintProtocol()
{
	Console.WriteLine("Blog Parser:");
	Console.WriteLine("");
	Console.WriteLine("To add a title            -> title:Example Title");
	Console.WriteLine("To add an image           -> image:path-url/image.jpg");
	Console.WriteLine("To add an image gallery   -> image:path-url/image1.jpg;path-url/image2.jpg");
	Console.WriteLine("To add a paragraph        -> Just write some text and seperate it like a normal paragraph!");
	Console.WriteLine("");
}


