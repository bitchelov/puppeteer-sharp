using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class QueryObjectsTests : PuppeteerPageBaseTest
    {
        public QueryObjectsTests(ITestOutputHelper output) : base(output)
        {
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            // Instantiate an object
            await Page.EvaluateExpressionAsync("window.set = new Set(['hello', 'world'])");
            var prototypeHandle = await Page.EvaluateExpressionHandleAsync("Set.prototype");
            var objectsHandle = await Page.QueryObjectsAsync(prototypeHandle);
            var count = await Page.EvaluateFunctionAsync<int>("objects => objects.length", objectsHandle);
            Assert.Equal(1, count);
            var values = await Page.EvaluateFunctionAsync<string[]>("objects => Array.from(objects[0].values())", objectsHandle);
            Assert.Equal(new[] { "hello", "world" }, values);
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkForNonBlankPage()
        {
            // Instantiate an object
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync("() => window.set = new Set(['hello', 'world'])");
            var prototypeHandle = await Page.EvaluateFunctionHandleAsync("() => Set.prototype");
            var objectsHandle = await Page.QueryObjectsAsync(prototypeHandle);
            var count = await Page.EvaluateFunctionAsync<int>("objects => objects.length", objectsHandle);
            Assert.Equal(1, count);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldFailForDisposedHandles()
        {
            var prototypeHandle = await Page.EvaluateExpressionHandleAsync("HTMLBodyElement.prototype");
            await prototypeHandle.DisposeAsync();
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => Page.QueryObjectsAsync(prototypeHandle));
            Assert.Equal("Prototype JSHandle is disposed!", exception.Message);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldFailPrimitiveValuesAsPrototypes()
        {
            var prototypeHandle = await Page.EvaluateExpressionHandleAsync("42");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => Page.QueryObjectsAsync(prototypeHandle));
            Assert.Equal("Prototype JSHandle must not be referencing primitive value", exception.Message);
        }
    }
}
