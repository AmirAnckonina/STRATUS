using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using Microsoft.Playwright;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Utils.DTO;

namespace CloudApiClient.Utils
{
    public class AWSScraper
    {
        private readonly HttpClient _httpClient;
        private readonly IWebDriver _driver;

        public AWSScraper()
        {
            _httpClient = new HttpClient();
            _driver = new ChromeDriver();
        }

        public async Task<List<AlternativeInstance>> ScrapeInstanceDetails()
        {
            var url = "https://aws.amazon.com/ec2/pricing/on-demand/";

            // Navigate to the web page
            _driver.Navigate().GoToUrl(url);

            // Switch to the iframe
            _driver.SwitchTo().Frame(_driver.FindElement(By.CssSelector("#ec2-on-demand-plan iframe")));
            try
            {
                WebDriverWait waitToButtonsDisplay = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
                waitToButtonsDisplay.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".awsui_button-trigger_18eso_1550o_97.awsui_has-caret_18eso_1550o_137")));

                // Find the list box button
                var listBoxButtons = _driver.FindElements(By.CssSelector(".awsui_button-trigger_18eso_1550o_97.awsui_has-caret_18eso_1550o_137"));
                listBoxButtons[1].Click();

                // Wait for the options to be displayed
                var waitToDropDownDisplay = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                waitToDropDownDisplay.Until(ExpectedConditions.ElementExists(By.CssSelector(".awsui_options-list_19gcf_1apnh_93")));

                // Find all the options inside the list
                var options = _driver.FindElements(By.CssSelector(".awsui_options-list_19gcf_1apnh_93 li"));

                // Loop through each option and click on it
                for (int j = 0; j < options.Count; j++)
                {
                    try
                    {
                        // Retry mechanism with explicit wait for the dropdown options to be clickable
                        var retryAttempts = 3;
                        while (retryAttempts > 0)
                        {
                            try
                            {
                                // Reopen the dropdown to ensure fresh references to elements
                                listBoxButtons[1].Click();

                                // Wait for the options to be clickable
                                var optionsToClick = _driver.FindElements(By.CssSelector(".awsui_options-list_19gcf_1apnh_93 li"));
                                optionsToClick[j].Click();
                                List<AlternativeInstance> alternativeInstances = getAlternativeInstancesFromHtmlTable();
                                break; // Successfully clicked, break out of the retry loop
                            }
                            catch (Exception e)
                            {
                                // Retry on StaleElementReferenceException
                                retryAttempts--;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                // Close the dropdown after all options are clicked
                listBoxButtons[1].Click();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            _driver.Quit();
            return null;
        }
        private List<AlternativeInstance> getAlternativeInstancesFromHtmlTable()
        {
            // Wait for the instance details table to be visible
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.CssSelector(".awsui_table_wih1l_5nk4n_144")).Displayed);

            // Extract instance details from all table pages
            var instanceDetails = new List<AlternativeInstance>();
            while (true)
            {
                // Extract instance details from the current page
                IReadOnlyCollection<IWebElement> instanceRows = _driver.FindElements(By.CssSelector(".awsui_row_wih1l_5nk4n_316"));
                foreach (IWebElement row in instanceRows)
                {
                    string instanceName = row.FindElement(By.CssSelector("td:nth-child(1)")).Text;
                    string hourlyRate = row.FindElement(By.CssSelector("td:nth-child(2)")).Text;
                    string vCPU = row.FindElement(By.CssSelector("td:nth-child(3)")).Text;
                    string memory = row.FindElement(By.CssSelector("td:nth-child(4)")).Text;
                    string storage = row.FindElement(By.CssSelector("td:nth-child(5)")).Text;
                    string networkPerformance = row.FindElement(By.CssSelector("td:nth-child(6)")).Text;

                    AlternativeInstance instance = new AlternativeInstance(instanceName, hourlyRate, vCPU, memory, storage, networkPerformance);
                    instanceDetails.Add(instance);
                }

                // Check if there is a next page button
                IWebElement nextPageButton = _driver.FindElement(By.CssSelector(".awsui_arrow_fvjdu_f73zt_141[aria-label='Next page']"));
                bool isNextPageDisabled = !nextPageButton.Enabled;
                if (isNextPageDisabled)
                {
                    break; // No more pages, exit the loop
                }

                // Click the next page button
                nextPageButton.Click();

                // Wait for the next page to load
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                wait.Until(driver => driver.FindElements(By.CssSelector(".awsui_row_wih1l_5nk4n_316")).Count > 0);
            }

            // Close the browser
            //_driver.Quit();

            return instanceDetails;
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

