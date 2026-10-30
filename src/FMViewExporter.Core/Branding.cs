namespace FMViewExporter.Core
{
    /// <summary>
    /// Logo und Farbe aus docs/branding (gemeinsames Design mit dem FM Player Analyzer).
    /// Alles liegt als Text im Code, damit die HTML-Seite nichts nachladen muss.
    /// </summary>
    public static class Branding
    {
        public const string AccentColor = "#1B6AF5";

        /// <summary>Logo aus docs/branding/logo.svg, ohne Metadaten und ohne xmlns (in HTML5 nicht noetig).</summary>
        public const string LogoSvg =
            "<svg viewBox=\"0 0 1024 1024\" width=\"36\" height=\"36\" aria-hidden=\"true\">" +
            "<circle cx=\"512\" cy=\"512\" r=\"512\" fill=\"#1B6AF5\"/>" +
            "<rect x=\"184\" y=\"184\" width=\"650\" height=\"620\" rx=\"100\" fill=\"white\"/>" +
            "<g fill=\"#1B6AF5\"><rect x=\"254\" y=\"276\" width=\"158\" height=\"108\" rx=\"15\"/>" +
            "<rect x=\"448\" y=\"276\" width=\"318\" height=\"108\" rx=\"15\"/>" +
            "<rect x=\"254\" y=\"422\" width=\"158\" height=\"108\" rx=\"15\"/>" +
            "<rect x=\"448\" y=\"422\" width=\"318\" height=\"108\" rx=\"15\"/>" +
            "<rect x=\"254\" y=\"568\" width=\"158\" height=\"108\" rx=\"15\"/>" +
            "<rect x=\"448\" y=\"568\" width=\"318\" height=\"108\" rx=\"15\"/>" +
            "<circle cx=\"736\" cy=\"700\" r=\"196\"/></g>" +
            "<circle cx=\"736\" cy=\"700\" r=\"168\" fill=\"white\"/>" +
            "<path d=\"M620 666H754V614Q754 603 763 611L855 691Q864 700 855 709L763 789Q754 797 754 786V734H620Z\" fill=\"#1B6AF5\"/>" +
            "</svg>";

        /// <summary>docs/branding/logo-512.png auf 32x32 verkleinert, als Base64 (Favicon).</summary>
        public const string FaviconPngBase64 =
            "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAHxElEQVR42qWXeYzdVRXHP+fe+/u9ZWY602UW2lICdABpqyCBWhJN2CPEaCBDUGMCCtiypIYQCZKAFTBoNKasZQ+JCcYJFY1CalXqBhrCJmIpFYTSaTvdOzNv3vst9x7/+L3pTGepEG/yy++9++4799zv+d5zzleYcajQh6FfPEDnddoTufQi8XoBhNOBBYTQBoAxw8AAmNfUysYsjzfseVB2AdCnln4CiE63i0y79x1qWCMBoHvl8NIoKt+gGr4kNu4GIHg05KC+acUixoGxhet5OiiGZzPPfYMPlt6abPPoDvSppV/8wr5tFe3pXgOy2kRRrGmK+iwgKIpBZOL/FVUQAoqIjYzEMSHLUtC1smvwju39i+pjtmd2oLlg/tWHTpJK6Wkblz5NMoqq5gIWEeGjDFVV8Ig4KVXxWfaKjta/suOx9ncmOyGTNz9m1cEzjKs+51zUldZrWT3DqYqA8vGGYES1WiK3pZbI5+nukNcv3vlQxysTnZCJ8VnwzUO9ROUXrXXzhkdG89mt1i1dKJQiQWZmzDQIFK+hhvKPDwJpHvJqteq8z/eSNc4eeLh969ieMsb2hQuJQ1L/e1SqLBsaGsk/0+vc/VeVOKFL+H/Gy+8Frn00YWCfzyutrS5P6m+aUmX59u2k9BPM2FXTRu1OV6ksSxu1rKPFukevmXnz8DGiceYJhp98LcarcSGtZbZSWaaN2p30i6cPI6Ayf2Vykjj+aUXlUM2bc5ZY+cVNZZIMNr7pGWkozkKaw4I5wmdPsWx4w3NwVHFmKjsEyAPMaxPOXWJJc1hxe52dB4LGsQ2KqOYs3bGu9I4DUaR2s4mrTpNaDkhsBVXYO6J845GEkYYSOxiuw1mLLc/fYrjuiYRdB5U4Ap3kgQg0Ujix2/DGDypYC5EFRYTg1ZRbnPcjN0P5Gte9crgL0ctCmmLBjnFIpDBcjcEYITLFXMkVIahEwqxq0/A0DsQOStE4IjoOjw1pCmIu7V45fJuzIpeYqGW2piMBa44AtBRBV7twaLQwaI0yb5YQO+hsF/LAURHobheMgA+TfvVpMHHrHNLaJU6QCynQ0YkGADrbhL98t0KSKUaEoEolFqyB399WptGcn56oSrm5NvOTbnCxlwpyoQNOI3hBMeOJrHgfqCnXP5kyNKpEDkYTWLbIcNflEaseTxk8VHBjOgSSDI7rFO6/qsSUNKYYghfgNIfoAg0Z06XZWgJ/3OxppEWshxvFXD2FP78d2DesuAkOBC0+WwNpBgMHBGeKEOiRHoqGDIX5BqUFDTPe4yRTRtPxJ/XaPGHxvT7hiS10VCnirspIQ/n3oFJy05BVA4K2uqPV6Dmtwvf6YoYbEBloZHB8l9BaFm6/LGbfiBLZ4oSRhU2bPS+85Vm+2NLbIwSF1U8ltJaE/SPTh8sh1BDTdgQKTQ9KDi78pKWRgjHgPXS0FLCet9RQS4ql1sBb2wNJBj9fXeaiT9nDh0hz+PWrOVt3BfaPKNW4mUnFgPqaQ2VAbHSK5g0d4/+Yl4NDyufWNBiqFzAO1WH5Ystzt5S44O6EnQcD5bjgRE+H8IfbKhzfJfgwzofYwaVnOVb0Wi7+YYOB/UrJqWIi0RAGDPA6xirCFCKEAJUYOqpCW0XoqApxVEA+Nt9eEcoRPPT1Esc3a4c1RUjWv5zz5KYcgGNmC3ddHpPkRT7EWAV93Sn6W4ErUJXJ9TZqptB6Os7yMUJFrmB2PYPF3YZzl1i27VP2DRcktQKbBwJ3r89o5Mqq8yPOW2Lp7TFs2+OlEiG52A3Oq/6GrHZAbDwbzYromAK+zlnCxu9UaGSKFfAKbZWChL+6uUySKVt2Kk9syogd3PPLjIc2psxtM/hQwH/sPOFbT6V8uFf5/hUxi+aKvrs7NpVsdL8P4Tk3uK5t9/xVtWdMHF+tSeZFxI0mxSlCgGPnypRORBmftyYcTrXd7cJJ8w2zq8UNaKQweEiZ2yp8YkGR53Kv3pRiJ5KvH1zXttuBitrhH4c0uVKwUo29vvqfIFt2BE6ebwg6tREaK1QK9HQY6s2Nbv1ixLe/EBEUnIEHNmbc82zGMzeVWNFrOVBDt+42EmuSi7M/AhVDH2bn/bPexudrpVy2RtQnGVy5LuGlrZ7cN7OYjD9j1TJoQcYzTzQ8vikndgU3KnFRyJYea3j+1jIreot2/Wd/TfyOWsmWbb72/bXlLYcbkoktmYkqyySr5fVMnDVF4ond0duyNFe27VV+ekOJc06106750+Y8/+rDzvm0/uZx5cryvzVbsilNqUTlFzF2nuSNXEVcI5uavSYPI8XVE4HrL4i44mzLonkGVfhgr9L/Upo/8IJz3vu9La5x9nv3HtGUTm7La2dYGz0vLuoMaS0TwQlH1wM6oVccGlXmtAoL5xQ57cN9IT+YtUSz4nRP2eSff/felmna8snCZOWhk8VVnjZxdHpojIJqjmD5H44IRcrOvWqS4wVx5bYqzmevkdS//P669i0zC5PppFlX5xrErDZxPEmaiSDIkdIMRVTHpJktxYQ0TQNhreza8xGl2TTitOfGxqkWuRHVS8WWusbFacbhAjZZnPpkNyLrPXrfrvvK//p44nQGeb7o2pFj8ji6SDScD3oaykK0Kc/FDCNsB/O6ivzOpdmGbY+07vwo8vy/8+b/rQjr+LYAAAAASUVORK5CYII=";
    }
}
