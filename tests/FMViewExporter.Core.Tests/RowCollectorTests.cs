using System.Linq;
using FMViewExporter.Core;
using Xunit;

namespace FMViewExporter.Core.Tests
{
    public class RowCollectorTests
    {
        [Fact]
        public void Rows_seen_twice_are_kept_once()
        {
            var rows = new RowCollector(3, 0);
            Assert.True(rows.Add(new[] { "1", "A", "x" }, 0));
            Assert.True(rows.Add(new[] { "2", "B", "y" }, 1));
            Assert.False(rows.Add(new[] { "2", "B", "y" }, 5));
            Assert.Equal(2, rows.Count);
        }

        [Fact]
        public void Later_reads_fill_empty_cells()
        {
            var rows = new RowCollector(3, 0);
            rows.Add(new[] { "1", "A", "" }, 0);
            rows.Add(new[] { "1", "", "late" }, 1);
            Assert.Equal(new[] { "1", "A", "late" }, rows.RowsInOrder().Single());
        }

        [Fact]
        public void Order_follows_first_position()
        {
            var rows = new RowCollector(2, 0);
            rows.Add(new[] { "2", "B" }, 20);
            rows.Add(new[] { "1", "A" }, 10);
            rows.Add(new[] { "2", "B" }, 5);
            Assert.Equal(new[] { "1", "2" }, rows.RowsInOrder().Select(r => r[0]));
        }

        [Fact]
        public void Without_key_column_the_whole_row_is_the_key_and_empty_rows_are_ignored()
        {
            var rows = new RowCollector(2, -1);
            Assert.False(rows.Add(new[] { "", "" }, 0));
            Assert.True(rows.Add(new[] { "a", "b" }, 0));
            Assert.False(rows.Add(new[] { "a", "b" }, 1));
            Assert.True(rows.Add(new[] { "a", "c" }, 2));
            Assert.Equal(2, rows.Count);
        }

        [Fact]
        public void Reports_columns_without_any_value()
        {
            var rows = new RowCollector(3, 0);
            rows.Add(new[] { "1", "", "x" }, 0);
            rows.Add(new[] { "2", "", "" }, 1);
            Assert.Equal(new[] { 1 }, rows.EmptyColumns());
        }
    }
}
