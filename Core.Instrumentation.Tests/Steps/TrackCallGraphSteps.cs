namespace Core.Instrumentation.Tests.Steps
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
        
        [When(@"I call api layer to get product by id (\d+)$")]
        public void WhenICallApiLayerToGetProductById(int id)
        {
            var products = ScenarioContext.Current.Get<List<Product>>();
            var api = new ProductController(products);
            var product = api.Get(id);
            ScenarioContext.Current.Set(product, "Product");
        }
        
        [When(@"I call api layer to get product by id (\d+) and (\d+) asynchronously$")]
        public async void GivenICallApiLayerToGetProductByIdAsynchronously(int id1, int id2)
        {
            var products = ScenarioContext.Current.Get<List<Product>>();
            var api = new ProductController(products);
            var tasks = new Task<Product>[]
            {
                api.GetAsync(id1),
                api.GetAsync(id2)
            };
            Task.WaitAll(tasks);
            ScenarioContext.Current.Set(tasks[0].Result, "Product1");
            ScenarioContext.Current.Set(tasks[1].Result, "Product2");
        }


        [Then(@"I should get the following product with name ""(.*)""")]
        public void ThenIShouldGetTheFollowingProductWithName(string expectedProductName)
        {
            var product = ScenarioContext.Current.Get<Product>("Product");
            Assert.AreEqual(expectedProductName, product.Name);
        }

        [Then(@"I should get the following product with name ""(.*)"" and ""(.*)"" respectively")]
        public void ThenIShouldGetTheFollowingProductWithNameAndRespectively(string prodName1, string prodName2)
        {
            var product1 = ScenarioContext.Current.Get<Product>("Product1");
            Assert.AreEqual(prodName1, product1.Name);
            var product2 = ScenarioContext.Current.Get<Product>("Product2");
            Assert.AreEqual(prodName2, product2.Name);
        }

    }

    namespace StorageLayer
    {
        using System.Linq;
        using System.Threading.Tasks;
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

            public Task<Product> GetByIdAsync(int id)
            {
                return Task.Run(()=> products.FirstOrDefault(p => p.Id == id));
            }
        }
}

    namespace APILayer
    {
        using System.Threading.Tasks;
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

            public Task<Product> GetAsync(int id)
            {
                return Task.Factory.StartNew(()=> repository.GetById(id));
            }
        }
    }
}
