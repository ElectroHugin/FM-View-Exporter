using System;
using FMViewExporter.Core;
using Xunit;

namespace FMViewExporter.Core.Tests
{
    public class HtmlWriterTests
    {
        [Fact]
        public void Encodes_cells_and_contains_headers_and_rows()
        {
            var table = new ExportTable("Squad <Test>", new[] { "Name", "Club" }, new DateTime(2026, 10, 29, 20, 15, 0));
            table.AddRow(new[] { "<script>alert(1)</script>", "A & B" });

            var html = HtmlWriter.ToHtml(table);

            Assert.Contains("<title>Squad &lt;Test&gt;</title>", html);
            Assert.Contains("<th>Name</th><th>Club</th>", html);
            Assert.Contains("<td>&lt;script&gt;alert(1)&lt;/script&gt;</td><td>A &amp; B</td>", html);
            Assert.Contains("1 rows, exported 2026-10-29 20:15", html);
        }

        [Fact]
        public void Page_loads_nothing_from_outside()
        {
            var table = new ExportTable("t", new[] { "A" }, DateTime.Now);
            var html = HtmlWriter.ToHtml(table);

            Assert.DoesNotContain("http://", html);
            Assert.DoesNotContain("https://", html);
            Assert.DoesNotContain("src=", html);
        }
    }
}
