namespace Core.Instrumentation.Tests.Steps
{
    using System.Collections.Generic;
    using Core.Instrumentation.Tests.Models;
    using Core.Instrumentation.Tests.Steps.APILayer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TechTalk.SpecFlow;

    [Binding]
    public class TrackCallGraphSteps
    {
        [Given(@"product store is setup with following records")]
        public void GivenProductStoreIsSetupWithFollowingRecords(Table table)
        {
            var products = table.ToModelList<Product>();
            ScenarioContext.Current.Set(products);
        }
        
        [When(@"I call api layer to get product by id (.*)")]
        public void WhenICallApiLayerToGetProductById(int id)
        {
            var products = ScenarioContext.Current.Get<List<Product>>();
            var api = new ProductController(products);
            var product = api.Get(id);
            ScenarioContext.Current.Set(product, "Product");
        }
        
        [Then(@"I should get the following product with name ""(.*)""")]
        public void ThenIShouldGetTheFollowingProductWithName(string expectedProductName)
        {
            var product = ScenarioContext.Current.Get<Product>("Product");
            Assert.AreEqual(expectedProductName, product.Name);
        }
    }

    namespace StorageLayer
    {
        using System.Linq;
        using Core.Instrumentation.Tracking;

        [TraceCallGraphAspect(Categories.Default, Layers.DataAccess, CallFlowType.Layer, true)]
        public class ProductRepository
        {
            private List<Product> products;

            public ProductRepository(List<Product> products)
            {
                this.products = products;
            }

            public Product GetById(int id)
            {
                return products.FirstOrDefault(p => p.Id == id);
            }
        }
}

    namespace APILayer
    {
        using Core.Instrumentation.Tests.Steps.StorageLayer;
        using Core.Instrumentation.Tracking;

        [TraceCallGraphAspect(Categories.Default, Layers.DataAccess, CallFlowType.Layer, true)]
        public class ProductController
        {
            private ProductRepository repository;

            public ProductController(List<Product> products)
            {
                repository=new ProductRepository(products);
            }

            public Product Get(int id)
            {
                return repository.GetById(id);
            }
        }
    }
}
