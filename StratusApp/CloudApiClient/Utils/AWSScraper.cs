﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;

namespace CloudApiClient.Utils
{
    public class AWSScraper
    {
        private readonly HttpClient _httpClient;

        public AWSScraper()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<InstancePrice>> ScrapeInstancePrices()
        {
            var url = "https://aws.amazon.com/ec2/pricing/on-demand/";

            var html = await _httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var iframeElement = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='ec2-on-demand-plan']/iframe");
            if (iframeElement != null)
            {
                var iframeUrl = iframeElement.GetAttributeValue("data-src", "");

                if (!string.IsNullOrEmpty(iframeUrl))
                {
                    var iframeHtml = await _httpClient.GetStringAsync(iframeUrl);
                    var iframeDocument = new HtmlDocument();
                    iframeDocument.LoadHtml(iframeHtml);

                    var instanceTypeNodes = iframeDocument.DocumentNode.SelectNodes("//tr[@class='awsui-table-row']/td[1]/span/span");
                    var priceNodes = iframeDocument.DocumentNode.SelectNodes("//tr[@class='awsui-table-row']/td[2]/span/span");

                    var instancePrices = new List<InstancePrice>();

                    if (instanceTypeNodes != null && priceNodes != null && instanceTypeNodes.Count == priceNodes.Count)
                    {
                        for (int i = 0; i < instanceTypeNodes.Count; i++)
                        {
                            var instanceType = instanceTypeNodes[i].InnerText.Trim();
                            var price = priceNodes[i].InnerText.Trim();

                            instancePrices.Add(new InstancePrice { InstanceType = instanceType, Price = price });
                        }
                    }

                    return instancePrices;
                }
            }
            return null;
        }
    }
    public class InstancePrice
    {
        public string InstanceType { get; set; }
        public string Price { get; set; }
    }
}