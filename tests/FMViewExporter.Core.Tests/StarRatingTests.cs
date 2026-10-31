using System.Linq;
using FMViewExporter.Core;
using Xunit;

namespace FMViewExporter.Core.Tests
{
    public class StarRatingTests
    {
        private static double Total(params string[] stars) =>
            stars.Sum(s => StarRating.StarValue(new[] { "fm-star-rating-star", "ability-" + s + "-youth-false" }));

        [Fact]
        public void Gold_stars_only()
        {
            // Kader: 3 gelbe Sterne
            Assert.Equal(3.0, Total("max-potential-level-fullfilled", "max-potential-level-fullfilled",
                "max-potential-level-fullfilled", "minimum-potential-level-none", "minimum-potential-level-none"));
        }

        [Fact]
        public void Grey_full_star_counts()
        {
            // Chris Afanou, Potential: 1 gelber + 1 grauer Stern = 2.0 (vorher 1.0)
            Assert.Equal(2.0, Total("max-potential-level-fullfilled", "minimum-potential-level-full",
                "minimum-potential-level-none", "minimum-potential-level-none", "minimum-potential-level-none"));
        }

        [Fact]
        public void Half_stars()
        {
            // Rodrygo, Ability aus dem Shortlist-Abbild: 1 + 1 + halb gelb + halb grau = 3.0
            Assert.Equal(3.0, Total("max-potential-level-fullfilled", "max-potential-level-fullfilled",
                "half-potential-level-half", "minimum-potential-level-half", "minimum-potential-level-none"));
        }

        [Fact]
        public void Youth_variant_and_unknown_scheme()
        {
            Assert.Equal(1.0, StarRating.StarValue(new[] { "ability-minimum-potential-level-full-youth-true" }));
            Assert.Equal(0.5, StarRating.StarValue(new[] { "something-half-star" }));
            Assert.Equal(0.0, StarRating.StarValue(new[] { "fm-star-rating-star" }));
            Assert.Equal("2.5", StarRating.Format(2.5));
        }
    }
}
