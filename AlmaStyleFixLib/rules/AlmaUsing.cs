//----------------------------------------------------------------------------------------------
// <copyright file="AlmaUsing.cs" company="Almaviva TSF" author="Andrea De Lucia">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------
namespace AlmaStyleFixLib.Drivers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Riorganizza gli using (al posto di Narrange).
    /// </summary>
    public class AlmaUsing : StyleCopRules
    {
        /// <summary>
        /// Esegue il fix della regola sulla posizione degli using.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void Sa1200(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "SA1200"))
                {
                    bool haveNamespace = false;
                    bool haveBracet = false;
                    for (int j = i; j < workingLines.Count; j++)
                    {
                        if (!haveNamespace)
                        {
                            if (workingLines[j].Line.Contains("namespace"))
                            {
                                haveNamespace = true;
                            }
                        }

                        if (haveNamespace && !haveBracet)
                        {
                            if (workingLines[j].Line.Contains("{"))
                            {
                                haveBracet = true;

                                var ind = workingLines[j].Line.IndexOf("{");
                                if (ind < workingLines[j].Line.Length - 1)
                                {
                                    var appo = workingLines[j].Line.Substring(ind + 1);
                                    workingLines.Insert(j, new SFWorkingLine(j, appo, false, new List<SAObject>()));
                                    workingLines[j].Line = workingLines[j].Line.Substring(0, ind);
                                }
                            }
                        }

                        if (haveNamespace && haveBracet)
                        {
                            j++;

                            // while (workingLines[j++].Line.Contains("using")) ;
                            var appo = workingLines[i].Line;
                            workingLines.RemoveAt(i);
                            --i;
                            workingLines.Insert(j, new SFWorkingLine(j, appo, false, new List<SAObject>()));
                            break;
                        }
                    }
                }
            }
        }

        // internal void arrange(ref List<SFWorkingLine> workingLines)
        // {
        //    // prima passata identifico tutti i punti che mi servono
        //    for (int i = 0; i < workingLines.Count; i++)
        //    {
        //        if (IsLineViolated(workingLines[i], "SA1200"))
        //        {
        //            return;
        //        }
        //    }
        // }
    }
}