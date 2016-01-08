//-------------------------------------------------------------------------------------------------
// <copyright file="Guids.cs" company="TSF">
// Copyright (c) TSF.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
// Guids.cs
// MUST match guids.h
namespace TSF.AlmaStyleFix
{
    using System;

    /// <summary>
    /// Created by the project template.
    /// </summary>
    public static class GuidList
    {
        /// <summary>
        /// Created by the project template.
        /// </summary>
        public const string GuidAlmaStyleFixCmdSetString = "988a067f-9f0e-4b4c-a045-3120a5ec4676";

        /// <summary>
        /// Created by the project template.
        /// </summary>
        public const string GuidAlmaStyleFixPkgString = "b890da57-6f9c-4466-97bc-a15777ac21d6";

        /// <summary>
        /// Created by the project template.
        /// </summary>
        public static readonly Guid GuidAlmaStyleFixCmdSet = new Guid(GuidAlmaStyleFixCmdSetString);
    }
}