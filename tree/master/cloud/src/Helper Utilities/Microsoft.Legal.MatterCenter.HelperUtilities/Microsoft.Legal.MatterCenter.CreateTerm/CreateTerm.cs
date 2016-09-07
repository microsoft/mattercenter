// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.CreateTerm
// Author           : v-diajme
// Created          : 06-19-2014
//
// ***********************************************************************
// <copyright file="CreateTerm.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file creates term store data.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.CreateTerm
{
    #region using
    using Microsoft.Legal.MatterCenter.Common;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.Taxonomy;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    #endregion

    internal class DataStorage
    {
        public string TermName { get; set; }
        public string PracticeGroup { get; set; }
        public string AreaofLaw { get; set; }
        public string SubAreaofLaw { get; set; }
        public string PGFolders { get; set; }
        public string AOLFolders { get; set; }
        public string SAOLFolders { get; set; }
        public string SAOLContentType { get; set; }
        public string SAOLDocumentTemplates { get; set; }
        public string SAOLIsFolderStructurePresent { get; set; }
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public string ClientURL { get; set; }
    }

    internal class CustomTermGroup
    {
        public string name;
        public List<CustomPracticeGroup> Pg;
    }

    internal class CustomPracticeGroup
    {
        public string name;
        public List<CustomAreaOfLaw> Aol;
        public Dictionary<string, string> properties;
    }

    internal class CustomAreaOfLaw
    {
        public string name;
        public List<CustomSubAreaOfLaw> Saol;
        public Dictionary<string, string> properties;
    }

    internal class CustomSubAreaOfLaw
    {
        public string name;
        public Dictionary<string, string> properties;
    }

    internal class CustomClientGroup
    {
        public string name;
        public List<ClientList> clt;
    }

    internal class ClientList
    {
        public string ClientName;
        public string ClientID;
        public string ClientURL;
    }

    internal class CreateTerm
    {
        public static string errorFilePath = Directory.GetParent(Directory.GetCurrentDirectory()) + ConfigurationManager.AppSettings["errorlog"];
        public static void Main(string[] args)
        {
            if (2 <= args.Length)
            {
                string username = args[1], password = args[2];
                bool action = Convert.ToBoolean(args[0], CultureInfo.InvariantCulture); // Flag to Create or Delete Term store Hierarchy
                if (!ExcelOperations.IsNullOrEmptyCredential(username, password))
                {
                    Console.WriteLine("Reading inputs from Excel...");
                    string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
                    string sheetName = ConfigurationManager.AppSettings["sheetname"];

                    Dictionary<string, string> listval = ExcelOperations.ReadFromExcel(filePath, sheetName);
                    listval.Add("Username", username);
                    listval.Add("Password", password);
                    if (args.Length != 0 && !string.IsNullOrWhiteSpace(args[0]))
                    {
                        ReadConfigExcel(listval, action);
                    }
                }
                else
                {
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: Invalid Credentials");
                }
            }
            else
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: Insufficient Parameters");
            }
        }
        /// <summary>
        /// Read configuration values from Excel and create term store hierarchy
        /// </summary>
        /// <param name="listval">Configuration values from configuration Excel</param>
        /// <param name="action">Flag to create or delete hierarchy</param>
        public static void ReadConfigExcel(Dictionary<string, string> listval, bool action)
        {
            try
            {
                string filePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\" + ConfigurationManager.AppSettings["filename"];
                string termSheetName = ConfigurationManager.AppSettings["termconfig"];
                string clientSheetName = ConfigurationManager.AppSettings["clientconfig"];
                if (System.IO.File.Exists(filePath))
                {
                    Collection<Collection<string>> termSheetValues = ExcelOperations.ReadSheet(filePath, termSheetName);
                    Collection<Collection<string>> clientSheetValues = ExcelOperations.ReadSheet(filePath, clientSheetName);
                    List<DataStorage> termList = ReadTermData(termSheetValues, "Practice_Group").ToList();
                    List<DataStorage> clientList = ReadTermData(clientSheetValues, "Client").ToList();
                    if (action)
                    {
                        CreateTermStructure(listval, termList);
                        CreateClientStructure(listval, clientList, action);
                    }
                    else
                    {
                        CreateClientStructure(listval, clientList, action);
                    }
                }
                else
                {
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Couldn't find file: " + filePath);
                }
            }
            catch (Exception exception)
            {

                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Create client structure in taxonomy
        /// </summary>
        /// <param name="listval">Configuration values from configuration Excel</param>
        /// <param name="clientList">List of Clients from configuration Excel</param>
        /// <param name="action">flag to create or delete client hierarchy</param>
        public static void CreateClientStructure(Dictionary<string, string> listval, List<DataStorage> clientList, bool action)
        {
            try
            {
                List<CustomClientGroup> clientsData = new List<CustomClientGroup>();
                foreach (DataStorage item in clientList)
                {
                    string termName = item.TermName;
                    CustomClientGroup termCurrent = (from term in clientsData
                                                     where term.name == termName
                                                     select term).FirstOrDefault();

                    if (null == termCurrent)
                    {
                        clientsData.Add(new CustomClientGroup() { name = termName, clt = new List<ClientList>() });
                    }
                    termCurrent = (from term in clientsData
                                   where term.name == termName
                                   select term).FirstOrDefault();
                    termName = item.ClientName;
                    ClientList cltCurrent = (from term in termCurrent.clt
                                             where term.ClientName == termName
                                             select term).FirstOrDefault();

                    if (null == cltCurrent)
                    {
                        termCurrent.clt.Add(new ClientList() { ClientID = item.ClientId, ClientName = item.ClientName, ClientURL = item.ClientURL });
                    }
                }
                if (action)
                {
                    CreateClientTerms(listval, clientsData);
                }
                else
                {
                    DeleteTerms(listval, clientsData);
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }

        }

        /// <summary>
        /// Create Term store structure
        /// </summary>
        /// <param name="listval">List of Clients from configuration Excel</param>
        /// <param name="termList">Terms list</param>
        public static void CreateTermStructure(Dictionary<string, string> listval, List<DataStorage> termList)
        {
            try
            {
                List<CustomTermGroup> groups = new List<CustomTermGroup>();
                foreach (DataStorage item in termList)
                {
                    string termName = item.TermName;
                    CustomTermGroup termCurrent = (from termGroup in groups
                                                   where termGroup.name == termName
                                                   select termGroup).FirstOrDefault();

                    if (null == termCurrent)
                    {
                        groups.Add(new CustomTermGroup() { name = termName, Pg = new List<CustomPracticeGroup>() });
                    }

                    termCurrent = (from termGroup in groups
                                   where termGroup.name == termName
                                   select termGroup).FirstOrDefault();


                    termName = item.PracticeGroup;
                    CustomPracticeGroup pgCurrent = (from termGroup in termCurrent.Pg
                                                     where termGroup.name == termName
                                                     select termGroup).FirstOrDefault();

                    if (null == pgCurrent)
                    {
                        Dictionary<string, string> customPGConfig = new Dictionary<string, string>();
                        if (!string.IsNullOrWhiteSpace(item.PGFolders))
                        {
                            customPGConfig.Add("PGFolders", item.PGFolders);
                        }
                        termCurrent.Pg.Add(new CustomPracticeGroup() { name = termName, Aol = new List<CustomAreaOfLaw>(), properties = customPGConfig });
                    }

                    pgCurrent = (from termGroup in termCurrent.Pg
                                 where termGroup.name == termName
                                 select termGroup).FirstOrDefault();


                    termName = item.AreaofLaw;
                    CustomAreaOfLaw aolCurrent = (from termGroup in pgCurrent.Aol
                                                  where termGroup.name == termName
                                                  select termGroup).FirstOrDefault();

                    if (null == aolCurrent)
                    {
                        Dictionary<string, string> customAOLConfig = new Dictionary<string, string>();
                        if (!string.IsNullOrWhiteSpace(item.AOLFolders))
                        {
                            customAOLConfig.Add("AOLFolders", item.AOLFolders);
                        }
                        pgCurrent.Aol.Add(new CustomAreaOfLaw() { name = termName, Saol = new List<CustomSubAreaOfLaw>(), properties = customAOLConfig });
                    }

                    aolCurrent = (from termGroup in pgCurrent.Aol
                                  where termGroup.name == termName
                                  select termGroup).FirstOrDefault();


                    termName = item.SubAreaofLaw;
                    CustomSubAreaOfLaw saolCurrent = (from term in aolCurrent.Saol
                                                      where term.name == termName
                                                      select term).FirstOrDefault();

                    if (null == saolCurrent)
                    {
                        Dictionary<string, string> customSAOLConfig = new Dictionary<string, string>();
                        if (!string.IsNullOrWhiteSpace(item.SAOLFolders))
                        {
                            customSAOLConfig.Add("SAOLFolders", item.SAOLFolders);
                        }
                        if (!string.IsNullOrWhiteSpace(item.SAOLContentType))
                        {
                            customSAOLConfig.Add("SAOLContentType", item.SAOLContentType);
                        }
                        if (!string.IsNullOrWhiteSpace(item.SAOLDocumentTemplates))
                        {
                            customSAOLConfig.Add("SAOLDocumentTemplate", item.SAOLDocumentTemplates);
                        }
                        if (!string.IsNullOrWhiteSpace(item.SAOLIsFolderStructurePresent))
                        {
                            customSAOLConfig.Add("SAOLIsFolder", item.SAOLIsFolderStructurePresent);
                        }

                        aolCurrent.Saol.Add(new CustomSubAreaOfLaw() { name = termName, properties = customSAOLConfig });
                    }

                    saolCurrent = (from a in aolCurrent.Saol
                                   where a.name == termName
                                   select a).FirstOrDefault();

                }
                CreateTerms(listval, groups);
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Function to create client terms
        /// </summary>
        /// <param name="listval">List of Clients from configuration Excel</param>
        /// <param name="TermsData">Terms data</param>
        public static void CreateClientTerms(Dictionary<string, string> listval, List<CustomClientGroup> TermsData)
        {
            try
            {
                string targetSite = listval["CatalogSiteURL"]; // Get the URL of site collection
                string login = listval["Username"]; // Get the user name
                string password = listval["Password"]; // Get the password

                using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(targetSite, login, password ))
                {
                    CreateTermsForClients(TermsData, clientContext);
                }

            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
        }

        /// <summary>
        /// Create client terms
        /// </summary>
        /// <param name="TermsData">Data for client terms</param>
        /// <param name="clientContext">Client Context object</param>
        private static void CreateTermsForClients(List<CustomClientGroup> TermsData, ClientContext clientContext)
        {
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            clientContext.Load(taxonomySession.TermStores);

            //Execute the query to the server
            clientContext.ExecuteQuery();
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            clientContext.Load(termStore.Groups);

            //Execute the query to the server
            clientContext.ExecuteQuery();
            foreach (CustomClientGroup cltGroup in TermsData)
            {
                TermGroup group = termStore.Groups.Where(termGroup => termGroup.Name == cltGroup.name).Count() > 0 ? termStore.Groups.GetByName(cltGroup.name) : termStore.CreateGroup(cltGroup.name, Guid.NewGuid());
                if (null != group)
                {
                    TermSet cltTermSet = group.CreateTermSet(ConfigurationManager.AppSettings["clientterm"], Guid.NewGuid(), 1033);
                    TermSet cltIDTermSet = group.CreateTermSet(ConfigurationManager.AppSettings["clientidterm"], Guid.NewGuid(), 1033);
                    foreach (ClientList clients in cltGroup.clt)
                    {
                        Term clientCustom = cltTermSet.CreateTerm(clients.ClientName, 1033, Guid.NewGuid());
                        cltIDTermSet.CreateTerm(clients.ClientID, 1033, Guid.NewGuid());
                        clientCustom.SetCustomProperty("ClientID", clients.ClientID);
                        clientCustom.SetCustomProperty("ClientURL", clients.ClientURL);
                    }
                }
            }
            termStore.CommitAll();
            clientContext.Load(termStore);
            //Execute the query to the server
            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Creates terms in term group
        /// </summary>
        /// <param name="listval">Configuration list values from configuration Excel </param>
        /// <param name="TermsData">Terms Data</param>
        public static void CreateTerms(Dictionary<string, string> listval, List<CustomTermGroup> TermsData)
        {
            string targetSite = listval["CatalogSiteURL"]; // Get the URL of site collection
            string login = listval["Username"]; // Get the user name
            string password = listval["Password"]; // Get the password

            using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(targetSite, login, password ))
            {
                try
                {
                    CreatePGTerms(TermsData, clientContext);
                }
                catch (Exception exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                }
            }
        }

        /// <summary>
        /// Create practice group terms
        /// </summary>
        /// <param name="TermsData">Practice group, SAOL and AOL data</param>
        /// <param name="clientContext">Client context object</param>
        private static void CreatePGTerms(List<CustomTermGroup> TermsData, ClientContext clientContext)
        {
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            clientContext.Load(taxonomySession.TermStores);

            //Execute the query to the server
            clientContext.ExecuteQuery();
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            clientContext.Load(termStore.Groups);

            //Execute the query to the server
            clientContext.ExecuteQuery();

            foreach (CustomTermGroup termGroup in TermsData)
            {
                TermGroup group = termStore.Groups.Where(groupProperties => groupProperties.Name == termGroup.name).Count() > 0 ? termStore.Groups.GetByName(termGroup.name) : termStore.CreateGroup(termGroup.name, Guid.NewGuid());
                TermSet pracGrp = group.CreateTermSet(ConfigurationManager.AppSettings["pgterm"], Guid.NewGuid(), 1033);
                foreach (CustomPracticeGroup pg in termGroup.Pg)
                {
                    if (!string.IsNullOrWhiteSpace(pg.name))
                    {
                        Console.WriteLine("Creating practice group " + pg.name);
                        Term pgTermSet = pracGrp.CreateTerm(pg.name, 1033, Guid.NewGuid());
                        if (pg.properties.ContainsKey("PGFolders") && !string.IsNullOrWhiteSpace(pg.properties["PGFolders"]))
                        {
                            pgTermSet.SetCustomProperty("FolderNames", pg.properties["PGFolders"]);
                        }
                        foreach (CustomAreaOfLaw aol in pg.Aol)
                        {
                            if (!string.IsNullOrWhiteSpace(aol.name))
                            {
                                Console.WriteLine("\tCreating area of law " + aol.name);
                                Term AOLTerm = pgTermSet.CreateTerm(aol.name, 1033, Guid.NewGuid());
                                if (aol.properties.ContainsKey("AOLFolders") && !string.IsNullOrWhiteSpace(aol.properties["AOLFolders"]))
                                {
                                    AOLTerm.SetCustomProperty("FolderNames", aol.properties["AOLFolders"]);
                                }
                                CustomArea(aol, AOLTerm);
                            }
                        }
                    }
                }
            }
            termStore.CommitAll();
            clientContext.Load(termStore);
            //Execute the query to the server
            clientContext.ExecuteQuery();
        }

        private static void CustomArea(CustomAreaOfLaw aol, Term AOLTerm)
        {
            foreach (CustomSubAreaOfLaw asol in aol.Saol)
            {
                Console.WriteLine("\t\tCreating sub area of law " + asol.name);
                Term ASOLTerm = AOLTerm.CreateTerm(asol.name, 1033, Guid.NewGuid());
                if (asol.properties.ContainsKey("SAOLFolders") && !string.IsNullOrWhiteSpace(asol.properties["SAOLFolders"]))
                {
                    ASOLTerm.SetCustomProperty("FolderNames", asol.properties["SAOLFolders"]);
                }
                if (asol.properties.ContainsKey("SAOLContentType") && !string.IsNullOrWhiteSpace(asol.properties["SAOLContentType"]))
                {
                    ASOLTerm.SetCustomProperty("ContentTypeName", asol.properties["SAOLContentType"]);
                }
                if (asol.properties.ContainsKey("SAOLDocumentTemplate") && !string.IsNullOrWhiteSpace(asol.properties["SAOLDocumentTemplate"]))
                {
                    ASOLTerm.SetCustomProperty("DocumentTemplates", asol.properties["SAOLDocumentTemplate"]);
                }
                if (asol.properties.ContainsKey("SAOLIsFolder") && !string.IsNullOrWhiteSpace(asol.properties["SAOLIsFolder"]))
                {
                    ASOLTerm.SetCustomProperty("isNoFolderStructurePresent", asol.properties["SAOLIsFolder"]);
                }
            }
        }

        /// <summary>
        /// Function is used to return objects formed using values read from Excel sheet
        /// </summary>
        /// <param name="sheetValues">List of values read from Excel sheet</param>
        /// <param name="termType">Term type</param>
        /// <returns>List of data storage objects</returns>
        public static Collection<DataStorage> ReadTermData(Collection<Collection<string>> sheetValues, string termType)
        {
            Collection<DataStorage> termsData = new Collection<DataStorage>();
            try
            {
                int iCount = 0;

                foreach (Collection<string> row in sheetValues)
                {
                    if (iCount == 0)
                    {
                        iCount = 1;
                        continue;
                    }
                    if (!string.IsNullOrWhiteSpace(termType) && termType.Equals("Practice_Group", StringComparison.OrdinalIgnoreCase))
                    {
                        string key = Convert.ToString(row[0], CultureInfo.InvariantCulture);
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            DataStorage TermInfoTuple = new DataStorage();
                            TermInfoTuple.TermName = ConfigurationManager.AppSettings["termName"];
                            TermInfoTuple.PracticeGroup = Convert.ToString(row[0], CultureInfo.InvariantCulture);
                            TermInfoTuple.AreaofLaw = Convert.ToString(row[1], CultureInfo.InvariantCulture);
                            TermInfoTuple.SubAreaofLaw = Convert.ToString(row[2], CultureInfo.InvariantCulture);
                            TermInfoTuple.PGFolders = Convert.ToString(row[3], CultureInfo.InvariantCulture);
                            TermInfoTuple.AOLFolders = Convert.ToString(row[4], CultureInfo.InvariantCulture);
                            TermInfoTuple.SAOLFolders = Convert.ToString(row[5], CultureInfo.InvariantCulture);
                            TermInfoTuple.SAOLContentType = Convert.ToString(row[6], CultureInfo.InvariantCulture);
                            TermInfoTuple.SAOLDocumentTemplates = Convert.ToString(row[7], CultureInfo.InvariantCulture);
                            TermInfoTuple.SAOLIsFolderStructurePresent = Convert.ToString(row[8], CultureInfo.InvariantCulture);
                            termsData.Add(TermInfoTuple);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(termType) && termType.Equals("Client", StringComparison.OrdinalIgnoreCase))
                    {
                        string key = Convert.ToString(row[0], CultureInfo.InvariantCulture);
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            DataStorage TermInfoTuple = new DataStorage();
                            TermInfoTuple.TermName = ConfigurationManager.AppSettings["termName"];
                            TermInfoTuple.ClientName = Convert.ToString(row[0], CultureInfo.InvariantCulture);
                            TermInfoTuple.ClientId = Convert.ToString(row[1], CultureInfo.InvariantCulture);
                            TermInfoTuple.ClientURL = Convert.ToString(row[2], CultureInfo.InvariantCulture);
                            termsData.Add(TermInfoTuple);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
            }
            return termsData;
        }

        /// <summary>
        /// Deletes term store hierarchy
        /// </summary>
        /// <param name="listval">Configuration values from Excel</param>
        /// <param name="TermsData">Terms Data</param>
        public static void DeleteTerms(Dictionary<string, string> listval, List<CustomClientGroup> TermsData)
        {
            string targetSite = listval["CatalogSiteURL"]; // Get the URL of site collection
            string login = listval["Username"]; // Get the user name
            string password = listval["Password"]; // Get the password

            using (ClientContext clientContext = ConfigureSharePointContext.ConfigureClientContext(targetSite, login, password ))
            {
                try
                {
                    Console.WriteLine("Deleting existing taxonomy hierarchy");
                    DeleteMatterCenterTerms(TermsData, clientContext);
                }
                catch (Exception exception)
                {
                    ErrorLogger.LogErrorToTextFile(errorFilePath, "Message: " + exception.Message + "\nStacktrace: " + exception.StackTrace);
                }
            }
        }

        /// <summary>
        /// Deletes terms from term stores
        /// </summary>
        /// <param name="TermsData">Data to be delete from term store</param>
        /// <param name="clientContext">Client context object</param>
        private static void DeleteMatterCenterTerms(List<CustomClientGroup> TermsData, ClientContext clientContext)
        {
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
            clientContext.Load(taxonomySession.TermStores);

            //Execute the query to the server
            clientContext.ExecuteQuery();
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            clientContext.Load(termStore.Groups);

            //Execute the query to the server
            clientContext.ExecuteQuery();
            foreach (CustomClientGroup cltGroup in TermsData)
            {
                TermGroup groupCurrent = termStore.Groups.Where(termGroup => termGroup.Name == cltGroup.name).Count() > 0 ? termStore.Groups.GetByName(cltGroup.name) : termStore.CreateGroup(cltGroup.name, Guid.NewGuid());
                clientContext.Load(groupCurrent.TermSets);

                //Execute the query to the server
                clientContext.ExecuteQuery();
                TermSet clientTerm = (from t in groupCurrent.TermSets
                                      where t.Name == ConfigurationManager.AppSettings["clientterm"]
                                      select t).FirstOrDefault();
                TermSet clientID = (from t in groupCurrent.TermSets
                                    where t.Name == ConfigurationManager.AppSettings["clientidterm"]
                                    select t).FirstOrDefault();
                TermSet PGTerm = (from t in groupCurrent.TermSets
                                  where t.Name == ConfigurationManager.AppSettings["pgterm"]
                                  select t).FirstOrDefault();

                if (clientTerm != null)
                {
                    Console.WriteLine("Deleting clients");
                    clientTerm.DeleteObject();
                }
                if (clientID != null)
                {
                    Console.WriteLine("Deleting client ids");
                    clientID.DeleteObject();
                }
                if (PGTerm != null)
                {
                    Console.WriteLine("Deleting practice groups");
                    PGTerm.DeleteObject();
                }
                groupCurrent.DeleteObject();
            }
            //Execute the query to the server
            clientContext.ExecuteQuery();
            Console.WriteLine("Existing taxonomy hierarchy deleted successfully");
        }
    }
}
