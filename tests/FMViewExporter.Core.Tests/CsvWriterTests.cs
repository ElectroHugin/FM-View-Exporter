using System;
using System.IO;
using FMViewExporter.Core;
using Xunit;

namespace FMViewExporter.Core.Tests
{
    public class CsvWriterTests
    {
        private static ExportTable Sample()
        {
            var table = new ExportTable("Squad", new[] { "UID", "Name", "Value" }, new DateTime(2026, 10, 29, 20, 15, 0));
            table.AddRow(new[] { "123", "Müller, Thomas", "€1.5M" });
            table.AddRow(new[] { "456", "He said \"hi\"", "1,234" });
            return table;
        }

        [Fact]
        public void English_uses_comma_and_quotes_cells_containing_commas()
        {
            var csv = CsvWriter.ToCsv(Sample(), CsvStyle.English);

            Assert.Equal(
                "UID,Name,Value\r\n" +
                "123,\"Müller, Thomas\",€1.5M\r\n" +
                "456,\"He said \"\"hi\"\"\",\"1,234\"\r\n",
                csv);
        }

        [Fact]
        public void German_uses_semicolon_and_leaves_commas_unquoted()
        {
            var csv = CsvWriter.ToCsv(Sample(), CsvStyle.German);

            Assert.Equal(
                "UID;Name;Value\r\n" +
                "123;Müller, Thomas;€1.5M\r\n" +
                "456;\"He said \"\"hi\"\"\";1,234\r\n",
                csv);
        }

        [Fact]
        public void Line_breaks_and_outer_spaces_are_quoted()
        {
            Assert.Equal("\"a\nb\"", CsvWriter.Escape("a\nb", ','));
            Assert.Equal("\" a\"", CsvWriter.Escape(" a", ','));
            Assert.Equal("a b", CsvWriter.Escape("a b", ','));
        }

        [Fact]
        public void File_starts_with_utf8_bom()
        {
            var path = Path.GetTempFileName();
            try
            {
                CsvWriter.WriteFile(Sample(), path, CsvStyle.English);
                var bytes = File.ReadAllBytes(path);
                Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, bytes[..3]);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Short_rows_are_padded_and_long_rows_cut()
        {
            var table = new ExportTable("t", new[] { "A", "B" }, DateTime.Now);
            table.AddRow(new[] { "1" });
            table.AddRow(new[] { "1", "2", "3" });

            Assert.Equal(new[] { "1", "" }, table.Rows[0]);
            Assert.Equal(new[] { "1", "2" }, table.Rows[1]);
        }

        [Fact]
        public void Styles_define_separators()
        {
            Assert.Equal(',', CsvStyle.English.Delimiter());
            Assert.Equal('.', CsvStyle.English.DecimalSeparator());
            Assert.Equal(';', CsvStyle.German.Delimiter());
            Assert.Equal(',', CsvStyle.German.DecimalSeparator());
        }
    }
}
