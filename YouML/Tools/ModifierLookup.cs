using System.Collections;
using System.Linq;

namespace YouML.Tools
{
    public static class ModifierLookup
    {

        public static string GetUmlModifiers(this IEnumerable modifier)
        {
            var retValue = string.Empty;

            foreach (var m in modifier)
            {
                switch (m.ToString())
                {
                    case "public": retValue = string.Concat(retValue, "+") ; break;
                    case "private": retValue = string.Concat(retValue, "-"); break; 
                    case "internal": retValue = string.Concat(retValue, "~"); break; 
                    case "protected": retValue = string.Concat(retValue, "#"); break; 
                }
            }
            return retValue;
        }

    }
}
