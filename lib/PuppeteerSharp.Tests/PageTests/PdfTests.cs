using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PuppeteerSharp.Media;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PdfTests : PuppeteerPageBaseTest
    {
        public PdfTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Usage()
        {
            var outputFile = Path.Combine(BaseDirectory, "Usage.pdf");
            var fileInfo = new FileInfo(outputFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            #region PdfAsync

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions {Headless = true});
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync("http://www.google.com");
            await page.PdfAsync(outputFile);

            #endregion

            Assert.True(File.Exists(outputFile));
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldBeAbleToSaveFile()
        {
            var outputFile = Path.Combine(BaseDirectory, "output.pdf");
            var fileInfo = new FileInfo(outputFile);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            await Page.PdfAsync(outputFile);
            fileInfo = new FileInfo(outputFile);
            Assert.True(new FileInfo(outputFile).Length > 0);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public void PdfOptionsShouldBeSerializable()
        {
            var pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                DisplayHeaderFooter = true,
                MarginOptions = new MarginOptions
                {
                    Top = "20px",
                    Right = "20px",
                    Bottom = "40px",
                    Left = "20px"
                },
                FooterTemplate = "<div id=\"footer-template\" style=\"font-size:10px !important; color:#808080; padding-left:10px\">- <span class=\"pageNumber\"></span> - </div>"
            };

            var serialized = JsonConvert.SerializeObject(pdfOptions);
            var newPdfOptions = JsonConvert.DeserializeObject<PdfOptions>(serialized);
            Assert.Equal(pdfOptions, newPdfOptions);
        }
    }
}
