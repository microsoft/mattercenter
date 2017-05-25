// ***********************************************************************
// Assembly         : OneNote_Rename_Utilty
// Author           : v-nikupa
// Created          : 02-03-2016
//
// ***********************************************************************
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file is used adds group.</summary>
// ***********************************************************************
namespace Microsoft.Legal.ProjectCenter.UpdateProjectName
{
    /// <summary>
    /// Static class to define constant value for utility
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Project description property name in property bag
        /// </summary>
        public const string PROJECTDESCRIPTION = "ProjectDescription";

        /// <summary>
        /// Constant specify the URL need to open in OneNote app
        /// </summary>
        public const string TRUESTATE = "True";

        /// <summary>
        /// Constant specify whether calendar present or not
        /// </summary>
        public const string ISCALENDARPRESENT = "IsCalendarPresent";

        /// <summary>
        /// Constant specify the Project GUID
        /// </summary>
        public const string PROJECTGUID = "ProjectGUID";

        /// <summary>
        /// Constant specify the Project ID
        /// </summary>
        public const string PROJECTID = "ProjectID";

        /// <summary>
        /// Constant specify the task present or not
        /// </summary>
        public const string ISTASKPRESENT = "IsTaskPresent";

        /// <summary>
        /// Special character expression to validate project title
        /// </summary>
        public const string SpecialCharacterExpressionProjectTitle = "[a-zA-Z0-9-_()' ]*$";

        /// <summary>
        /// Project name max length
        /// </summary>
        public const int ProjectNameLength = 120;

        /// <summary>
        /// File not found exception type
        /// </summary>
        public const string FILENOTFOUNDEXCEPTION = "System.IO.FileNotFoundException";

        /// <summary>
        /// Constant specify the RssFeed present or not
        /// </summary>
        public const string ISRSSFEEDPRESENT = "IsRSSFeedPresent";

        /// <summary>
        /// Constant specify whether the document library is project library
        /// </summary>
        public const string ISPROJECT = "IsProject";

        /// <summary>
        /// Constant specify the calendar
        /// </summary>
        public const string CALENDAR = "_Calendar";

        /// <summary>
        /// Constant specify the calendar pane
        /// </summary>
        public const string CALENDARPANE = "calendarPane";

        /// <summary>
        /// Constant specify the task
        /// </summary>
        public const string TASK = "_Task";

        /// <summary>
        /// Constant specify the task pane
        /// </summary>
        public const string TASKPANE = "taskPane";

        /// <summary>
        /// Constant specify the RSS title
        /// </summary>
        public const string RSSTITLE = "RSSTitle";

        /// <summary>
        /// Constant specify the RSS pane
        /// </summary>
        public const string RSSPANE = "rssPane";

        /// <summary>
        /// Constant specify the RSS feed
        /// </summary>
        public const string RSSFEED = "RSSFeed";

        /// <summary>
        /// Constant specify the OneNote
        /// </summary>
        public const string ONENOTE = "_OneNote";

        /// <summary>
        /// Constant specify the OneNote Pane
        /// </summary>
        public const string ONENOTEPANE = "oneNotePane";

        /// <summary>
        /// Constant specify the site pages
        /// </summary>
        public const string SITEPAGES = "site pages";

        /// <summary>
        /// Constant specify the page URL to re-index site collection
        /// </summary>
        public const string SITEREINDEXPAGE = "/_layouts/15/srchvis.aspx";

        /// <summary>
        /// Constant specify the sites
        /// </summary>
        public const string SITE = "Site Pages";

        /// <summary>
        /// Constant specify the folder of Project Center Assets
        /// </summary>
        public const string PROJECTCENTERASSETS = "Project Center Assets";

        /// <summary>
        /// Constant specify the Project landing assets folder
        /// </summary>
        public const string PROJECTLANDINGASSETS = "Project Landing Assets";

        /// <summary>
        /// Constant specify the Common Assets folder
        /// </summary>
        public const string COMMONASSETS = "Common Assets";

        /// <summary>
        /// Constant specify the project landing header web-part
        /// </summary>
        public const string MATTERCENTERHEADER = "matterCenterHeader";

        /// <summary>
        /// Constant specify the Project landing page CSS file
        /// </summary>
        public const string PROJECTLANDINGCSS = "ProjectLanding.css";

        /// <summary>
        /// Constant specify the common CSS file
        /// </summary>
        public const string COMMONCSS = "spCommon.css";

        /// <summary>
        /// Constant specify the sites
        /// </summary>
        public const string SITEPAGE = "SitePages";

        /// <summary>
        /// Constant specify the right zone
        /// </summary>
        public const string RIGHTZONE = "RightZone";

        /// <summary>
        /// Constant specify the underscore
        /// </summary>
        public const string UNDERSCORE = "_";

        /// <summary>
        /// Constant specify the pipe
        /// </summary>
        public const string PIPE = "|";

        /// <summary>
        /// Constant specify the dollar
        /// </summary>
        public const string DOLLAR = "$";

        /// <summary>
        /// Constant specify the front slash
        /// </summary>
        public const string FRONTSLASH = "/";

        /// <summary>
        /// Constant specify the back slash
        /// </summary>
        public const string BACKSLASH = "\\";

        /// <summary>
        /// Constant specify the lists directory
        /// </summary>
        public const string LISTS = "Lists/";

        /// <summary>
        /// Constant specify the tasks
        /// </summary>
        public const string TASKS = "Tasks";

        /// <summary>
        /// Constant specify the is
        /// </summary>
        public const string IS = "Is";

        /// <summary>
        /// Constant specify the present
        /// </summary>
        public const string PRESENT = "Present";

        /// <summary>
        /// Constant specify ASPX page extension
        /// </summary>
        public const string ASPX = ".aspx";

        /// <summary>
        /// Constant specify the project landing section contents
        /// </summary>
        public const string SECTIONCONTENT = "<div id=\"{0}\"></div>";

        /// <summary>
        /// Constant specify the false state
        /// </summary>
        public const string FALSESTATE = "False";

        /// <summary>
        /// Constant specify the project user emails
        /// </summary>
        public const string PROJECTCENTERUSEREMAILS = "ProjectCenterUserEmails";

        /// <summary>
        /// Constant specify the project permission
        /// </summary>
        public const string PROJECTCENTERPERMISSIONS = "ProjectCenterPermissions";

        /// <summary>
        /// Constant specify the dollar separator
        /// </summary>
        public const string DOLLARSEPERATOR = "$|$";

        /// <summary>
        /// Constant specify the semicolon
        /// </summary>
        public const string SEMICOLON = ";";

        public const string ContentEditorWebPart = @"<?xml version=""1.0"" encoding=""utf-8""?>
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
                      <Assembly>Microsoft.SharePoint, Version={1}, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Assembly>
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
        /// XML definition of the RssFeedWebpart web part
        /// </summary>
        public const string RssFeedWebpart = @"<webParts>
                                   <webPart xmlns=""http://schemas.microsoft.com/WebPart/v3"">
                                  <metaData>
                                  <type name=""Microsoft.SharePoint.Portal.WebControls.RSSAggregatorWebPart, Microsoft.SharePoint.Portal, Version={1}, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" />
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
                                    <property name=""ViewFlags"" type=""Microsoft.SharePoint.SPViewFlags, Microsoft.SharePoint, Version={1}, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">None</property>
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
                                    &lt;%@ Register TagPrefix=""WebControls"" Namespace=""Microsoft.SharePoint.WebControls"" Assembly=""Microsoft.SharePoint, Version={1}, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" %&gt;
                                    &lt;%@ Register TagPrefix=""WebPartPages"" Namespace=""Microsoft.SharePoint.WebPartPages"" Assembly=""Microsoft.SharePoint, Version={1}, Culture=neutral, PublicKeyToken=71e9bce111e9429c"" %&gt;
                                    &lt;WebControls:XmlUrlDataSource runat=""server"" AuthType=""None"" HttpMethod=""GET""&gt;
                                    &lt;DataFileParameters&gt;
                                    &lt;WebPartPages:DataFormParameter Name=""RequestUrl"" ParameterKey=""RequestUrl"" PropertyName=""ParameterValues""/&gt;
                                    &lt;/DataFileParameters&gt;
                                    &lt;/WebControls:XmlUrlDataSource&gt;</property>
                                    <property name=""DisplayName"" type=""string"" />
                                    <property name=""PageType"" type=""Microsoft.SharePoint.PAGETYPE, Microsoft.SharePoint, Version={1}, Culture=neutral, PublicKeyToken=71e9bce111e9429c"">PAGE_NORMALVIEW</property>
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
    }
}