using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Reflection;
using Il2CppObject = Il2CppSystem.Object;

namespace FMViewExporter.Diagnostics
{
    /// <summary>
    /// Liest Felder und Eigenschaften von Spielobjekten ueber die IL2CPP-Reflection,
    /// ohne die Klassen des Spiels beim Kompilieren zu kennen. Nur lesend.
    /// </summary>
    internal static class Il2CppInspector
    {
        private const BindingFlags AllInstance =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        public static string TypeName(Il2CppObject? obj)
        {
            if (obj == null)
                return "null";
            try
            {
                return obj.GetIl2CppType().FullName;
            }
            catch (Exception)
            {
                return "?";
            }
        }

        /// <summary>Alle Typnamen von der Klasse bis zur Basis, z. B. fuer "ist das eine Liste?".</summary>
        public static IEnumerable<string> TypeChain(Il2CppObject obj)
        {
            var type = obj.GetIl2CppType();
            while (type != null)
            {
                yield return type.FullName;
                type = type.BaseType;
            }
        }

        public static string Short(Il2CppObject? value, int maxLength = 120)
        {
            if (value == null)
                return "null";
            string text;
            try
            {
                text = value.ToString();
            }
            catch (Exception ex)
            {
                text = "<" + ex.GetType().Name + ">";
            }
            text = text.Replace("\r", "\\r").Replace("\n", "\\n");
            return text.Length > maxLength ? text.Substring(0, maxLength) + "..." : text;
        }

        /// <summary>Liest eine Eigenschaft oder ein Feld mit diesem Namen, auch aus Basisklassen.</summary>
        public static Il2CppObject? GetMember(Il2CppObject obj, string name)
        {
            var type = obj.GetIl2CppType();
            while (type != null)
            {
                var property = type.GetProperty(name, AllInstance);
                if (property != null && property.CanRead && property.GetIndexParameters().Length == 0)
                    return property.GetValue(obj, null);

                var field = type.GetField(name, AllInstance);
                if (field != null)
                    return field.GetValue(obj);

                type = type.BaseType;
            }
            return null;
        }

        public static int? GetInt(Il2CppObject obj, string name)
        {
            var value = GetMember(obj, name);
            return value != null && int.TryParse(value.ToString(), out var number) ? number : null;
        }

        /// <summary>Element einer IL2CPP-Liste ueber ihren Indexer.</summary>
        public static Il2CppObject? GetListItem(Il2CppObject list, int index)
        {
            var type = list.GetIl2CppType();
            var getter = type.GetMethod("get_Item") ?? type.GetMethod("GetValue", new[] { Il2CppSystem.Type.GetType("System.Int32") });
            if (getter == null)
                return null;

            var args = new Il2CppReferenceArray<Il2CppObject>(1);
            args[0] = BoxInt(index);
            return getter.Invoke(list, args);
        }

        private static unsafe Il2CppObject BoxInt(int value)
        {
            var klass = Il2CppClassPointerStore<int>.NativeClassPtr;
            return new Il2CppObject(IL2CPP.il2cpp_value_box(klass, (IntPtr)(&value)));
        }

        /// <summary>
        /// Beschreibt alle Felder und Eigenschaften eines Objekts mit Typ und Wert.
        /// Mit depth &gt; 0 werden Unterobjekte aus Spiel-Namensraeumen ebenfalls aufgeklappt.
        /// </summary>
        public static void Describe(Il2CppObject obj, string indent, int depth, Action<string> writeLine, ref int budget)
        {
            var type = obj.GetIl2CppType();
            while (type != null && type.FullName != "System.Object" && budget > 0)
            {
                writeLine($"{indent}[{type.FullName}]");

                foreach (var field in type.GetFields(AllInstance))
                {
                    if (--budget < 0)
                        return;
                    Il2CppObject? value = null;
                    string shown;
                    try
                    {
                        value = field.GetValue(obj);
                        shown = Short(value);
                    }
                    catch (Exception ex)
                    {
                        shown = "<" + ex.GetType().Name + ">";
                    }
                    writeLine($"{indent}  F {field.Name} : {field.FieldType.Name} = {shown}");
                    Expand(value, field.FieldType, indent, depth, writeLine, ref budget);
                }

                foreach (var property in type.GetProperties(AllInstance))
                {
                    if (--budget < 0)
                        return;
                    if (!property.CanRead || property.GetIndexParameters().Length > 0)
                        continue;
                    Il2CppObject? value = null;
                    string shown;
                    try
                    {
                        value = property.GetValue(obj, null);
                        shown = Short(value);
                    }
                    catch (Exception ex)
                    {
                        shown = "<" + ex.GetType().Name + ">";
                    }
                    writeLine($"{indent}  P {property.Name} : {property.PropertyType.Name} = {shown}");
                    Expand(value, property.PropertyType, indent, depth, writeLine, ref budget);
                }

                type = type.BaseType;
            }
        }

        private static void Expand(Il2CppObject? value, Il2CppSystem.Type declared, string indent, int depth, Action<string> writeLine, ref int budget)
        {
            if (value == null || depth <= 0 || declared.IsPrimitive || declared.IsEnum)
                return;
            var ns = declared.Namespace ?? string.Empty;
            if (ns.StartsWith("System", StringComparison.Ordinal) || ns.StartsWith("Unity", StringComparison.Ordinal))
                return;
            Describe(value, indent + "    ", depth - 1, writeLine, ref budget);
        }
    }
}
