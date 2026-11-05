using System;
using System.Linq;
using FMViewExporter.Core;
using Xunit;

namespace FMViewExporter.Core.Tests
{
    public class SortValueTests
    {
        [Theory]
        [InlineData("€900K", "900000")]
        [InlineData("€1M", "1000000")]
        [InlineData("€1.2M", "1200000")]
        [InlineData("£2bn", "2000000000")]
        [InlineData("1,200", "1200")]
        [InlineData("950", "950")]
        [InlineData("7.19", "7.19")]
        [InlineData("175 cm", "175")]
        [InlineData("€275K p/w", "275000")]
        [InlineData("-3", "-3")]
        [InlineData("10-12", "10|12")]
        [InlineData("€9.4M - €12.5M", "9400000|12500000")]
        [InlineData("€11.5K - €15.75K p/w", "11500|15750")]
        public void English_values(string text, string expected)
        {
            Assert.Equal(expected, SortValue.Format(text, NumberFormat.English));
        }

        [Theory]
        [InlineData("1,5", "1.5")]
        [InlineData("1.200", "1200")]
        [InlineData("1.200,75", "1200.75")]
        [InlineData("€1,2M", "1200000")]
        public void German_values(string text, string expected)
        {
            Assert.Equal(expected, SortValue.Format(text, NumberFormat.German));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("Unknown")]
        [InlineData("Max Müller")]
        [InlineData("1. FC Heidenheim 1846")]
        [InlineData("Borussia Dortmund II")]
        [InlineData("29.10.2026")]
        [InlineData("1,2,3")]
        public void Text_and_empty_cells_have_no_number(string? text)
        {
            Assert.Null(SortValue.Format(text, NumberFormat.English));
        }

        [Fact]
        public void Detects_column_format()
        {
            Assert.Equal(NumberFormat.English, SortValue.DetectFormat(new[] { "1,200", "950" }));
            Assert.Equal(NumberFormat.English, SortValue.DetectFormat(new[] { "7.19", "6.50" }));
            Assert.Equal(NumberFormat.German, SortValue.DetectFormat(new[] { "1,5", "1.200" }));
            Assert.Equal(NumberFormat.English, SortValue.DetectFormat(new[] { "1,5", "7.25" }));
            Assert.Equal(NumberFormat.English, SortValue.DetectFormat(new[] { "", "Unknown" }));
        }

        [Fact]
        public void Magnitudes_sort_correctly()
        {
            double Low(string s)
            {
                Assert.True(SortValue.TryParse(s, NumberFormat.English, out var low, out _));
                return low;
            }

            var sorted = new[] { "€1M", "€900K", "€1.5M", "€50K" }.OrderBy(Low).ToArray();
            Assert.Equal(new[] { "€50K", "€900K", "€1M", "€1.5M" }, sorted);
            Assert.True(Low("950") < Low("1,200"));
        }
    }

    public class HtmlSortTests
    {
        [Fact]
        public void Numeric_cells_carry_sort_value_and_keep_their_text()
        {
            var table = new ExportTable("t", new[] { "Name", "Value", "Height" }, new DateTime(2026, 11, 5));
            table.AddRow(new[] { "A", "€900K", "1,5" });
            table.AddRow(new[] { "B", "€1M", "1.200" });
            table.AddRow(new[] { "C", "", "" });

            var html = HtmlWriter.ToHtml(table);

            Assert.Contains("<td>A</td><td data-s=\"900000\">€900K</td><td data-s=\"1.5\">1,5</td>", html);
            Assert.Contains("<td>B</td><td data-s=\"1000000\">€1M</td><td data-s=\"1200\">1.200</td>", html);
            Assert.Contains("<td>C</td><td></td><td></td>", html);
        }
    }
}
