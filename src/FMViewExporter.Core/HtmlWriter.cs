using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace FMViewExporter.Core
{
    /// <summary>
    /// Schreibt eine <see cref="ExportTable"/> als eigenstaendige HTML-Seite.
    /// Alles ist eingebettet, die Seite laedt nichts aus dem Internet.
    /// Ein Klick auf einen Spaltenkopf sortiert die Tabelle.
    /// </summary>
    public static class HtmlWriter
    {
        private const string Style =
            "body{font-family:Segoe UI,Arial,sans-serif;margin:16px;background:#fff;color:#1b1b1b}" +
            "header{display:flex;align-items:center;gap:10px;margin:0 0 12px}" +
            "h1{font-size:18px;margin:0 0 2px}p{margin:0;color:#555;font-size:13px}" +
            "table{border-collapse:collapse;font-size:13px}" +
            "th,td{border:1px solid #d0d0d0;padding:4px 8px;text-align:left;white-space:nowrap}" +
            "th{background:#e8f0fe;border-bottom:2px solid " + Branding.AccentColor + ";cursor:pointer;position:sticky;top:0}" +
            "tr:nth-child(even) td{background:#fafafa}";

        // Sortiert per Klick auf den Kopf. Zahlen tragen ihren Sortierwert in data-s (beim Export
        // berechnet, siehe SortValue; Spannen als "unten|oben"). Reihenfolge: Zahlen, dann Text, leere
        // Zellen immer zuletzt (auch absteigend). Gleiche Werte behalten ihre Reihenfolge.
        private const string Script =
            "document.querySelectorAll('th').forEach(function(th,i){th.addEventListener('click',function(){" +
            "var tb=th.closest('table').tBodies[0],rows=Array.from(tb.rows),asc=th.dataset.asc!=='1';" +
            "th.dataset.asc=asc?'1':'0';" +
            "var key=function(r){var c=r.cells[i];if(!c)return{k:2};var s=c.getAttribute('data-s');" +
            "if(s!==null){var p=s.split('|');return{k:0,a:parseFloat(p[0]),b:parseFloat(p[p.length-1])};}" +
            "var t=c.textContent.trim();return t===''?{k:2}:{k:1,t:t};};" +
            "var keyed=rows.map(function(r,n){return{r:r,n:n,v:key(r)};});" +
            "keyed.sort(function(x,y){var a=x.v,b=y.v;if(a.k!==b.k)return a.k-b.k;var r=0;" +
            "if(a.k===0)r=(a.a-b.a)||(a.b-b.b);else if(a.k===1)r=a.t.localeCompare(b.t,undefined,{numeric:true,sensitivity:'base'});" +
            "return (asc?r:-r)||(x.n-y.n);});" +
            "keyed.forEach(function(e){tb.appendChild(e.r);});});});";

        public static void Write(ExportTable table, TextWriter writer)
        {
            var title = Encode(table.Title);
            writer.Write("<!DOCTYPE html>\n<html>\n<head>\n<meta charset=\"utf-8\">\n");
            writer.Write("<title>" + title + "</title>\n");
            writer.Write("<link rel=\"icon\" type=\"image/png\" href=\"data:image/png;base64," + Branding.FaviconPngBase64 + "\">\n");
            writer.Write("<style>" + Style + "</style>\n</head>\n<body>\n");
            writer.Write("<header>" + Branding.LogoSvg + "<div>\n<h1>" + title + "</h1>\n");
            writer.Write("<p>" + table.Rows.Count.ToString(CultureInfo.InvariantCulture) + " rows, exported "
                + table.CreatedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)
                + " with FM View Exporter</p>\n</div></header>\n");
            writer.Write("<table>\n<thead><tr>");
            foreach (var header in table.Headers)
                writer.Write("<th>" + Encode(header) + "</th>");
            writer.Write("</tr></thead>\n<tbody>\n");
            var formats = new NumberFormat[table.Headers.Count];
            for (var c = 0; c < formats.Length; c++)
                formats[c] = SortValue.DetectFormat(Column(table, c));
            foreach (var row in table.Rows)
            {
                writer.Write("<tr>");
                for (var c = 0; c < row.Length; c++)
                {
                    var sort = SortValue.Format(row[c], formats[c]);
                    writer.Write(sort == null ? "<td>" : "<td data-s=\"" + sort + "\">");
                    writer.Write(Encode(row[c]) + "</td>");
                }
                writer.Write("</tr>\n");
            }
            writer.Write("</tbody>\n</table>\n<script>" + Script + "</script>\n</body>\n</html>\n");
        }

        public static void WriteFile(ExportTable table, string path)
        {
            using var writer = new StreamWriter(path, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Write(table, writer);
        }

        public static string ToHtml(ExportTable table)
        {
            using var writer = new StringWriter();
            Write(table, writer);
            return writer.ToString();
        }

        private static IEnumerable<string> Column(ExportTable table, int column)
        {
            foreach (var row in table.Rows)
                yield return row[column];
        }

        private static string Encode(string value) => WebUtility.HtmlEncode(value);
    }
}
