//-------------------------------------------------------------------------------------------------
// <copyright file="SAObject.cs" company="OfficeClip LLC">
// Copyright (c) OfficeClip LLC.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace AlmaStyleFixLib
{
    /// <summary>
    /// The Container for the StyleCop Analysis Object
    /// </summary>
    public class SAObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SAObject"/> class.
        /// </summary>
        /// <param name="errorId">The error id.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="description">The description.</param>
        public SAObject(string errorId, int lineNumber, string description, bool isFixed)
        {
            this.ErrorId = errorId;
            this.LineNumber = lineNumber;
            this.Description = description;
            this.IsFixed = isFixed;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error id.
        /// </summary>
        /// <value>The error id.</value>
        public string ErrorId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether StyleFix is able to fix the violation, this is good for showing the result.
        /// </summary>
        public bool IsFixed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>The line number.</value>
        public int LineNumber
        {
            get;
            set;
        }
    }
}