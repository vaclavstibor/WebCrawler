using System;
using System.Collections.Generic;

namespace WebsiteCrawler.Infrastructure.entity
{
    /// <summary>
    /// Represents a href of an anchor element and subsequent enrichment that was discovered while crawling a HTML content
    /// </summary>
    public class SearchResult
    {
        public SearchResult()
        {
        }

        /// <summary>
        /// Gets or sets the absolute transformation of the href of the anchor element
        /// </summary>
        public Uri AbsoluteLink { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the discovered href is a link to a domain outside the domain being searched
        /// </summary>
        public bool IsLinkExternalDomain { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the discovered href is an absolute or a relative link.
        /// </summary>
        public bool IsLinkFullyQualified { get; set; }

        /// <summary>
        /// Gets or sets a numeric value that determines how many pages deep was the container page from which the current anchor element was extracted
        /// E.g. All links on the landing page are at level=1
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Get or sets the unmodified href of the anchor that that was discovered from the HTML
        /// </summary>
        public string OriginalLink { get; set; }

        /// <summary>
        /// Gets or sets the Url of the parent web page page on which the current link was discovered
        /// </summary>
        public string ParentPageUrl { get; set; }

        public int Id { get; set; }

        public Uri Url { get; set; }

        public string Domain { get; set; }

        public DateTime CrawlTime { get; set; }

        public bool? RegExpMatch { get; set; }

        public List<int> Children { get; set; }

        public override string ToString()
        {
            return $"Parent={this.ParentPageUrl}    Child={this.AbsoluteLink}   Level={this.Level}, IsLinkFullyQualified={this.IsLinkFullyQualified}";
        }
    }
}

/*
namespace WebCrawler.DataAccessLayer.Models
{
    public class Node
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Domain { get; set; }
        public DateTime CrawlTime { get; set; }
        public bool? RegExpMatch { get; set; } // none RegExp filled -> null
        public virtual ICollection<Node>? Children { get; set; }
        public int WebsiteRecordId { get; set; }
    }
}
*/

/*
{
    "nodes": [
        {
        "id": 0,             
            "url": "https://www.google.com/bla", 
            "domain": "www.google.com",
            "crawlTime": "00:30",
            "regExpMatch": null, 
            "children": [
                {
            "id": 1,                    
                    "url":"https://www.google.com/bla/bla", 
                    "domain": "www.google.com",
                    "crawlTime":"01:30", 
                    "regExpMatch": null,                    
                    "children": null
                }, 
                {
            "id": 2,                    
                    "url":"https://www.google.com/bla/bla/bla", 
                    "domain": "www.google.com",
                    "crawlTime":"01:40", 
                    "regExpMatch": null,                    
                    "children": null
                },                 
                {
            "id": 3,  
                    "url": "http://www.twitter.com/bla",
                    "domain": "www.twitter.com",  
                    "crawlTime":"01:30", 
                    "regExpMatch": null,
                    "children": null
                } 
            ]
        },
        {
        "id": 1,                    
            "url":"https://www.google.com/bla/bla", 
            "domain": "www.google.com",
            "crawlTime":"01:30", 
            "regExpMatch": null,                    
            "children": null
        },
        {
        "id": 2,                    
            "url":"https://www.google.com/bla/bla/bla", 
            "domain": "www.google.com",
            "crawlTime":"01:40", 
            "regExpMatch": null,                    
            "children": null
        },
        {
        "id": 3,  
            "url": "http://www.twitter.com/bla",
            "domain": "www.twitter.com",  
            "crawlTime":"01:30", 
            "regExpMatch": null,
            "children": null
        }
    ],

    "links": [


    ]
}
*/