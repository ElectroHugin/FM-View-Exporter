using FMViewExporter.Core;
using Xunit;

namespace FMViewExporter.Core.Tests
{
    public class ViewNamesTests
    {
        [Fact]
        public void Shortlist_TableInsidePlayerTableFrame()
        {
            // FM27: #PlayerTable (Rahmen) > #playertableshortlist, im Bereich #RecruitmentShortlistTool
            var name = ViewNames.Guess(new[] { "PlayerTable", "playertableshortlist" },
                new[] { "", "ElementBindingRoot", "RecruitmentShortlistTool", "ReportBody" });
            Assert.Equal("shortlist", name);
        }

        [Fact]
        public void Shortlist_ByPanelName()
        {
            Assert.Equal("shortlist", ViewNames.Guess(new[] { "PlayerTable" }, new[] { "RecruitmentShortlistTool" }));
        }

        [Fact]
        public void PlayerDatabase_ByPanelName()
        {
            Assert.Equal("player-database", ViewNames.Guess(new[] { "PlayerTable", "playertable" },
                new[] { "ElementBindingRoot", "PlayerSearchReport", "ReportBody" }));
        }

        [Fact]
        public void Squad_ByScreenName()
        {
            Assert.Equal("squad", ViewNames.Guess(new[] { "PlayerTable", "playertable" }, new[] { "x", "SquadScreen" }));
        }

        [Fact]
        public void Unknown_FallsBackToPlayerDatabase()
        {
            Assert.Equal("player-database", ViewNames.Guess(new[] { "playertable" }, new[] { "Somewhere", null }));
        }
    }
}
