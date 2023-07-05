using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using WebsiteCrawler.Infrastructure.entity;
using WebsiteCrawler.Infrastructure.interfaces;

namespace WebsiteCrawler.Service
{
	public class JSONResultFormatter : ICrawlerResultsFormatter
	{
		public void WriteResults(Stream output, List<SearchResult> searchResults)
		{
			//string resultJSON = JsonSerializer.Serialize(searchResults);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(searchResults, options);

            Console.WriteLine(jsonString);
        }
	}
}

