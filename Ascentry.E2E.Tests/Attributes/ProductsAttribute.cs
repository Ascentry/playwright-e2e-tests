using Ascentry.E2E.Contracts.Enums;
using Xunit.v3;

namespace Ascentry.E2E.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ProductsAttribute : Attribute, ITraitAttribute
    {
        private readonly ProductEnum[] _products;

        public ProductsAttribute(params ProductEnum[] products)
        {
            _products = products;
        }

        public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits()
        {
            return _products
                .Select(p => new KeyValuePair<string, string>("Product", p.ToString()))
                .ToList();
        }
    }
}