// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.ProviderService
// Author           : v-shpate
// Created          : 11-28-2014
//
// ***********************************************************************
// <copyright file="WebpartConstants.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file contains constants required for creating web part in SharePoint.</summary>
// ***********************************************************************


namespace Microsoft.Legal.MatterCenter.ProviderService
{
    #region using
    using Microsoft.Legal.MatterCenter.Entity;
    using Microsoft.Legal.MatterCenter.Utility;
    #endregion

    internal static class WebpartConstants
    {
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
        /// XML definition of the RSS feed web part
        /// </summary>
        internal const string RssFeedWebpart = @"<webParts>
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
        /// XML definition of the list view web part
        /// </summary>
        internal const string ListviewWebpart = @"
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
                                <property name=""Width"" type=""string"" >750px</property>
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
        /// Matter Landing page sections
        /// </summary>
        internal static string MatterLandingPageSections = ConstantStrings.GetConfigurationFromResourceFile("Constants", "Matter_Landing_Page_Sections", Enumerators.ResourceFileLocation.App_GlobalResources);

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
        internal const string matterLandingStampProperties = "var documentLibraryName = \"{0}\", isNewMatterLandingPage = true, documentLibraryGUID=\"{1}\";";
    }
}