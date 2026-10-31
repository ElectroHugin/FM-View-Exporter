using System;
using FMViewExporter.Core;
using Xunit;

namespace FMViewExporter.Core.Tests
{
    public class ExportFileNamesTests
    {
        [Fact]
        public void Builds_name_with_timestamp()
        {
            Assert.Equal("squad_20261029_201500", ExportFileNames.Build("Squad", new DateTime(2026, 10, 29, 20, 15, 0)));
        }

        [Fact]
        public void Builds_fixed_name_with_prefix_and_without_timestamp()
        {
            Assert.Equal("fc-bayern-squad", ExportFileNames.Build("FC Bayern", "Squad", null));
            Assert.Equal("fc-bayern-player-database", ExportFileNames.Build("fc-bayern", "Player Database", null));
        }

        [Fact]
        public void Combines_prefix_and_timestamp()
        {
            Assert.Equal("fc-bayern-shortlist_20261029_201500",
                ExportFileNames.Build("fc-bayern", "shortlist", new DateTime(2026, 10, 29, 20, 15, 0)));
        }

        [Fact]
        public void Ignores_empty_prefix()
        {
            Assert.Equal("squad", ExportFileNames.Build("  ", "Squad", null));
        }

        [Fact]
        public void Removes_characters_windows_does_not_allow()
        {
            Assert.Equal("player-search-ab", ExportFileNames.Sanitize("Player Search: A/B"));
            Assert.Equal("export", ExportFileNames.Sanitize("  ?*  ".Trim()));
        }
    }
}
