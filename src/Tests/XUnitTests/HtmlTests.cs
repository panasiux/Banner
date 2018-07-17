using Common;
using Xunit;

namespace XUnitTests
{
    public class HtmlTests
    {
        [Fact]
        public void TestValidHtml()
        {
            string explanation;
            Assert.True(HtmlFormatter.IsValidHtml("<!DOCTYPE html> <html> <body> <h1>This is heading 1</h1> <h2>This is heading 2</h2> <h3>This is heading 3</h3> <h4>This is heading 4</h4> <h5>This is heading 5</h5> <h6>This is heading 6</h6> </body> </html>",
                    out explanation));
        }

        [Fact]
        public void TestInvalidValidHtml()
        {
            string explanation;
            Assert.False(HtmlFormatter.IsValidHtml("<!DOCTYPE html> > <body> <h1>This is heading 1</h1> <h2>This is heading 2</h2> <h3>This is heading 3</h3> <h4>This is heading 4</h4> <h5>This is heading 5</h5> <h6>This is heading 6</h6> </body> </html>",
                out explanation));
        }
    }
}
