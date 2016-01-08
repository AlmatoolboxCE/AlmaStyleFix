//----------------------------------------------------------------------------------------------
// <copyright file="FindUnder.cs" company="Almaviva TSF">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace AlmaStyleFixLib
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Management;
    using System.Net;
    using System.Net.Mail;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// 
    /// </summary>
    public class FindUnder
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] directories;

        /// <summary>
        /// 
        /// </summary>
        public bool findDirectories;

        /// <summary>
        /// 
        /// </summary>
        public bool findFiles;

        /// <summary>
        /// 
        /// </summary>
        public string path;

        /// <summary>
        /// 
        /// </summary>
        public string searchExpression;

        /// <summary>
        /// 
        /// </summary>
        public bool showEmptyOnly;

        /// <summary>
        /// Inizializza una nuova istanza della classe FindUnder.
        /// </summary>
        public FindUnder()
        {
            this.findDirectories = false;
            this.findFiles = false;
            this.searchExpression = "*";
            this.showEmptyOnly = false;
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se usare la.
        /// </summary>
        public bool FindDirectories
        {
            get
            {
                return this.findDirectories;
            }

            set
            {
                this.findDirectories = value;
            }
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se.
        /// </summary>
        public bool FindFiles
        {
            get
            {
                return this.findFiles;
            }

            set
            {
                this.findFiles = value;
            }
        }

        /// <summary>
        /// Recupera o imposta.
        /// </summary>
        public string[] FoundItems
        {
            get
            {
                return this.directories;
            }

            set
            {
                this.directories = value;
            }
        }

        /// <summary>
        /// Recupera o imposta This is the directory to search under for sub directories.
        /// This directory will not be included in the <c>Directories</c> item.
        /// </summary>
        public string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                this.path = value;
            }
        }

        /// <summary>
        /// Recupera o imposta.
        /// </summary>
        public string SearchExpression
        {
            get
            {
                return this.searchExpression;
            }

            set
            {
                this.searchExpression = value;
            }
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se.
        /// </summary>
        public bool ShowEmptyOnly
        {
            get
            {
                return this.showEmptyOnly;
            }

            set
            {
                this.showEmptyOnly = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath">
        /// 
        /// </param>
        public void Execute(string fullPath)
        {
            bool isInclude = false;

            // string fullPath = this.Path.GetMetadata("Fullpath");
            if (string.IsNullOrEmpty(fullPath) || !Directory.Exists(fullPath))
            {
                string message = string.Format("Path specified {0} doesn't exist", fullPath);
                throw new Exception(message);
            }

            if (!findFiles && !findDirectories)
            {
                string message = "Either FindFiles or FindDirectories must be true";
                throw new Exception(message);
            }

            DirectoryInfo dir = new DirectoryInfo(fullPath);
            FileInfo[] files = new FileInfo[0];
            DirectoryInfo[] subDirs = new DirectoryInfo[0];

            if (this.findFiles)
            {
                files = dir.GetFiles(this.searchExpression, SearchOption.AllDirectories);
            }

            if (this.findDirectories)
            {
                subDirs = dir.GetDirectories(this.searchExpression, SearchOption.AllDirectories);
            }

            List<string> items = new List<string>();
            foreach (FileInfo fInfo in files)
            {
                isInclude = true;
                if (this.showEmptyOnly)
                {
                    if (fInfo.Length > 0)
                    {
                        isInclude = false;
                    }
                }

                if (isInclude)
                {
                    items.Add(fInfo.FullName);
                }
            }

            foreach (DirectoryInfo dInfo in subDirs)
            {
                isInclude = true;
                if (this.showEmptyOnly)
                {
                    if (dInfo.Name == "XXX")
                    {
                        ;
                    }

                    if (!IsFolderEmpty(dInfo.FullName))
                    {
                        isInclude = false;
                    }
                }

                if (isInclude)
                {
                    items.Add(dInfo.FullName);
                }
            }

            this.directories = items.ToArray();

            foreach (string str in items)
            {
                Console.WriteLine(str);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirName">
        /// 
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        protected bool IsFolderEmpty(string dirName)
        {
            // first check if there is files
            if (Directory.GetDirectories(dirName).Length > 0)
            {
                return false;
            }

            // now check if there is any subfolder. Note that we should exclude the current folder
            if (Directory.GetFiles(dirName).Length > 0)
            {
                return false;
            }

            return true;
        }
    }
}