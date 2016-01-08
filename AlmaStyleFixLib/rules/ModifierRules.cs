namespace AlmaStyleFixLib.Drivers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Classe che esegue il Fix delle regole dei modificatori.
    /// </summary>
    public class ModifierRules : StyleCopRules
    {
        internal void SA1400_TheMethodMustHaveAnAccessModifier(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1400"))
                {
                    workingLine.Line = "private " + workingLine.Line;
                }
            }
        }
    }
}
