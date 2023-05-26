using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Pricing;
using Amazon.Pricing.Model;

namespace CloudApiClient.Utils
{
    public class InstanceFilterHelper
    {
        private readonly List<Filter> filters = new List<Filter>();

        public void AddFilter(FilterType filterType, string field,  string value)
        {
            Filter newFilter = new Filter()
            {
                Type = filterType,
                Field = field,
                Value = value
            };

            filters.Add(newFilter);
        }

        public List<Filter> Filters
        {
            get { return filters; }
        }
    }
}
