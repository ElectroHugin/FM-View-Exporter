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
        public void Removes_characters_windows_does_not_allow()
        {
            Assert.Equal("player_search_ab", ExportFileNames.Sanitize("Player Search: A/B"));
            Assert.Equal("export", ExportFileNames.Sanitize("  ?*  ".Trim()));
        }
    }
}
