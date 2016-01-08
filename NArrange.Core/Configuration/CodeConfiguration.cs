#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2007-2008 James Nies and NArrange contributors.
 *    All rights reserved.
 *
 * This program and the accompanying materials are made available under
 * the terms of the Common Public License v1.0 which accompanies this
 * distribution.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in
 * the documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *<author>James Nies</author>
 *<contributor>Justin Dearing</contributor>
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace NArrange.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Code arranger configuration information.
    /// </summary>
    public class CodeConfiguration : ConfigurationElement
    {
        #region Fields

        /// <summary>
        /// Synchronization lock for the default configuration instance.
        /// </summary>
        private static readonly object _defaultLock = new object();

        /// <summary>
        /// Seriarializer for serializing and deserializing the configuration 
        /// to and from XML.
        /// </summary>
        private static readonly XmlSerializer _serializer;

        /// <summary>
        /// The default configuration instance.
        /// </summary>
        private static CodeConfiguration _default;

        /// <summary>
        /// Encoding configuration.
        /// </summary>
        private EncodingConfiguration _encoding;

        /// <summary>
        /// Formatting configuration.
        /// </summary>
        private FormattingConfiguration _formatting;

        /// <summary>
        /// Collection of source/project handler configurations.
        /// </summary>
        private HandlerConfigurationCollection _handlers;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Type initialization.
        /// </summary>
        static CodeConfiguration()
        {
            _serializer = new XmlSerializer(typeof(CodeConfiguration));

            // Register handlers for invalid configuration elements
            _serializer.UnknownAttribute += new XmlAttributeEventHandler(HandleUnknownAttribute);
            _serializer.UnknownElement += new XmlElementEventHandler(HandleUnknownElement);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the default configuration.
        /// </summary>
        public static CodeConfiguration Default
        {
            get
            {
                if (_default == null)
                {
                    lock (_defaultLock)
                    {
                        if (_default == null)
                        {
                            // Load the default configuration from the embedded resource file.
                            using (Stream resourceStream =
                                typeof(CodeConfiguration).Assembly.GetManifestResourceStream(
                                typeof(CodeConfiguration).Assembly.GetName().Name + ".DefaultConfig.xml"))
                            {
                                _default = Load(resourceStream);
                            }
                        }
                    }
                }

                return _default;
            }
        }

        /// <summary>
        /// Gets or sets the closing comment configuration.
        /// </summary>
        [Description("The settings for closing comments (Obsolete - Use Formatting.ClosingComments instead).")]
        [DisplayName("Closing comments (Obsolete)")]
        [ReadOnly(true)]
        [Browsable(false)]
        public ClosingCommentConfiguration ClosingComments
        {
            get
            {
                return null;
            }
            set
            {
                Formatting.ClosingComments = value;
            }
        }

        /// <summary>
        /// Gets or sets the encoding configuration.
        /// </summary>
        [Description("The encoding settings used for reading and writing source code files.")]
        [ReadOnly(true)]
        public EncodingConfiguration Encoding
        {
            get
            {
                if (_encoding == null)
                {
                    lock (this)
                    {
                        if (_encoding == null)
                        {
                            // Default encoding configuration
                            _encoding = new EncodingConfiguration();
                        }
                    }
                }

                return _encoding;
            }
            set
            {
                _encoding = value;
            }
        }

        /// <summary>
        /// Gets or sets the formatting configuration.
        /// </summary>
        [Description("Formatting settings.")]
        [DisplayName("Formatting")]
        [ReadOnly(true)]
        public FormattingConfiguration Formatting
        {
            get
            {
                if (_formatting == null)
                {
                    lock (this)
                    {
                        if (_formatting == null)
                        {
                            // Default style configuration
                            _formatting = new FormattingConfiguration();
                        }
                    }
                }

                return _formatting;
            }
            set
            {
                _formatting = value;
            }
        }

        /// <summary>
        /// Gets the collection of source code/project handlers.
        /// </summary>
        [XmlArrayItem(typeof(SourceHandlerConfiguration))]
        [XmlArrayItem(typeof(ProjectHandlerConfiguration))]
        [Description("The list of project/language handlers and their settings.")]
        public HandlerConfigurationCollection Handlers
        {
            get
            {
                if (_handlers == null)
                {
                    lock (this)
                    {
                        if (_handlers == null)
                        {
                            _handlers = new HandlerConfigurationCollection();
                        }
                    }
                }

                return _handlers;
            }
        }

        /// <summary>
        /// Gets or sets the regions configuration.
        /// </summary>
        [Description("The settings for all regions (Obsolete - Use Formatting.Regions instead).")]
        [ReadOnly(true)]
        [Browsable(false)]
        public RegionFormatConfiguration Regions
        {
            get
            {
                return null;
            }
            set
            {
                Formatting.Regions = value;
            }
        }

        /// <summary>
        /// Gets or sets the tab configuration.
        /// </summary>
        [Description("The settings for indentation (Obsolete - Use Formatting.Tabs instead).")]
        [ReadOnly(true)]
        [Browsable(false)]
        public TabConfiguration Tabs
        {
            get
            {
                return null;
            }
            set
            {
                Formatting.Tabs = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Loads a configuration from the specified file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <returns>The loaded code configuration if succesful, otherwise null.</returns>
        public static CodeConfiguration Load(string fileName)
        {
            return Load(fileName, true);
        }

        /// <summary>
        /// Loads a configuration from the specified file.
        /// </summary>
        /// <param name="fileName">Configuration file name.</param>
        /// <param name="resolveReferences">Resolve element references.</param>
        /// <returns>The code configuration if succesful, otherwise null.</returns>
        public static CodeConfiguration Load(string fileName, bool resolveReferences)
        {
            var ass = System.Reflection.Assembly.GetExecutingAssembly();
            var names = ass.GetManifestResourceNames();
            using (var strm = ass.GetManifestResourceStream("NArrange.Core.NArrangeConfig.xml"))
            {
                return Load(strm, resolveReferences);
            }
            //using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            //{
            //    return Load(fileStream, resolveReferences);
            //}
        }

        /// <summary>
        /// Loads a configuration from a stream.
        /// </summary>
        /// <param name="stream">The stream to load the configuration from.</param>
        /// <returns>The code configuration if succesful, otherwise null.</returns>
        public static CodeConfiguration Load(Stream stream)
        {
            return Load(stream, true);
        }

        /// <summary>
        /// Loads a configuration from a stream.
        /// </summary>
        /// <param name="stream">The sream to load the configuration from.</param>
        /// <param name="resolveReferences">
        /// Whether or not element references should be resolved.
        /// </param>
        /// <returns>The code configuration if succesful, otherwise null.</returns>
        public static CodeConfiguration Load(Stream stream, bool resolveReferences)
        {
            CodeConfiguration configuration = null;
            ////try
            ////{
            configuration = (CodeConfiguration)_serializer.Deserialize(stream);
            //}
            //catch
            //{
                //configuration = new CodeConfiguration();

                //configuration._encoding = new EncodingConfiguration();
                //configuration._encoding.CodePage = "Detect";

                //configuration._formatting = new FormattingConfiguration();
                //configuration._formatting.ClosingComments.Enabled = false;
                //configuration._formatting.ClosingComments.Format = "End $(ElementType) $(Name)";
                //configuration._formatting.LineSpacing.RemoveConsecutiveBlankLines = true;
                //configuration._formatting.Regions.CommentDirectiveBeginFormat = " $(Begin) {0}";
                //configuration._formatting.Regions.CommentDirectiveBeginPattern = "^\\s?\\$\\(\\s?Begin\\s?\\)\\s?(?<Name>.*)$";
                //configuration._formatting.Regions.CommentDirectiveEndFormat = " $(End) {0}";
                //configuration._formatting.Regions.CommentDirectiveEndPattern = "^\\s?\\$\\(\\s?End\\s?\\)\\s?(?<Name>.*)?$";
                //configuration._formatting.Regions.EndRegionNameEnabled = false;
                //configuration._formatting.Regions.Style = NArrange.Core.RegionStyle.NoDirective;
                //configuration._formatting.Tabs.SpacesPerTab = 4;
                //configuration._formatting.Tabs.TabStyle = TabStyle.Spaces;
                //configuration._formatting.Usings.MoveTo = CodeLevel.Namespace;

                //configuration._handlers = new HandlerConfigurationCollection();
                //configuration._handlers.Add(new ProjectHandlerConfiguration() { AssemblyName = null });
                //configuration._handlers.Add(new ProjectHandlerConfiguration() { AssemblyName = null });


                //{
                //    var source = new ExtensionConfigurationCollection();
                //    source.Add(new ExtensionConfiguration() { FilterBy = new FilterBy() { Condition = "!($(File.Name) : '.Designer.')" }, Name = "cs" });
                //    configuration._handlers.Add(new SourceHandlerConfiguration(source) { Language = "CSharp", AssemblyName = "NArrange.CSharp, Version=0.2.9.0, Culture=neutral, PublicKeyToken=null" });
                //}

                //configuration.ClosingComments = null;

                //configuration._elements = new ConfigurationElementCollection();
                //{
                //    //0
                //    var e2 = new ConfigurationElementCollection();
                //    e2.Add(new ElementConfiguration() { ElementType = Core.ElementType.Comment, FilterBy = new FilterBy() { Condition = "$(Type) != 'XmlLine'" } });
                //    configuration._elements.Add(new RegionConfiguration() { _elements = e2 });
                //}
                ////1
                //configuration._elements.Add(new ElementConfiguration() { ElementType = ElementType.Using, Id = "DefaultUsing", SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                ////2
                //configuration._elements.Add(new ElementConfiguration() { ElementType = ElementType.Attribute });
                ////3
                //{
                //    var e2 = new ConfigurationElementCollection();
                //    //0
                //    e2.Add(new ElementReferenceConfiguration() { Id = "DefaultNamespace" });
                //    //1
                //    e2.Add(new ElementReferenceConfiguration() { Id = "DefaultInterface" });
                //    //2
                //    e2.Add(new ElementReferenceConfiguration() { Id = "DefaultType" });
                //    //3
                //    e2.Add(new ElementConfiguration() { ElementType = Core.ElementType.NotSpecified, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });

                //    configuration._elements.Add(new ElementConfiguration() { ElementType = ElementType.ConditionDirective, Id = "DefaultConditional", _elements = e2 });
                //}
                ////4
                //configuration._elements.Add(new ElementReferenceConfiguration() { Id = "DefaultInterface" });
                ////5
                //configuration._elements.Add(new ElementReferenceConfiguration() { Id = "DefaultType" });
                ////6
                //{
                //    var e2 = new ConfigurationElementCollection();
                //    //0
                //    e2.Add(new ElementReferenceConfiguration() { Id = "DefaultUsing" });
                //    //1
                //    e2.Add(new ElementReferenceConfiguration() { Id = "DefaultConditional" });
                //    //2
                //    {
                //        var e3 = new ConfigurationElementCollection();
                //        e3.Add(new ElementConfiguration() { Id = "DefaultEnumeration", SortBy = new SortBy() { By = ElementAttributeType.Access, Direction = SortDirection.Descending, InnerSortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } }, FilterBy = new FilterBy() { Condition = "$(Type) == 'Enum'" } });
                //        e2.Add(new RegionConfiguration() { Name = "Enumerations", _elements = e3 });
                //    }
                //    //3
                //    {
                //        var e3 = new ConfigurationElementCollection();
                //        e3.Add(new ElementConfiguration() { ElementType = ElementType.Delegate, Id = "DefaultDelegate", SortBy = new SortBy() { By = ElementAttributeType.Access, Direction = SortDirection.Descending, InnerSortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } } });
                //        e2.Add(new RegionConfiguration() { Name = "Delegates", DirectivesEnabled = true, _elements = e3 });
                //    }
                //    //4                
                //    {
                //        var e3 = new ConfigurationElementCollection();
                //        //0
                //        {
                //            var e4 = new ConfigurationElementCollection();
                //            //0
                //            e4.Add(new ElementReferenceConfiguration() { Id = "DefaultConditional" });
                //            //1
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                //0
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { FilterBy = new FilterBy() { Condition = "!($(Name) : '.')" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Interface Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //1
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Event, FilterBy = new FilterBy() { Condition = "$(Name) : '.'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Inherited Interface Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                e4.Add(new RegionConfiguration() { Name = "Events", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //2
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                //0
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Property, FilterBy = new FilterBy() { Condition = "!($(Name) : '.')" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });

                //                    e5.Add(new RegionConfiguration() { Name = "Interface Properies", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //1
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Property, FilterBy = new FilterBy() { Condition = "!($(Name) : '.')" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Inherited Interface Properties", DirectivesEnabled = false, _elements = e6 });
                //                }

                //                e4.Add(new RegionConfiguration() { Name = "Properties", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //3
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                //0
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Method, FilterBy = new FilterBy() { Condition = "!($(Name) : '.')" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Interface Methods", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //1
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Method, FilterBy = new FilterBy() { Condition = "!($(Name) : '.')" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Inherited Interface Methods", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                e4.Add(new RegionConfiguration() { Name = "Methods", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //4
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                //0
                //                e5.Add(new ElementConfiguration() { ElementType = ElementType.NotSpecified, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                e4.Add(new RegionConfiguration() { Name = "Other", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            e3.Add(new ElementConfiguration() { ElementType = ElementType.Type, Id = "DefaultInterface", FilterBy = new FilterBy() { Condition = "$(Type) == 'Interface'" }, SortBy = new SortBy() { By = ElementAttributeType.Type, Direction = SortDirection.Descending, InnerSortBy = new SortBy() { By = ElementAttributeType.Access, Direction = SortDirection.Descending, InnerSortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } } }, _elements = e4 });
                //        }

                //        //1
                //        {
                //            var e4 = new ConfigurationElementCollection();
                //            //0
                //            e4.Add(new ElementReferenceConfiguration() { Id = "DefaultConditional" });
                //            //1
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                e5.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Parent.Attributes) : 'StructLayout' And !($(Modifier) : 'Static')" } });
                //                e4.Add(new RegionConfiguration() { Name = "Fixed Fields", DirectivesEnabled = false, _elements = e5 });
                //            }
                //            //2
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                // ce ne sono 12!!!!
                //                //0
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) == 'Public' And ($(Modifier) : 'Constant' Or $(Modifier) : 'ReadOnly')" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Public Constant/Read-Only Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //1
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) == 'Public' And $(Modifier) : 'Static'" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Public Static Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //2
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) == 'Public'" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Public Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //3
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) : 'Internal' And ($(Modifier) : 'Constant' Or $(Modifier) : 'ReadOnly')" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Internal Constant/Read-Only Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //4
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) : 'Internal' And $(Modifier) : 'Static'" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Internal Static Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //5
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) : 'Internal'" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Internal Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //6
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) == 'Protected'  And ($(Modifier) : 'Constant' Or $(Modifier) : 'ReadOnly')" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Protected Constant/Read-Only Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //7
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) == 'Protected' And $(Modifier) : 'Static'" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Protected Static Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //8
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) == 'Protected'" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Protected Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //9
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) == 'Private' And ($(Modifier) : 'Constant' Or $(Modifier) : 'ReadOnly')" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Private Constant/Read-Only Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //10
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) == 'Private' And $(Modifier) : 'Static'" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Private Static Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //11
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = ElementType.Field, FilterBy = new FilterBy() { Condition = "$(Access) == 'Private'" }, GroupBy = new GroupBy() { By = ElementAttributeType.Modifier, Direction = SortDirection.Descending, SeparatorType = GroupSeparatorType.NewLine }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Private Fields", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                e4.Add(new RegionConfiguration() { Name = "Fields", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //3
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                //0
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Constructor, FilterBy = new FilterBy() { Condition = "$(Modifier) : 'Static'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Static Constructor", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //1
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Constructor, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Other Constructor", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                e4.Add(new RegionConfiguration() { Name = "Constructors", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //4
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                e5.Add(new ElementReferenceConfiguration() { Id = "DefaultEnumeration" });
                //                e4.Add(new RegionConfiguration() { Name = "Enumerations", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //5
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                e5.Add(new ElementReferenceConfiguration() { Id = "DefaultDelegate" });
                //                e4.Add(new RegionConfiguration() { Name = "Delegates", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //6
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                e5.Add(new ElementReferenceConfiguration() { Id = "DefaultInterface" });
                //                e4.Add(new RegionConfiguration() { Name = "Nested Interface", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //7
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                //ce ne sono 8!
                //                //0
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Event, FilterBy = new FilterBy() { Condition = "$(Access) == 'Public' And $(Modifier) : 'Static'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Public Static Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //1
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Event, FilterBy = new FilterBy() { Condition = "$(Access) == 'Public'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Public Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //2
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Event, FilterBy = new FilterBy() { Condition = "$(Access) == 'Internal' And $(Modifier) : 'Static'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Internal Static Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //3
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Event, FilterBy = new FilterBy() { Condition = "$(Access) == 'Internal'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Internal Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //4
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Event, FilterBy = new FilterBy() { Condition = "$(Access) == 'Protected' And $(Modifier) : 'Static'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Portected Static Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //5
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Event, FilterBy = new FilterBy() { Condition = "$(Access) == 'Protected'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Protected Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //6
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Event, FilterBy = new FilterBy() { Condition = "($(Access) == 'Private' Or $(Access) == 'None') And $(Modifier) : 'Static'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Private Static Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //7
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Event, FilterBy = new FilterBy() { Condition = "$(Access) == 'Private' Or $(Access) == 'None'" }, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                    e5.Add(new RegionConfiguration() { Name = "Private Events", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                e4.Add(new RegionConfiguration() { Name = "Events", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //8
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                //ce ne sono 9!
                //                //0
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Property, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Public' And $(Modifier) : 'Static'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Public Static Properties", DirectivesEnabled = false, _elements = e6 });

                //                }
                //                //1
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Property, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Public' And $(Name) != 'this'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Public Properties", DirectivesEnabled = false, _elements = e6 });

                //                }
                //                //2
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Property, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'None' And $(Name) : '.'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Explicit Interface Properties", DirectivesEnabled = false, _elements = e6 });

                //                }
                //                //3
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Property, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Internal' And $(Modifier) : 'Static'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Internal Static Properties", DirectivesEnabled = false, _elements = e6 });

                //                }
                //                //4
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Property, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Internal' And $(Name) != 'this'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Internal Properties", DirectivesEnabled = false, _elements = e6 });

                //                }
                //                //5
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Property, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Protected' And $(Modifier) : 'Static'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Protected Static Properties", DirectivesEnabled = false, _elements = e6 });

                //                }
                //                //6
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Property, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Protected' And $(Name) != 'this'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Protected Properties", DirectivesEnabled = false, _elements = e6 });

                //                }
                //                //7
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Property, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "($(Access) == 'Private' Or $(Access) == 'None') And $(Modifier) : 'Static'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Private Static Properties", DirectivesEnabled = false, _elements = e6 });

                //                }
                //                //8
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Property, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "($(Access) == 'Private' Or $(Access) == 'None') And $(Name) != 'this'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Private Properties", DirectivesEnabled = false, _elements = e6 });

                //                }
                //                e4.Add(new RegionConfiguration() { Name = "Properties", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //9
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                e5.Add(new ElementConfiguration() { ElementType = ElementType.Property, FilterBy = new FilterBy() { Condition = "$(Name) == 'this'" }, SortBy = new SortBy() { By = ElementAttributeType.Access, Direction = SortDirection.Ascending, InnerSortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } } });
                //                e4.Add(new RegionConfiguration() { Name = "Indexers", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //10
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                //c ne sono 8!
                //                //0
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Method, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Public' And $(Modifier) : 'Static'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Public Static Method", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //1
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Method, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Public'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Public Method", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //2
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Method, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Internal' And $(Modifier) : 'Static'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Internal Static Method", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //3
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Method, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Internal'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Internal Method", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //4
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Method, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Protected' And $(Modifier) : 'Static'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Protected Static Method", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //5
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Method, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Protected'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Protected Method", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //6
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Method, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "($(Access) == 'Private' Or $(Access) == 'None') And $(Modifier) : 'Static'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Private Static Method", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                //7
                //                {
                //                    var e6 = new ConfigurationElementCollection();
                //                    e6.Add(new ElementConfiguration() { ElementType = Core.ElementType.Method, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, FilterBy = new FilterBy() { Condition = "$(Access) == 'Private' Or $(Access) == 'None'" } });
                //                    e5.Add(new RegionConfiguration() { Name = "Private Method", DirectivesEnabled = false, _elements = e6 });
                //                }
                //                e4.Add(new RegionConfiguration() { Name = "Methods", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //11
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                e5.Add(new ElementReferenceConfiguration() { Id = "Default Type" });
                //                e4.Add(new RegionConfiguration() { Name = "Nested Types", DirectivesEnabled = true, _elements = e5 });
                //            }
                //            //12
                //            {
                //                var e5 = new ConfigurationElementCollection();
                //                e5.Add(new ElementConfiguration() { ElementType = Core.ElementType.NotSpecified, SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending } });
                //                e4.Add(new RegionConfiguration() { Name = "Others", DirectivesEnabled = true, _elements = e5 });
                //            }

                //            e3.Add(new ElementConfiguration() { ElementType = ElementType.Type, Id = "DefaultType", SortBy = new SortBy() { By = ElementAttributeType.Type, Direction = SortDirection.Descending, InnerSortBy = new SortBy() { By = ElementAttributeType.Access, Direction = SortDirection.Descending, InnerSortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Descending } } }, _elements = e4 });
                //        }
                //        e2.Add(new RegionConfiguration() { Name = "Types", DirectivesEnabled = false, _elements = e3 });
                //    }
                //    configuration._elements.Add(new ElementConfiguration() { ElementType = Core.ElementType.Namespace, Id = "DefaultNamespace", SortBy = new SortBy() { By = ElementAttributeType.Name, Direction = SortDirection.Ascending }, _elements = e2 });
                //}
            //}



            if (resolveReferences)
            {
                configuration.ResolveReferences();
            }

            configuration.Upgrade();

            return configuration;
        }


        /// <summary>
        /// Override Clone so that we can force resolution of element references.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            CodeConfiguration clone = base.Clone() as CodeConfiguration;
            clone.ResolveReferences();

            return clone;
        }

        /// <summary>
        /// Resolves any reference elements in the configuration.
        /// </summary>
        public void ResolveReferences()
        {
            Dictionary<string, ElementConfiguration> elementMap = new Dictionary<string, ElementConfiguration>();
            List<ElementReferenceConfiguration> elementReferences = new List<ElementReferenceConfiguration>();

            Action<ConfigurationElement> populateElementMap = delegate(ConfigurationElement element)
            {
                ElementConfiguration elementConfiguration = element as ElementConfiguration;
                if (elementConfiguration != null && elementConfiguration.Id != null)
                {
                    elementMap.Add(elementConfiguration.Id, elementConfiguration);
                }
            };

            Action<ConfigurationElement> populateElementReferenceList = delegate(ConfigurationElement element)
            {
                ElementReferenceConfiguration elementReference = element as ElementReferenceConfiguration;
                if (elementReference != null && elementReference.Id != null)
                {
                    elementReferences.Add(elementReference);
                }
            };

            Action<ConfigurationElement>[] actions = new Action<ConfigurationElement>[]
                {
                    populateElementMap,
                    populateElementReferenceList
                };

            TreeProcess(this, actions);

            // Resolve element references
            foreach (ElementReferenceConfiguration reference in elementReferences)
            {
                ElementConfiguration referencedElement = null;
                elementMap.TryGetValue(reference.Id, out referencedElement);
                if (referencedElement != null)
                {
                    reference.ReferencedElement = referencedElement;
                }
                else
                {
                    throw new InvalidOperationException(
                        string.Format(
                        "Unable to resolve element reference for Id={0}.",
                        reference.Id));
                }
            }
        }

        /// <summary>
        /// Saves the configuration to a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Save(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                _serializer.Serialize(stream, this);
            }
        }

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>A clone of the instance.</returns>
        protected override ConfigurationElement DoClone()
        {
            CodeConfiguration clone = new CodeConfiguration();

            clone._encoding = Encoding.Clone() as EncodingConfiguration;
            clone._formatting = Formatting.Clone() as FormattingConfiguration;

            foreach (HandlerConfiguration handler in Handlers)
            {
                HandlerConfiguration handlerClone = handler.Clone() as HandlerConfiguration;
                clone.Handlers.Add(handlerClone);
            }

            return clone;
        }

        /// <summary>
        /// Handler for unknown attributes.
        /// </summary>
        /// <param name="sender">The sender/</param>
        /// <param name="e">Event arguments.</param>
        private static void HandleUnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            throw new InvalidOperationException(e.ToString() + " Unknown attribute " + e.Attr.Name);
        }

        /// <summary>
        /// Handler for unknown elements.
        /// </summary>
        /// <param name="sender">The sender/</param>
        /// <param name="e">Event arguments.</param>
        private static void HandleUnknownElement(object sender, XmlElementEventArgs e)
        {
            throw new InvalidOperationException(e.ToString() + " Unknown element " + e.Element.Name);
        }

        /// <summary>
        /// Recurses through the configuration tree and executes actions against 
        /// each configuration element.
        /// </summary>
        /// <param name="element">Element to process.</param>
        /// <param name="actions">Actions to perform.</param>
        private void TreeProcess(ConfigurationElement element, Action<ConfigurationElement>[] actions)
        {
            if (element != null)
            {
                foreach (ConfigurationElement childElement in element.Elements)
                {
                    foreach (Action<ConfigurationElement> action in actions)
                    {
                        action(childElement);
                    }

                    TreeProcess(childElement, actions);
                }
            }
        }

        /// <summary>
        /// Upgrades the configuration.
        /// </summary>
        private void Upgrade()
        {
            UpgradeProjectExtensions();
        }

        /// <summary>
        /// Moves project extensions to the new format.
        /// </summary>
        private void UpgradeProjectExtensions()
        {
            // Migrate project handler configurations
            string parserType = typeof(MSBuildProjectParser).FullName;
            ProjectHandlerConfiguration projectHandlerConfiguration = null;
            foreach (HandlerConfiguration handlerConfiguration in Handlers)
            {
                if (handlerConfiguration.HandlerType == HandlerType.Project)
                {
                    ProjectHandlerConfiguration candidateConfiguration = handlerConfiguration as ProjectHandlerConfiguration;
                    if (candidateConfiguration.ParserType != null &&
                        candidateConfiguration.ParserType.ToUpperInvariant() == parserType.ToUpperInvariant())
                    {
                        projectHandlerConfiguration = candidateConfiguration;
                        break;
                    }
                }
            }

            //
            // Create the new project configuration if necessary
            //
            if (projectHandlerConfiguration == null)
            {
                projectHandlerConfiguration = new ProjectHandlerConfiguration();
                projectHandlerConfiguration.ParserType = parserType;
                Handlers.Insert(0, projectHandlerConfiguration);
            }

            foreach (HandlerConfiguration handlerConfiguration in Handlers)
            {
                if (handlerConfiguration.HandlerType == HandlerType.Source)
                {
                    SourceHandlerConfiguration sourceHandlerConfiguration = handlerConfiguration as SourceHandlerConfiguration;
                    foreach (ExtensionConfiguration projectExtension in sourceHandlerConfiguration.ProjectExtensions)
                    {
                        bool upgraded = false;
                        foreach (ExtensionConfiguration upgradedExtension in projectHandlerConfiguration.ProjectExtensions)
                        {
                            if (string.Compare(upgradedExtension.Name, projectExtension.Name, true) == 0)
                            {
                                upgraded = true;
                                break;
                            }
                        }

                        if (!upgraded)
                        {
                            projectHandlerConfiguration.ProjectExtensions.Add(projectExtension);
                        }
                    }

                    sourceHandlerConfiguration.ProjectExtensions.Clear();
                }
            }
        }

        #endregion Methods
    }
}