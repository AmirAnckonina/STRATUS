using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using Microsoft.Playwright;
using CloudApiClient.DTO;

namespace CloudApiClient.Utils
{
    public class AWSScraper
    {
        private readonly HttpClient _httpClient;

        public AWSScraper()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<AlternativeInstance>> ScrapeInstanceDetails()
        {
            var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync();
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            var url = "https://aws.amazon.com/ec2/pricing/on-demand/";

            await page.GotoAsync(url);

            // Wait for the iframe to be loaded
            //await page.WaitForSelectorAsync("#ec2-on-demand-plan iframe");

            // Get the src attribute of the iframe
            var iframeSrc = await page.EvaluateAsync<string>("document.querySelector('#ec2-on-demand-plan iframe').getAttribute('data-src')");

            if (!string.IsNullOrEmpty(iframeSrc))
            {
                await page.GotoAsync(iframeSrc);
                var instanceDetails = new List<AlternativeInstance>();

                //await page.GetByRole(AriaRole.Button , new PageGetByRoleOptions { Name = "vCPU" }).ClickAsync();
                await page.ClickAsync("[class='awsui_button-trigger_18eso_1550o_97 awsui_has-caret_18eso_1550o_137']");
                // Wait for the dropdown options to be available

                //await page.WaitForSelectorAsync("#formField29-1688911664346-8879");
                //await page.WaitForTimeoutAsync(1000); // Adjust the timeout duration as needed
                // Get the listbox element.

                await page.WaitForTimeoutAsync(1000); // Adjust the timeout duration as needed

                // Get the listbox element.
                var listbox = await page.QuerySelectorAsync("[class='awsui_button-trigger_18eso_1550o_97 awsui_has-caret_18eso_1550o_137']");

                // Select the option with the value 1.
                await page.SelectOptionAsync("select[class='awsui_button-trigger_18eso_1550o_97 awsui_has-caret_18eso_1550o_137']", "1");



                //await page.SelectOptionAsync("#formField29-1688911664346-8879", "1");
                //await page.ClickAsync("[class='awsui_button-trigger_18eso_1550o_97 awsui_has-caret_18eso_1550o_137']");

                // Wait for the instance details table to be rendered
                await page.WaitForSelectorAsync(".awsui_table_wih1l_5nk4n_144");


                //var vCPUButton = await page.QuerySelectorAsync(".awsui_button-trigger_18eso_1550o_97 awsui_has-caret_18eso_1550o_137");
                //await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "vCPU" }).ClickAsync();
                //var vCPUOption = await page.QuerySelectorAsync("span[id^='trigger-content-35-1688731781661-2950']");
                // Extract instance details from all table pages
                while (true)
                {
                    // Wait for the instance details table to be rendered
                    await page.WaitForSelectorAsync(".awsui_table_wih1l_5nk4n_144");

                    // Extract instance details from the current page
                    var instanceRows = await page.QuerySelectorAllAsync(".awsui_row_wih1l_5nk4n_316");

                    var tasks = instanceRows.Select(async row =>
                    {
                        var instanceName = await row.QuerySelectorAsync("td:nth-child(1)")?.Result.InnerTextAsync();
                        var hourlyRate = await row.QuerySelectorAsync("td:nth-child(2)")?.Result.InnerTextAsync();
                        var vCPU = await row.QuerySelectorAsync("td:nth-child(3)")?.Result.InnerTextAsync();
                        var memory = await row.QuerySelectorAsync("td:nth-child(4)")?.Result.InnerTextAsync();
                        var storage = await row.QuerySelectorAsync("td:nth-child(5)")?.Result.InnerTextAsync();
                        var networkPerformance = await row.QuerySelectorAsync("td:nth-child(6)")?.Result.InnerTextAsync();

                        return new AlternativeInstance(instanceName, hourlyRate, vCPU, memory, storage, networkPerformance);
                    });

                    instanceDetails.AddRange(await Task.WhenAll(tasks));

                    // Wait for all tasks to complete
                    await Task.WhenAll(tasks);
                    // Check if there is a next page button
                    var nextPageButton = await page.QuerySelectorAsync(".awsui_arrow_fvjdu_f73zt_141[aria-label='Next page']");
                    if (nextPageButton == null || await nextPageButton.EvaluateAsync<bool>("button => button.disabled"))
                    {
                        break; // No more pages, exit the loop
                    }

                    // Click the next page button
                    await nextPageButton.ClickAsync();

                    // // Wait for the instance details table to be rendered on the next page
                    // await page.WaitForSelectorAsync(".awsui_table_wih1l_5nk4n_144");
                }

                // Close the browser
                await browser.CloseAsync();

                return instanceDetails;
            }

            // Close the browser
            await browser.CloseAsync();

            return null;
        }

        public async Task<decimal> GetInstancePrice(string instanceId)
        {
            try
            {
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync();
                var context = await browser.NewContextAsync();
                var page = await context.NewPageAsync();

                // Visit the AWS EC2 pricing page
                await page.GotoAsync("https://aws.amazon.com/ec2/pricing/on-demand/");

                // Wait for the page to load
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Enter the instance ID in the search input
                await page.TypeAsync("#awsp-search-bar-input", instanceId);

                // Submit the search form
                await page.Keyboard.PressAsync("Enter");

                // Wait for the search results page to load
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Extract the price per hour from the search results
                var pricePerHour = await page.EvaluateAsync<decimal?>(@"() => {
            const resultElement = document.querySelector('.as-search-results .as-search-result-card .as-price .a-price-whole');
            if (resultElement) {
                const priceText = resultElement.innerText.trim().replace(',', '');
                return parseFloat(priceText);
            }
            return null;
        }");

                // Close the browser
                await browser.CloseAsync();

                if (pricePerHour.HasValue)
                {
                    return pricePerHour.Value;
                }
                else
                {
                    // Price not found
                    Console.WriteLine($"Price not found for instance ID: {instanceId}");
                    return decimal.Zero;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving instance price:");
                Console.WriteLine(ex);
                return decimal.Zero;
            }
        }
    }
}
