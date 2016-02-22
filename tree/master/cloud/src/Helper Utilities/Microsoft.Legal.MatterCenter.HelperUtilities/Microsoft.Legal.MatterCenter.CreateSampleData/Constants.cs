// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateSampleData
// Author           : v-akdigh
// Created          : 08-27-2014
//
// ***********************************************************************
// <copyright file="Constants.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains list of constants used.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateSampleData
{
    /// <summary>
    /// Class to store constants
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Represents aspx extension
        /// </summary>
        internal const string AspxExtension = ".aspx";

        /// <summary>
        /// Represents backslash character
        /// </summary>
        internal const string Backslash = "/";

        /// <summary>
        /// Represents calendar name
        /// </summary>
        internal const string CalendarName = "Calendar";

        /// <summary>
        /// Represents closing curly brace
        /// </summary>
        internal const string ClosingCurlyBrace = "}";

        /// <summary>
        /// Represents default layout
        /// </summary>
        internal const string DefaultLayout = "ReportCenterLayout";

        /// <summary>
        /// Represents dms role query
        /// </summary>
        internal const string DMSRoleQuery = "<View></View>";

        /// <summary>
        /// Represents false value
        /// </summary>
        internal const string FALSE = "false";

        /// <summary>
        /// Represents footer zone
        /// </summary>
        internal const string FooterZone = "FooterZone";

        /// <summary>
        /// Represents header zone
        /// </summary>
        internal const string HeaderZone = "HeaderZone";

        /// <summary>
        /// Represents master page gallery
        /// </summary>
        internal const string MasterPageGallery = "Master Page Gallery";

        /// <summary>
        /// Represents middle center zone
        /// </summary>
        internal const string MiddleCenterZone = "MiddleMiddleZone";

        /// <summary>
        /// Represents middle left zone
        /// </summary>
        internal const string MiddleLeftZone = "MiddleLeftZone";

        /// <summary>
        /// Represents no input message
        /// </summary>
        internal const string MessageNoInputs = "No Inputs";

        /// <summary>
        /// Represents opening curly braces
        /// </summary>
        internal const string OpeningCurlyBrace = "{";

        /// <summary>
        /// Represents right zone
        /// </summary>
        internal const string RightZone = "RightZone";

        /// <summary>
        /// Represents top zone
        /// </summary>
        internal const string TopZone = "TopLeftZone";

        /// <summary>
        /// Represents service response
        /// </summary>
        internal const string ServiceResponse = "{{ \"code\": \"{0}\", \"value\": \"{1}\" }}";

        /// <summary>
        /// Represents true value
        /// </summary>
        internal const string TRUE = "true";

        /// <summary>
        /// Represents WOPI frame url
        /// </summary>
        internal const string WOPIFrameURL = "_layouts/WopiFrame.aspx?sourcedoc=";

        /// <summary>
        /// Represents colon character
        /// </summary>
        internal const string COLON = ":";

        /// <summary>
        /// Represents space character
        /// </summary>
        internal const string SPACE = " ";

        /// <summary>
        /// Represents seperator
        /// </summary>
        internal const string SEPARATOR = "$|$";

        /// <summary>
        /// Represents zero document count
        /// </summary>
        internal const string ZERO_DOCUMENT_COUNT = "0";

        /// <summary>
        /// Represents provision prerequisite success
        /// </summary>
        internal const string MatterProvisionPrerequisitesSuccess = "0";

        /// <summary>
        /// Represents Matter library existence
        /// </summary>
        internal const string MatterLibraryExists = "1";

        /// <summary>
        /// Represents OneNote library existence
        /// </summary>
        internal const string OneNoteLibraryExists = "2";

        /// <summary>
        /// Represents matter landing page existence
        /// </summary>
        internal const string MatterLandingPageExists = "3";

        /// <summary>
        /// Represents matter id validation 
        /// </summary>
        internal const string InvalidMatterID = "4";

        /// <summary>
        /// Represents matter title validation
        /// </summary>
        internal const string InvalidMatterTitle = "5";

        /// <summary>
        /// Represents matter description validation
        /// </summary>
        internal const string InvalidMatterDescription = "6";

        /// <summary>
        /// Represents conflict date validation
        /// </summary>
        internal const string InvalidConflictDate = "7";

        /// <summary>
        /// Lists path in GUID URL
        /// </summary>
        internal const string TitleListPath = "Lists/";

        /// <summary>
        /// Gets the XML chunk for RSS feed web part
        /// </summary>
        internal static string RssFeedWebpart
        {
            get
            {
                return rssFeedWebpart;
            }
        }

        /// <summary>
        /// XML definition of the RSS feed web part
        /// </summary>
        private static string rssFeedWebpart = @"<webParts>
                  <webPart xmlns=""http://schemas.microsoft.com/WebPart/v3"">
                    <metaData>
                      <type name=""Microsoft.SharePoint.Portal.WebControls.RSSAggregatorWebPart, Microsoft.SharePoint.Portal, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" />
                      <importErrorMessage>Cannot import this web part.</importErrorMessage>
                    </metaData>
                    <data>
                      <properties>
                        <property name=""InitialAsyncDataFetch"" type=""bool"">False</property>
                        <property name=""ChromeType"" type=""chrometype"">None</property>
                        <property name=""ListName"" type=""string"" null=""true"" />
                        <property name=""Height"" type=""string"" />
                        <property name=""CacheXslStorage"" type=""bool"">True</property>
                        <property name=""Default"" type=""string"" />
                        <property name=""ParameterBindings"" type=""string"">&lt;ParameterBinding Name=""RequestUrl"" Location=""WPProperty[FeedUrl]""/&gt;</property>
                        <property name=""AllowZoneChange"" type=""bool"">True</property>
                        <property name=""AutoRefresh"" type=""bool"">False</property>
                        <property name=""XmlDefinitionLink"" type=""string"" />
                        <property name=""DataFields"" type=""string"" />
                        <property name=""FeedLimit"" type=""int"">5</property>
                        <property name=""Hidden"" type=""bool"">False</property>
                        <property name=""NoDefaultStyle"" type=""string"" />
                        <property name=""XslLink"" type=""string"" null=""true"" />
                        <property name=""ViewFlag"" type=""string"">0</property>
                        <property name=""CatalogIconImageUrl"" type=""string"" />
                        <property name=""CacheXslTimeOut"" type=""int"">600</property>
                        <property name=""AutoRefreshInterval"" type=""int"">60</property>
                        <property name=""AllowConnect"" type=""bool"">True</property>
                        <property name=""FeedUrl"" type=""string"">http://www.bing.com/search?q={0}&amp;format=rss</property>
                        <property name=""AllowClose"" type=""bool"">True</property>
                        <property name=""ShowWithSampleData"" type=""bool"">False</property>       
                        <property name=""EnableOriginalValue"" type=""bool"">False</property>
                        <property name=""ExpandFeed"" type=""bool"">False</property>
                        <property name=""ListUrl"" type=""string"" null=""true"" />
                        <property name=""DataSourceID"" type=""string"" />
                        <property name=""FireInitialRow"" type=""bool"">True</property>
                        <property name=""ManualRefresh"" type=""bool"">False</property>
                        <property name=""ViewFlags"" type=""Microsoft.SharePoint.SPViewFlags, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">None</property>
                        <property name=""ChromeState"" type=""chromestate"">Normal</property>
                        <property name=""AllowHide"" type=""bool"">True</property>
                        <property name=""ListDisplayName"" type=""string"" null=""true"" />
                        <property name=""SampleData"" type=""string"" null=""true"" />
                        <property name=""AsyncRefresh"" type=""bool"">False</property>
                        <property name=""Direction"" type=""direction"">NotSet</property>
                        <property name=""Title"" type=""string"">RSS Viewer</property>
                        <property name=""ListId"" type=""System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">00000000-0000-0000-0000-000000000000</property>
                        <property name=""Description"" type=""string"">Displays an RSS feed.</property>
                        <property name=""AllowMinimize"" type=""bool"">True</property>
                        <property name=""TitleUrl"" type=""string"" />
                        <property name=""DataSourcesString"" type=""string"">
                        &lt;%@ Register TagPrefix=""WebControls"" Namespace=""Microsoft.SharePoint.WebControls"" Assembly=""Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" %&gt;
                        &lt;%@ Register TagPrefix=""WebPartPages"" Namespace=""Microsoft.SharePoint.WebPartPages"" Assembly=""Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" %&gt;
                        &lt;WebControls:XmlUrlDataSource runat=""server"" AuthType=""None"" HttpMethod=""GET""&gt;
                        &lt;DataFileParameters&gt;
                        &lt;WebPartPages:DataFormParameter Name=""RequestUrl"" ParameterKey=""RequestUrl"" PropertyName=""ParameterValues""/&gt;
                        &lt;/DataFileParameters&gt;
                        &lt;/WebControls:XmlUrlDataSource&gt;</property>
                        <property name=""DisplayName"" type=""string"" />
                        <property name=""PageType"" type=""Microsoft.SharePoint.PAGETYPE, Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">PAGE_NORMALVIEW</property>
                        <property name=""Width"" type=""string"" />
                        <property name=""AllowEdit"" type=""bool"">True</property>
                        <property name=""ExportMode"" type=""exportmode"">All</property>
                        <property name=""CacheRefreshTimeInMins"" type=""int"">120</property>
                        <property name=""PageSize"" type=""int"">-1</property>
                        <property name=""ViewContentTypeId"" type=""string"" />
                        <property name=""HelpUrl"" type=""string"" />
                        <property name=""XmlDefinition"" type=""string"" />
                        <property name=""UseSQLDataSourcePaging"" type=""bool"">True</property>
                        <property name=""TitleIconImageUrl"" type=""string"" />
                        <property name=""MissingAssembly"" type=""string"">Cannot import this web part.</property>
                        <property name=""HelpMode"" type=""helpmode"">Modeless</property>
                      </properties>
                    </data>
                  </webPart>
                </webParts>";
        
        /// <summary>
        /// Gets the XML chunk for List view Web part
        /// </summary>
        internal static string ListviewWebpart
        {
            get
            {
                return listviewWebpart;
            }
        }

        /// <summary>
        /// XML definition of the list view web part
        /// </summary>
        private static string listviewWebpart = @"
                <webParts>
                    <webPart xmlns=""http://schemas.microsoft.com/WebPart/v3"">
                        <metaData>
                            <type name=""Microsoft.SharePoint.WebPartPages.XsltListViewWebPart, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" />
                            <importErrorMessage>Cannot import this Web Part.</importErrorMessage>
                        </metaData>
                        <data>
                            <properties>
                                <property name=""ShowWithSampleData"" type=""bool"">False</property>
                                <property name=""Default"" type=""string"" />
                                <property name=""NoDefaultStyle"" type=""string"" null=""true"" />
                                <property name=""CacheXslStorage"" type=""bool"">True</property>
                                <property name=""ViewContentTypeId"" type=""string"" />
                                <property name=""XmlDefinitionLink"" type=""string"" />
                                <property name=""ManualRefresh"" type=""bool"">False</property>
                                <property name=""ListUrl"" type=""string"" />
                                <property name=""ListId"" type=""System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">{0}</property>
                                <property name=""TitleUrl"" type=""string"">{1}</property>
                                <property name=""EnableOriginalValue"" type=""bool"">False</property>
                                <property name=""Direction"" type=""direction"">NotSet</property>
                                <property name=""ServerRender"" type=""bool"">False</property>
                                <property name=""ViewFlags"" type=""Microsoft.SharePoint.SPViewFlags, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">Html, TabularView, Hidden, Mobile</property>
                                <property name=""AllowConnect"" type=""bool"">True</property>
                                <property name=""ListName"" type=""string"">{2}</property>
                                <property name=""ListDisplayName"" type=""string"" />
                                <property name=""AllowZoneChange"" type=""bool"">True</property>
                                <property name=""ChromeState"" type=""chromestate"">Normal</property>
                                <property name=""DisableSaveAsNewViewButton"" type=""bool"">False</property>
                                <property name=""ViewFlag"" type=""string"" />
                                <property name=""DataSourceID"" type=""string"" />
                                <property name=""ExportMode"" type=""exportmode"">All</property>
                                <property name=""AutoRefresh"" type=""bool"">False</property>
                                <property name=""FireInitialRow"" type=""bool"">True</property>
                                <property name=""AllowEdit"" type=""bool"">True</property>
                                <property name=""Description"" type=""string"" />
                                <property name=""HelpMode"" type=""helpmode"">Modeless</property>
                                <property name=""BaseXsltHashKey"" type=""string"" null=""true"" />
                                <property name=""AllowMinimize"" type=""bool"">True</property>
                                <property name=""CacheXslTimeOut"" type=""int"">86400</property>
                                <property name=""ChromeType"" type=""chrometype"">None</property>
                                <property name=""Xsl"" type=""string"" null=""true"" />
                                <property name=""JSLink"" type=""string"" null=""true"" />
                                <property name=""CatalogIconImageUrl"" type=""string"">/_layouts/15/images/itdl.png?rev=33</property>
                                <property name=""SampleData"" type=""string"" null=""true"" />
                                <property name=""UseSQLDataSourcePaging"" type=""bool"">True</property>
                                <property name=""TitleIconImageUrl"" type=""string"" />
                                <property name=""PageSize"" type=""int"">-1</property>
                                <property name=""ShowTimelineIfAvailable"" type=""bool"">True</property>
                                <property name=""Width"" type=""string"">750px</property>
                                <property name=""DataFields"" type=""string"" />
                                <property name=""Hidden"" type=""bool"">False</property>
                                <property name=""Title"" type=""string"" />
                                <property name=""PageType"" type=""Microsoft.SharePoint.PAGETYPE, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">PAGE_NORMALVIEW</property>
                                <property name=""DataSourcesString"" type=""string"" />
                                <property name=""AllowClose"" type=""bool"">True</property>
                                <property name=""InplaceSearchEnabled"" type=""bool"">True</property>
                                <property name=""WebId"" type=""System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">00000000-0000-0000-0000-000000000000</property>
                                <property name=""Height"" type=""string"" />
                                <property name=""GhostedXslLink"" type=""string"">main.xsl</property>
                                <property name=""DisableViewSelectorMenu"" type=""bool"">False</property>
                                <property name=""DisplayName"" type=""string"" />
                                <property name=""IsClientRender"" type=""bool"">False</property>
                                <property name=""XmlDefinition"" type=""string"">&lt;View Name=""{3}"" MobileView=""TRUE"" Type=""HTML"" Hidden=""TRUE"" DisplayName="" "" Url=""{4}"" Level=""1"" BaseViewID=""1"" ContentTypeID=""0x"" ImageUrl=""/_layouts/15/images/dlicon.png?rev=37"" &gt;&lt;Query&gt;&lt;OrderBy&gt;&lt;FieldRef Name=""FileLeafRef""/&gt;&lt;/OrderBy&gt;&lt;/Query&gt;&lt;ViewFields&gt;&lt;FieldRef Name=""DocIcon""/&gt;&lt;FieldRef Name=""LinkFilename""/&gt;&lt;FieldRef Name=""Modified""/&gt;&lt;FieldRef Name=""Editor""/&gt;&lt;/ViewFields&gt;&lt;RowLimit Paged=""TRUE""&gt;30&lt;/RowLimit&gt;&lt;JSLink&gt;clienttemplates.js&lt;/JSLink&gt;&lt;XslLink Default=""TRUE""&gt;main.xsl&lt;/XslLink&gt;&lt;Toolbar Type=""Standard""/&gt;&lt;/View&gt;</property>
                                <property name=""InitialAsyncDataFetch"" type=""bool"">False</property>
                                <property name=""AllowHide"" type=""bool"">True</property>
                                <property name=""ParameterBindings"" type=""string"">
                                    &lt;ParameterBinding Name=""dvt_sortdir"" Location=""Postback;Connection""/&gt;
                                    &lt;ParameterBinding Name=""dvt_sortfield"" Location=""Postback;Connection""/&gt;
                                    &lt;ParameterBinding Name=""dvt_startposition"" Location=""Postback"" DefaultValue="" ""/&gt;
                                    &lt;ParameterBinding Name=""dvt_firstrow"" Location=""Postback;Connection""/&gt;
                                    &lt;ParameterBinding Name=""OpenMenuKeyAccessible"" Location=""Resource(wss,OpenMenuKeyAccessible)"" /&gt;
                                    &lt;ParameterBinding Name=""open_menu"" Location=""Resource(wss,open_menu)"" /&gt;
                                    &lt;ParameterBinding Name=""select_deselect_all"" Location=""Resource(wss,select_deselect_all)"" /&gt;
                                    &lt;ParameterBinding Name=""idPresEnabled"" Location=""Resource(wss,idPresEnabled)"" /&gt;&lt;ParameterBinding Name=""NoAnnouncements"" Location=""Resource(wss,noitemsinview_doclibrary)"" /&gt;&lt;ParameterBinding Name=""NoAnnouncementsHowTo"" Location=""Resource(wss,noitemsinview_doclibrary_howto2)"" /&gt;</property>
                                <property name=""DataSourceMode"" type=""Microsoft.SharePoint.WebControls.SPDataSourceMode, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">List</property>
                                <property name=""AutoRefreshInterval"" type=""int"">60</property>
                                <property name=""AsyncRefresh"" type=""bool"">False</property>
                                <property name=""HelpUrl"" type=""string"" />
                                <property name=""MissingAssembly"" type=""string"">Cannot import this Web Part.</property>
                                <property name=""XslLink"" type=""string"" null=""true"" />
                                <property name=""SelectParameters"" type=""string"" />
                                <property name=""HasClientDataSource"" type=""bool"">False</property>
                            </properties>
                        </data>
                    </webPart>
                </webParts>";

        /// <summary>
        /// Represents order by
        /// </summary>
        internal const string ViewOrderByQuery = @"<OrderBy><FieldRef Name='{0}' Ascending='False' /></OrderBy>";
        
        /// <summary>
        /// XML definition of the left web part
        /// </summary>
        internal const string ContentEditorWebPart = @"<?xml version=""1.0"" encoding=""utf-8""?>
                    <WebPart xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/WebPart/v2"">
                      <Title>Content Editor</Title>
                      <FrameType>None</FrameType>
                      <Description>Allows authors to enter rich text content.</Description>
                      <IsIncluded>true</IsIncluded>
                      <ZoneID>MiddleLeftZone</ZoneID>
                      <PartOrder>0</PartOrder>
                      <FrameState>Normal</FrameState>
                      <Height />
                      <Width />
                      <AllowRemove>true</AllowRemove>
                      <AllowZoneChange>true</AllowZoneChange>
                      <AllowMinimize>true</AllowMinimize>
                      <AllowConnect>true</AllowConnect>
                      <AllowEdit>true</AllowEdit>
                      <AllowHide>true</AllowHide>
                      <IsVisible>true</IsVisible>
                      <DetailLink />
                      <HelpLink />
                      <HelpMode>Modeless</HelpMode>
                      <Dir>Default</Dir>
                      <PartImageSmall />
                      <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                      <PartImageLarge>/_layouts/15/images/mscontl.gif</PartImageLarge>
                      <IsIncludedFilter />
                      <Assembly>Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
                      <TypeName>Microsoft.SharePoint.WebPartPages.ContentEditorWebPart</TypeName>
                      <ContentLink xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                      <Content xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"">
                        <![CDATA[
                            {0}
                          ]]>
                      </Content>
                      <PartStorage xmlns=""http://schemas.microsoft.com/WebPart/v2/ContentEditor"" />
                    </WebPart>";

        /// <summary>
        /// Section to be appended on the page
        /// </summary>
        internal const string MatterLandingSectionContent = "<div id=\"{0}\"></div>";

        /// <summary>
        /// Style tag
        /// </summary>
        internal const string StyleTag = "<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />";

        /// <summary>
        /// Script tag
        /// </summary>
        internal const string ScriptTag = "<script src=\"{0}\" type=\"text/javascript\"></script>";

        /// <summary>
        /// Script tag with contents
        /// </summary>
        internal const string ScriptTagWithContents = "<script type=\"text/javascript\">{0}</script>";

        /// <summary>
        /// Matter landing stamp properties
        /// </summary>
        internal const string MatterLandingStampProperties = "var documentLibraryName = \"{0}\", isNewMatterLandingPage = true, documentLibraryGUID=\"{1}\";";

        /// <summary>
        /// String to be used for creating default value for metadata. This string is in following format: WSSID;#VAL|GUID
        /// </summary>
        internal const string MetadataDefaultValue = "{0};#{1}|{2}";

        /// <summary>
        /// Matter Landing page section mapping
        /// </summary>
        public enum MatterLandingSection
        {
            /// <summary>
            /// Task panel mapping
            /// </summary>
            TaskPanel,

            /// <summary>
            /// Calendar panel mapping
            /// </summary>
            CalendarPanel,

            /// <summary>
            /// Footer panel mapping
            /// </summary>
            FooterPanel,

            /// <summary>
            /// Information panel mapping
            /// </summary>
            InformationPanel,

            /// <summary>
            /// Header panel mapping
            /// </summary>
            HeaderPanel,

            /// <summary>
            /// RSS Title panel mapping
            /// </summary>
            RSSTitlePanel,

            /// <summary>
            /// OneNote panel mapping
            /// </summary>
            OneNotePanel
        }
    }
}
