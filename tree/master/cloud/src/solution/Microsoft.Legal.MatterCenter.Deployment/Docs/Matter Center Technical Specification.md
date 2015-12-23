Matter Center for Office 365

Technical specifications and system architecture

Introduction
============

Microsoft’s Corporate, External and Legal Affairs (CELA) Technology and
Operations team has developed Matter Center for Office 365, a
productivity and collaboration solution built using the office app
development model, Azure and Sharepoint.

Since Matter Center is built on Office 365, it inherits a multitude of
benefits from the underlying productivity platform, spanning
enterprise-grade features within SharePoint, Exchange, OneDrive for
Business and more. Examples include self-service business intelligence,
compliance and information protection, eDiscovery, document and
knowledge management, advanced federated search and discovery, and tools
for enterprise management and control.

This document provides an overview of Matter Center’s underlying
technical specifications, system architecture, components, communication
processes, and system quality attributes. For definitions of any and all
acronyms used throughout this document, refer to the
[Glossary](#_Glossary).

System architecture diagram 
============================

This section includes a diagram of Matter Center’s system architecture.

**System architecture diagram**

![](media/image2.png)

Matter Center components 
=========================

The components of Matter Center are as follows:

  -------------------------------------------------------------------------------------------------------------------------------------------------------
  **Component name**         **Purpose**                                                                                    > **Execution environment**
  -------------------------- ---------------------------------------------------------------------------------------------- -----------------------------
  Outlook/Office app         -   Opens app in Outlook & Office (Word, Excel, PowerPoint)                                    > Client machine
                                                                                                                            
                                                                                                                            

  Website                    -   Provides UI elements for all apps                                                          > Windows Azure
                                                                                                                            
                                                                                                                            

  WCF Service                -   Receives and validates inputs from Website                                                 > Windows Azure
                                                                                                                            
                             -   Performs actual SharePoint operations                                                      
                                                                                                                            
                             -   Sends output to Website in JSON format                                                     
                                                                                                                            
                                                                                                                            

  SharePoint Online (O365)   -   This is the data store for entire application                                              > SharePoint Online
                                                                                                                            
                             -   It holds documents, permissions, configurations, taxonomy, search relate components etc.   
                                                                                                                            
                                                                                                                            

  SharePoint app             -   Authenticates & Authorizes user                                                            > SharePoint Online
                                                                                                                            
                             -   Provides the entry point for all apps (including Outlook and Word)                         
                                                                                                                            
                                                                                                                            
  -------------------------------------------------------------------------------------------------------------------------------------------------------

Component descriptions
----------------------

The components of Matter Center are described below:

  -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  **Component name **        **Description**
  -------------------------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Outlook/Office app         Application wrapper within Outlook and Word client programs that allows the user to access the SharePoint system from Outlook and Word

  Website                    The frontend component which is developed using HTML, CSS & JS. This component interacts with the service to performs an action based on user interaction with other elements. This website is also responsible to perform validations whether user have provided correct information to take the action. This layer calls service methods using AJAX JS calls.
                             
                             This single website serves SharePoint, Outlook and Office apps. This website is following responsive design using media queries.

  WCF Service                This is a traditional Windows Communication Foundation (WCF) service which forms the business layer of the application. This service authenticates the calls from the frontend, interacts with SharePoint, and passes the requested data back to the frontend for display. In order to interact with SharePoint, this service makes use of [SharePoint Client Server Object Model (CSOM)](https://msdn.microsoft.com/en-us/library/ff798388.aspx)

  SharePoint Online (O365)   This is the system where all information is stored. It holds all documents library, documents, metadata associated with each document, configuration list, permission, indexes and crawler information etc.

  Windows Azure              Hosts Website and WCF Service as two separate websites with different domains

  SharePoint Search API      This application depends on SharePoint Search crawl to return its data to avoid any space and performance constraints for large documents. This also enables Matter Center to efficiently make use of metadata tags and search on document content.

  SharePoint App             This is a [provider hosted app](https://msdn.microsoft.com/en-us/library/office/fp142381.aspx) used to authenticate and authorize user with SPO environment, we make use of this app. Each SharePoint app has an appredirect URL which check if current user is allowed to access the app or not. Along with this it also provides a set of SPO tokens. These tokens are used to impersonate current user and taken action on SPO on their behalf.
  -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Deployment view 
================

1.  <span id="_Toc415568143" class="anchor"><span id="_Toc415575730"
    class="anchor"><span id="_Toc415576287" class="anchor"><span
    id="_Toc415662994" class="anchor"><span id="_Toc418066933"
    class="anchor"><span id="_Toc418066967" class="anchor"><span
    id="_Toc418067001" class="anchor"><span id="_Toc418067035"
    class="anchor"><span id="_Toc418067069" class="anchor"><span
    id="_Toc418067102" class="anchor"><span id="_Toc419211919"
    class="anchor"><span id="_Toc419211951" class="anchor"><span
    id="_Toc419211983" class="anchor"><span id="_Toc419386164"
    class="anchor"><span id="_Toc419728007" class="anchor"><span
    id="_Toc419728241" class="anchor"><span id="_Toc437877247"
    class="anchor"><span id="_Toc437897874" class="anchor"><span
    id="_Toc437897920" class="anchor"><span id="_Toc437898184"
    class="anchor"><span id="_Toc437899630" class="anchor"><span
    id="_Toc437950307" class="anchor"><span id="_Toc437951095"
    class="anchor"><span id="_Toc438050896" class="anchor"><span
    id="_Toc438062993"
    class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

2.  <span id="_Toc418066934" class="anchor"><span id="_Toc418066968"
    class="anchor"><span id="_Toc418067002" class="anchor"><span
    id="_Toc418067036" class="anchor"><span id="_Toc418067070"
    class="anchor"><span id="_Toc418067103" class="anchor"><span
    id="_Toc419211920" class="anchor"><span id="_Toc419211952"
    class="anchor"><span id="_Toc419211984" class="anchor"><span
    id="_Toc419386165" class="anchor"><span id="_Toc419728008"
    class="anchor"><span id="_Toc419728242" class="anchor"><span
    id="_Toc437877248" class="anchor"><span id="_Toc437897875"
    class="anchor"><span id="_Toc437897921" class="anchor"><span
    id="_Toc437898185" class="anchor"><span id="_Toc437899631"
    class="anchor"><span id="_Toc437950308" class="anchor"><span
    id="_Toc437951096" class="anchor"><span id="_Toc438050897"
    class="anchor"><span id="_Toc438062994"
    class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

3.  <span id="_Toc418066935" class="anchor"><span id="_Toc418066969"
    class="anchor"><span id="_Toc418067003" class="anchor"><span
    id="_Toc418067037" class="anchor"><span id="_Toc418067071"
    class="anchor"><span id="_Toc418067104" class="anchor"><span
    id="_Toc419211921" class="anchor"><span id="_Toc419211953"
    class="anchor"><span id="_Toc419211985" class="anchor"><span
    id="_Toc419386166" class="anchor"><span id="_Toc419728009"
    class="anchor"><span id="_Toc419728243" class="anchor"><span
    id="_Toc437877249" class="anchor"><span id="_Toc437897876"
    class="anchor"><span id="_Toc437897922" class="anchor"><span
    id="_Toc437898186" class="anchor"><span id="_Toc437899632"
    class="anchor"><span id="_Toc437950309" class="anchor"><span
    id="_Toc437951097" class="anchor"><span id="_Toc438050898"
    class="anchor"><span id="_Toc438062995"
    class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

4.  <span id="_Toc418066936" class="anchor"><span id="_Toc418066970"
    class="anchor"><span id="_Toc418067004" class="anchor"><span
    id="_Toc418067038" class="anchor"><span id="_Toc418067072"
    class="anchor"><span id="_Toc418067105" class="anchor"><span
    id="_Toc419211922" class="anchor"><span id="_Toc419211954"
    class="anchor"><span id="_Toc419211986" class="anchor"><span
    id="_Toc419386167" class="anchor"><span id="_Toc419728010"
    class="anchor"><span id="_Toc419728244" class="anchor"><span
    id="_Toc437877250" class="anchor"><span id="_Toc437897877"
    class="anchor"><span id="_Toc437897923" class="anchor"><span
    id="_Toc437898187" class="anchor"><span id="_Toc437899633"
    class="anchor"><span id="_Toc437950310" class="anchor"><span
    id="_Toc437951098" class="anchor"><span id="_Toc438050899"
    class="anchor"><span id="_Toc438062996"
    class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

Communication flow diagram
--------------------------

The detailed communication flow diagram for Matter Center is as follows:

**Communication flow diagram**

![](media/image3.png)

1.  <span id="_Toc415568147" class="anchor"><span id="_Toc415575734"
    class="anchor"><span id="_Toc415576292" class="anchor"><span
    id="_Toc415662999" class="anchor"><span id="_Toc418066938"
    class="anchor"><span id="_Toc418066972" class="anchor"><span
    id="_Toc418067006" class="anchor"><span id="_Toc418067040"
    class="anchor"><span id="_Toc418067074" class="anchor"><span
    id="_Toc418067107" class="anchor"><span id="_Toc419211924"
    class="anchor"><span id="_Toc419211956" class="anchor"><span
    id="_Toc419211988" class="anchor"><span id="_Toc419386169"
    class="anchor"><span id="_Toc419728012" class="anchor"><span
    id="_Toc419728246" class="anchor"><span id="_Toc437877252"
    class="anchor"><span id="_Toc437897879" class="anchor"><span
    id="_Toc437897925" class="anchor"><span id="_Toc437898189"
    class="anchor"><span id="_Toc437899635" class="anchor"><span
    id="_Toc437950312" class="anchor"><span id="_Toc437951100"
    class="anchor"><span id="_Toc438050901" class="anchor"><span
    id="_Toc438062998"
    class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

Detailed communication flow for Matter Center 
----------------------------------------------

The communication flow for Matter Center follows these steps:

1.  A user navigates to the app page from Office/Outlook or the
    Web Client. The app checks if the user is already logged in; if yes,
    the app loads and allows the user to perform operations (see
    step 4).

2.  If the user is not logged in, the app redirects the user to
    SharePoint’s login page via the app redirect page
    for authentication.

3.  SharePoint asks the user to login and, in turn, returns the
    SPAppToken to the app. Using the SPAppToken, the app generates
    RefreshToken and saves it in cookies to specify that user has
    successfully logged in.

4.  Every user action performs calls to WCF service using this
    RefreshToken for SharePoint operations:

<!-- -->

a.  In case if the user is trying to upload mail/attachments, the Azure
    WCF service makes a calls to Exchange web service using the current
    attachments or email token to get current email or attachments in
    SOAP format.

b.  After getting current email/attachments from the Exchange service,
    the WCF service uploads that mail/attachments to SharePoint using
    RefreshToken and SPO CSOM.

<!-- -->

1.  For each SharePoint operation, RefreshToken passed to service by
    website is authenticated by SharePoint to get current
    user’s context.

2.  SharePoint then provides results to service for the
    operation performed.

3.  Service passes the results to the UI layer in JSON string format. In
    case of an error in the service layer, the error message is logged
    into Azure Cloud Table Storage and the user sees a user friendly
    error message on the UI layer.

4.  The app saves user entered data or selections on Matter creation
    process only, in case, if the app closes before completing all
    the steps. This data is stored in the user’s local storage.

**Note**: To ensure secure communication of data, Matter Center uses
HTTPS (Hypertext Transfer Protocol Secure) protocol across all
communication channels.

1.  <span id="_Toc415576294" class="anchor"><span id="_Toc415663001"
    class="anchor"><span id="_Toc418066940" class="anchor"><span
    id="_Toc418066974" class="anchor"><span id="_Toc418067008"
    class="anchor"><span id="_Toc418067042" class="anchor"><span
    id="_Toc418067076" class="anchor"><span id="_Toc418067109"
    class="anchor"><span id="_Toc419211926" class="anchor"><span
    id="_Toc419211958" class="anchor"><span id="_Toc419211990"
    class="anchor"><span id="_Toc419386171" class="anchor"><span
    id="_Toc419728014" class="anchor"><span id="_Toc419728248"
    class="anchor"><span id="_Toc437877254" class="anchor"><span
    id="_Toc437897881" class="anchor"><span id="_Toc437897927"
    class="anchor"><span id="_Toc437898191" class="anchor"><span
    id="_Toc437899637" class="anchor"><span id="_Toc437950314"
    class="anchor"><span id="_Toc437951102" class="anchor"><span
    id="_Toc438050903" class="anchor"><span id="_Toc438063000"
    class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

System attributes
=================

Merits and limitations 
-----------------------

Matter Center creates a site collection for each client for a particular
law firm. Each client can contain multiple legal matters. Each matter
creates a separate document library within the specific client site
collection. Along with this document library, Matter Center also creates
following for a matter:

-   OneNote library with an Empty OneNote file;

-   A Calendar list;

-   A Task list;

-   A Matter Details page which brings all of the above elements
    together on a single web page; and

-   Each Matters is created with unique GUID which is auto-generate by
    the system. This is different than the Matter ID which is currently
    manually entered by users by creating the matter.

The table below provides the merits and limitations of using a client as
a site collection and a matter as a document library. Further
information about these software limits can be viewed
[here](https://support.office.com/en-in/article/sharepoint-online-software-boundaries-and-limits-8f34ff47-b749-408b-abc0-b605e1f6d498).

  ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                **Client as site collection**                                                                                                                                                                                                                                                                                                                       **Matter as document library (with contiguous memory allocation)**
  ----------------------------- --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Major limitation              Each tenant can only support 2000 clients                                                                                                                                                                                                                                                                                                           The upper limit to the storage space is 1 TB for each site collection
                                                                                                                                                                                                                                                                                                                                                                                    
                                Therefore, the maximum number of clients that can be supported is 100,000, which will require a total of 50 tenancies                                                                                                                                                                                                                               Once 80% of the space is in use, a new site collection must be manually created as SharePoint online doesn’t provide an API to create site collections

  Content separation            High                                                                                                                                                                                                                                                                                                                                                High

  Ease of data access           It is easier to access data for a client as long as data resides in only one site collection                                                                                                                                                                                                                                                        There is no limit on the number of document libraries in a site, meaning that there is no limit on the number of matters a site collection can have, as long as it’s below 1 TB overall
                                                                                                                                                                                                                                                                                                                                                                                    
                                Complexity will quickly increase if the content spreads across multiple site collections and could become unmanageable                                                                                                                                                                                                                              Custom administrative policies should be followed; no new content should be created when usage reaches 80% or 90% of the maximum allowable limit, so as to allow content to expand

  Administration requirements   High                                                                                                                                                                                                                                                                                                                                                Medium

  Archival                      Unknown                                                                                                                                                                                                                                                                                                                                             Matter libraries can be archived with records management

  Can contain                   Anything in SharePoint such as reusable workflows, documents, custom lists, images etc.                                                                                                                                                                                                                                                             Limited availability
                                                                                                                                                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                                                                                                                                                    Can contain only documents, images etc.

  Permission assignment         Easier to assign at site collection level                                                                                                                                                                                                                                                                                                           Need to break permissions at document library level when created

  Calculations                  Size limitation:                                                                                                                                                                                                                                                                                                                                    Size limitations: 
                                                                                                                                                                                                                                                                                                                                                                                    
                                **Note:** This calculation does not take into account any space requirement for versioning and archival                                                                                                                                                                                                                                             Memory available/site collection on SharePoint Online = 1 TB 
                                                                                                                                                                                                                                                                                                                                                                                    
                                Approx. matters/client = 500,000 A                                                                                                                                                                                                                                                                                                                  Memory available/tenant on SharePoint Online = 25 TB  
                                                                                                                                                                                                                                                                                                                                                                                    
                                Number of sites each site collection can have = 2000 sub-sites/site collection B                                                                                                                                                                                                                                                                    Once 25 TB is used up, we will need to move onto a new tenancy
                                                                                                                                                                                                                                                                                                                                                                                    
                                Since each site collection can only hold 2000 matters, 250 site collections/clients are needed (500,000/2000) to accommodate all matters (From A & B) C                                                                                                                                                                                             
                                                                                                                                                                                                                                                                                                                                                                                    
                                Each tenant in SharePoint Online can only hold 500,000 site collections                                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                                                                                                                                                    
                                Since every client will need upward of 250 site collections to hold its matters (from C above), and because we can have only 2000 clients (500,000/250) in one tenant (each tenant = 500,000 matters) collection to hold its matters (from C above), we can have only 2000 clients (500,000/250) in one tenant (each tenant with 500,000 matters)   
                                                                                                                                                                                                                                                                                                                                                                                    
                                So, to hold 100,000 clients, 50 tenancies are required (100,000/2000)                                                                                                                                                                                                                                                                               
                                                                                                                                                                                                                                                                                                                                                                                    
                                Please note the total number of tenants is huge (2500)                                                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                                                                                    
                                This may or may not always be the case (so large) because a firm may not always have 100,000 active clients                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                                                                                    
                                The figure of 100,000 reflects both active and inactive clients                                                                                                                                                                                                                                                                                     
  ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Dependencies
------------

Matter Center requires the following to function appropriately:

  -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Object                                                                     **Dependencies**
  -------------------------------------------------------------------------- --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Term store                                                                 Term store is used for saving details about clients and details of hierarchy for Practice Groups, Area of Law and Sub Area of Law
                                                                             
                                                                             These are displayed as filters in search apps

  Search API                                                                 SharePoint’s Search API is leveraged to get indexed search results (libraries and documents)

  Managed properties                                                         Managed properties are used along with Search API to filter, sort and retrieve required results

  Site columns and content types                                             -   Site columns and content types are used when creating matters
                                                                             
                                                                             -   Site columns are added to content types so that custom properties can be associated to each document
                                                                             
                                                                             -   Content types are associated to matter libraries so as to categorize the uploaded documents
                                                                             
                                                                             

  SharePoint list                                                            We are currently using multiple list which are used by the application which contains configuration information

  Custom Permission groups on SharePoint                                     In order to get users permission, we are making use of a special SharePoint group. This group allows the SharePoint JSOM object to make remote calls from SharePoint web pages

  Full Control for SharePoint app on Entire Tenant                           The SharePoint App required full control on the entire tenant. This permission is required because we are creating SharePoint pages with the help of SharePoint app

  Read and Write permission for Exchange App in ItemRead and ItemEdit mode   Exchange app should have ItemRead permission so that Matter Center appear in the email. At the same time, it also requires ItemEdit permission so that Matter Center app can be added while users are composing emails as well

  Application Insights (if required)                                         This is a usage tracking tool available with Azure. You can use this tool to capture usage pattern for your application

  Redis Cache                                                                To reduce the amount of time to pull static content from SPO, we are making use of Azure Redis cache to store this information. Redis cache currently only holds data from term store & Help list

  Azure Table Storage                                                        This will hold all error log information in case if there are unexpected issues with the application
  -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

The below table identifies core Matter Center Lists:

  -------------------------------------------------------------------------------------------------------------------
  **Object**                        **Details**
  --------------------------------- ---------------------------------------------------------------------------------
  Matter Center role list           SharePoint list where details about roles are stored
                                    
                                    These roles are used while creating matters

  Matter Center matter list         SharePoint list where details about matters are stored
                                    
                                    These matter details are provided by user during creating matters
                                    
                                    The list is populated for storing details
                                    
                                    Otherwise, the list is not used anywhere in the app

  Pinned matter details             SharePoint list where details about pinned matters for each user are stored
                                    
                                    Find pinned matters, pin matter and unpin matter operations use this list

  Pinned document details           SharePoint list where details about pinned documents for each user are stored
                                    
                                    Find pinned documents, pin document and unpin document operations use this list

  Matter Configurations             SharePoint list where default settings of Matter Creation page is stored

  Matter Center help section list   SharePoint list where details about contextual help sections are stored.

  Matter Center help links          SharePoint list where details about contextual help links are stored
  -------------------------------------------------------------------------------------------------------------------

### Availability

**Windows Azure:** The availability of apps depends on the availability
of websites for UI and service. These websites will be unavailable for a
brief period when publishing is performed. You can even configure
multiple instances for your website and service on azure to reduce
downtime to your end users

**SharePoint Online** maintenance may also cause the apps to be
unavailable.

### Reliability

**Windows Azure:** The reliability of apps depends on the integrity of
data as handled by SharePoint. WCF service does not store any data
within it and hence, if down, can be restored by re-publishing.

### Safety

**Windows Azure:** Matter Center depends on Azure for secure the
communication channels and its content. Azure website and service does
not store any information Apps support default safety as provided by
SharePoint and/or Windows Azure.

### Maintainability

**Windows Azure:** The apps are maintainable as they follow three-layer
architecture with separation of presentation, service and utility layer.
Also, the SharePoint component will require minimum updates if
redeployed after changes.

### Storage requirement

**SharePoint Online:** The storage space for each site collection will
be dependent on the space allocated by the administrator. The max limit
for the site collection depends the Office 365 SKU and the storage of
quota allocated for the site collection. This is configurable and can be
extended depending on the storage available in the tenant.

**Windows Azure:** In our application we are using Azure Storage to
capture all error logs only. This logging will help in debugging errors
with apps in Outlook, Word and SharePoint.

In-app security
---------------

**Windows Azure:** Security in the app is handled primarily by
RefreshToken which is provided by SharePoint app. This RefreshToken is
retrieved from SharePoint upon successful login as part of response
headers. This Encrypted RefreshToken is then saved in secure cookies for
future app launches. This token is valid for ten hours. In order to
perform an action on SharePoint, we use RefreshToken to generate
AccessToken and then, using AccessToken, we create SharePoint’s context.

The RefreshToken is extracted from Response Headers using the
TokenHelper class. This class is added by default when you create
Provider Hosted SharePoint add-in in Visual Studio. Using the
TokenHelper class, we generate AccessToken from RefreshToken to
impersonate logged in user and perform a task on SharePoint.

Data within the app is displayed based on SharePoint’s context created
from AccessToken. This SharePoint context is user based and hence, if
the user has permission on that SharePoint object, he/she will able to
view that object (note that this holds true even when SharePoint’s
search API is used). Hence, the authorization within the apps is handled
by SharePoint’s default behavior.

**Encryption and decryption of RefreshToken for SharePoint online**

After RefreshToken is retrieved from SPAppToken, RefreshToken is
encrypted using an AES algorithm. The key used for the encryption is
stored in the app settings of the Azure website for the UI. RefreshToken
is exchange between UI and Service with each call to ensure the
authenticity of the request.

RefreshToken is decrypted in service before retrieving the access token
to generate SharePoint context. The key used for decryption is again
stored in the app settings of the Azure for the service. Please note,
this key has to be same as the one which is present in UI. This key
should be changed after a duration. To keep track of the old key used
for decryption, it is stored in the app settings of the Azure website
for the UI and service.

When the user reloads the page or switches from one app to another,
RefreshToken is checked whether it is encrypted using the new encryption
key. It will be encrypted with the new key if not already done. The new
encrypted RefreshToken is then stored in the cookie for future purposes.

In the service, RefreshToken is first decrypted with the new encryption
key. If the decryption fails, the service will try to decrypted it with
an old encryption key. In case if service is still not able to decrypt
the Token it will simply throw and error. RefreshToken will remain
encrypted with the old encryption key until the user reloads the page or
switches from one app to another.

3.  <span id="_Toc414964027" class="anchor"><span id="_Toc414964374"
    class="anchor"><span id="_Toc415564110" class="anchor"><span
    id="_Toc415568160" class="anchor"><span id="_Toc415575747"
    class="anchor"><span id="_Toc415576306" class="anchor"><span
    id="_Toc415663013" class="anchor"><span id="_Toc418066951"
    class="anchor"><span id="_Toc418066985" class="anchor"><span
    id="_Toc418067019" class="anchor"><span id="_Toc418067053"
    class="anchor"><span id="_Toc418067087" class="anchor"><span
    id="_Toc418067120" class="anchor"><span id="_Toc419211936"
    class="anchor"><span id="_Toc419211968" class="anchor"><span
    id="_Toc419212000" class="anchor"><span id="_Toc419386181"
    class="anchor"><span id="_Toc419728024" class="anchor"><span
    id="_Toc419728258" class="anchor"><span id="_Toc437877264"
    class="anchor"><span id="_Toc437897892" class="anchor"><span
    id="_Toc437897938" class="anchor"><span id="_Toc437898201"
    class="anchor"><span id="_Toc437899647" class="anchor"><span
    id="_Toc437950324" class="anchor"><span id="_Toc437951112"
    class="anchor"><span id="_Toc438050913" class="anchor"><span
    id="_Toc438063010"
    class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

    1.  <span id="_Toc415564111" class="anchor"><span id="_Toc415568161"
        class="anchor"><span id="_Toc415575748" class="anchor"><span
        id="_Toc415576307" class="anchor"><span id="_Toc415663014"
        class="anchor"><span id="_Toc418066952" class="anchor"><span
        id="_Toc418066986" class="anchor"><span id="_Toc418067020"
        class="anchor"><span id="_Toc418067054" class="anchor"><span
        id="_Toc418067088" class="anchor"><span id="_Toc418067121"
        class="anchor"><span id="_Toc419211937" class="anchor"><span
        id="_Toc419211969" class="anchor"><span id="_Toc419212001"
        class="anchor"><span id="_Toc419386182" class="anchor"><span
        id="_Toc419728025" class="anchor"><span id="_Toc419728259"
        class="anchor"><span id="_Toc437877265" class="anchor"><span
        id="_Toc437897893" class="anchor"><span id="_Toc437897939"
        class="anchor"><span id="_Toc437898202" class="anchor"><span
        id="_Toc437899648" class="anchor"><span id="_Toc437950325"
        class="anchor"><span id="_Toc437951113" class="anchor"><span
        id="_Toc438050914" class="anchor"><span id="_Toc438063011"
        class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

    2.  <span id="_Toc415564112" class="anchor"><span id="_Toc415568162"
        class="anchor"><span id="_Toc415575749" class="anchor"><span
        id="_Toc415576308" class="anchor"><span id="_Toc415663015"
        class="anchor"><span id="_Toc418066953" class="anchor"><span
        id="_Toc418066987" class="anchor"><span id="_Toc418067021"
        class="anchor"><span id="_Toc418067055" class="anchor"><span
        id="_Toc418067089" class="anchor"><span id="_Toc418067122"
        class="anchor"><span id="_Toc419211938" class="anchor"><span
        id="_Toc419211970" class="anchor"><span id="_Toc419212002"
        class="anchor"><span id="_Toc419386183" class="anchor"><span
        id="_Toc419728026" class="anchor"><span id="_Toc419728260"
        class="anchor"><span id="_Toc437877266" class="anchor"><span
        id="_Toc437897894" class="anchor"><span id="_Toc437897940"
        class="anchor"><span id="_Toc437898203" class="anchor"><span
        id="_Toc437899649" class="anchor"><span id="_Toc437950326"
        class="anchor"><span id="_Toc437951114" class="anchor"><span
        id="_Toc438050915" class="anchor"><span id="_Toc438063012"
        class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

    3.  <span id="_Toc415564113" class="anchor"><span id="_Toc415568163"
        class="anchor"><span id="_Toc415575750" class="anchor"><span
        id="_Toc415576309" class="anchor"><span id="_Toc415663016"
        class="anchor"><span id="_Toc418066954" class="anchor"><span
        id="_Toc418066988" class="anchor"><span id="_Toc418067022"
        class="anchor"><span id="_Toc418067056" class="anchor"><span
        id="_Toc418067090" class="anchor"><span id="_Toc418067123"
        class="anchor"><span id="_Toc419211939" class="anchor"><span
        id="_Toc419211971" class="anchor"><span id="_Toc419212003"
        class="anchor"><span id="_Toc419386184" class="anchor"><span
        id="_Toc419728027" class="anchor"><span id="_Toc419728261"
        class="anchor"><span id="_Toc437877267" class="anchor"><span
        id="_Toc437897895" class="anchor"><span id="_Toc437897941"
        class="anchor"><span id="_Toc437898204" class="anchor"><span
        id="_Toc437899650" class="anchor"><span id="_Toc437950327"
        class="anchor"><span id="_Toc437951115" class="anchor"><span
        id="_Toc438050916" class="anchor"><span id="_Toc438063013"
        class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

    4.  <span id="_Toc415564114" class="anchor"><span id="_Toc415568164"
        class="anchor"><span id="_Toc415575751" class="anchor"><span
        id="_Toc415576310" class="anchor"><span id="_Toc415663017"
        class="anchor"><span id="_Toc418066955" class="anchor"><span
        id="_Toc418066989" class="anchor"><span id="_Toc418067023"
        class="anchor"><span id="_Toc418067057" class="anchor"><span
        id="_Toc418067091" class="anchor"><span id="_Toc418067124"
        class="anchor"><span id="_Toc419211940" class="anchor"><span
        id="_Toc419211972" class="anchor"><span id="_Toc419212004"
        class="anchor"><span id="_Toc419386185" class="anchor"><span
        id="_Toc419728028" class="anchor"><span id="_Toc419728262"
        class="anchor"><span id="_Toc437877268" class="anchor"><span
        id="_Toc437897896" class="anchor"><span id="_Toc437897942"
        class="anchor"><span id="_Toc437898205" class="anchor"><span
        id="_Toc437899651" class="anchor"><span id="_Toc437950328"
        class="anchor"><span id="_Toc437951116" class="anchor"><span
        id="_Toc438050917" class="anchor"><span id="_Toc438063014"
        class="anchor"></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span></span>

### Confidentiality 

Matters can be open (accessible to everyone) or secure/confidential
(accessible to only selected individuals (due to conflict of interest or
sensitivity of information)). Office 365 currently does not support
explicit deny (i.e. the mechanism to specifically block an individual’s
access to a document or matter, irrespective of their role in Office
365). As a work around to maintain the confidentiality of matters and
its document, Matter Center explicitly grants permissions only to users
assigned as part of the matter team during the matter creation process.

To elaborate on more on this point, consider an example where a law firm
is working on a matter with the client **Northwind Traders**. John Doe,
Jane Doe and Sara Davis are the only members allowed to work with
Northwind Trader’s client.

If an open matter is created for Northwind Traders, John, Jane and Sara
all will be able to view everything associated with this matter because
permissions will have been inherited from the client site collection.

If the matter is determined confidential, and only John and Jane are
assigned to the team, then Matter Centers breaks the permission to all
matter components (document library, page, calendar, OneNote library)
and grants permissions to only John and Jane. However, tenant and site
collection administrators will still be able to view all the information
related to the Northwind Traders matter.

In addition to above, each matter created via Matter Center, is created
using a system generate GUID. For e.g. the document library created for
the matter is created using this GUID. This GUID is generated using
ASP.NET GUID class. This GUID is different from Matter ID which should
be manually entered by users. This implementation will later allow to
easily integrate with automated Matter ID generation systems as well.
This same GUID is used for other Matter Components as well like,
calendar, OneNote library, task list and Matter Details Pages as well.

### Integrity

All legal documents present in each matter will be available and
editable for only those users who have been provided explicit access
when the matter was created.

### User groups

In order to access the app two SharePoint groups are required to provide
appropriate permissions. The details for these groups are as follows:

1.  **Matter Center users** – These are the users who will be accessing
    the Matter Center apps, but will have no access to the create
    matter app. Users in this group will have read permissions on the
    app catalog and will have contribute permissions on the
    configuration lists used by the Matter Center apps.

<!-- -->

1.  **Provision matter users** – These are the users who will have
    access to the Matter Center create matter app. Users in this group
    will have read permissions on the app catalog.

List structure
==============

As described in above sections, Matter Center makes use of multiple
configuration list which help with functionality of Matter Center. This
section describes purpose of each list which is used by Matter Center.
All lists mentioned in this section are available under you App Catalog
site collection.

MatterCenterRole
----------------

This list contains all roles currently belonging within the law firm.
These roles are used on the matter creation process. The user must
select specific role and identify users for those roles which assigning
team for a matter.

Following are default roles in Matter Center’s.

  **Role **
  ----------------------
  Responsible Attorney
  Paralegal
  Working Attorney
  Legal Admin
  Attorney

A user role of Responsible Attorney is mandatory while creating a new
matter. Each role can be assigned specific permissions and, based on
these permissions, the user can access specific actions.

Depending on the business need, the roles are specified as mandatory or
non-mandatory.

When a matter is being created, at least one user must be assigned to
each of the mandatory roles. By default, we have set Responsible
Attorney as the mandatory role.

Following is the list structure of this list:

  **Column name**   **Data type**         **Description**
  ----------------- --------------------- --------------------------------------------------------------------
  Role name         Single line of text   Name of the role. This columns is used in the Matter Creation page
  Mandatory         Yes/no                Flag to specify if the role is a mandatory role

MatterCenterMatters
-------------------

This list holds all Matters which are created by the app. Only purpose
of this list is to help during debugging in case if any Matters goes
missing. A new item is always added in this list when a matter is
successfully created

Following is the list structure for this list:

  **Column name**       **Data type**         **Description**
  --------------------- --------------------- -----------------------------------------------------------------------------------------------------------------------------
  Title                 Single line of text   This column holds client name and matter name
  ClientName            Single line of text   This column holds the client name for which matter is created which is selected in the matter creation form
  ClientID              Single line of text   This column holds the client ID for which the matter is created which is automatically selected in the matter creation form
  MatterName            Single line of text   This column holds the Matter Name which is entered by user in the matter creation form
  MatterID              Single line of text   This column holds the Matter ID which is manually entered by user in the matter creation form
  ConflictConductedBy   Person or Group       This column holds who performed conflict check for this matter which is entered by user in the matter creation form
  ConflictCheckOn       DateTime              This column holds when the conflict check was performed which is entered by user in the matter creation form
  ConflictIdentified    Yes/No                This column identifies whether conflict was found for this matter which is manually selected by user
  BlockUser             Person or Group       This column hold name of the individual who have conflict of interest and are blocked from accessing this matter
  ManagingAttorney      Person or Group       This column holds name of users who are identified in Responsible Attorney role
  Support               Person or Group       This column holds name of users who are identified in role other than Responsible Attorney

MatterCenterHelpSection
-----------------------

This list is used in the in solution help for creating sections. Using
this list, you can configure different sections on different page. Using
this you can also configure how the section should be shown in the help.

Following contains list structure of this list:

  **Column name**   **Data type**         **Description**
  ----------------- --------------------- ------------------------------------------------------------------------------------------------------
  Section ID        Single line of text   This column holds unique Section ID which help identify which section is this
  Section Title     Single line of text   This column holds the title of the section
  PageName          Choice                This column hold pre-configured list of pages present in the app
  Section Order     Number                This column allows you to determine the order in which sections should show on the page
  NumberofColumn    Choice                This column determines who many columns the section should span. It contains values as either 1 or 2

MatterCenterHelpLinks
---------------------

This list is used for creating Help link within each section within the
in-solution help. Following is the list structure:

  **Column name**   **Data type**         **Description**
  ----------------- --------------------- --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  LinkTitle         Single line of text   This column holds the link title which will be shown under a section
  LinkOrder         Number                This column allow you to determine the order in which links will appear within each section
  LinkURL           Hyperlink             This column contains the URL to which user will redirect on clicking this link
  SectionID         Lookup                This column is a look up field to the Section ID field in MatterCenterHelpSection list. This will allow to create the mapping between which links should appear under which sections

UserPinnedMatter
----------------

This list is used to hold all pinned matters by users. This list is
security trimmed so that if users gets access to this list, they can
only view content associated with their alias only. Tenant and catalog
admin will still be able to view content for all users in this list. For
each user there will always be single line item in the list

Following is the list structure:

  **Column name**   **Data type**           **Description**
  ----------------- ----------------------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  UserAlias         Single line of text     This column holds the user’s alias who has pinned matters. This alias is also used to only show data associated with logged in user in case if they directly landing on this list
  UserPinDetails    Multiple line of text   This column holds list of all matters which user has pinned in a JSON format. This format allows us to quickly show pinned information. In this JSON structure we also store some metadata of the matter as well which is required to be displayed in Pinned section

Following is the JSON structure on how the information is stored in
UserPinDetails column:

{ "Doc library url": {

"MatterName": "MatterName",

"MatterDescription": "MatterDescription",

"MatterCreatedDate": "CreatedDateTile",

"MatterUrl": "document library url",

"MatterPracticeGroup": "semi colon separated list of practice groups",

"MatterAreaOfLaw": "semi color separated list of area of law",

"MatterSubAreaOfLaw": "semi color separated list of sub area of law",

"MatterClientUrl": "Client Site collection URL",

"MatterClient": "Client Name",

"MatterClientId": "Client ID",

"HideUpload": "Does user have upload file permission",

"MatterID": "Matter ID",

"MatterResponsibleAttorney": "semi color separated list of Responsible
Attorney Name",

"MatterModifiedDate": "Last Modified Date of the matter" } ,

"Doc library url": {

"MatterName": "MatterName",

"MatterDescription": "MatterDescription",

"MatterCreatedDate": "CreatedDateTile",

"MatterUrl": "document library url",

"MatterPracticeGroup": "semi colon separated list of practice groups",

"MatterAreaOfLaw": "semi color separated list of area of law",

"MatterSubAreaOfLaw": "semi color separated list of sub area of law",

"MatterClientUrl": "Client Site collection URL",

"MatterClient": "Client Name",

"MatterClientId": "Client ID",

"HideUpload": "Does user have upload file permission",

"MatterID": "Matter ID",

"MatterResponsibleAttorney": "semi color separated list of Responsible
Attorney Name",

"MatterModifiedDate": "Last Modified Date of the matter" }

}

UserPinnedDetails
-----------------

This list is used to hold all pinned documents by users. This list is
security trimmed so that if users gets access to this list, they can
only view content associated with their alias only. Tenant and catalog
admin will still be able to view content for all users in this list. For
each user there will always be single line item in the list

Following is the list structure of this list:

  **Column name**          **Data type**           **Description**
  ------------------------ ----------------------- --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  UserAlias                Single line of text     This column holds the user’s alias who has pinned documents. This alias is also used to only show data associated with logged in user in case if they directly landing on this list
  UserPinDocumentDetails   Multiple line of text   This column holds list of all documents which user has pinned in a JSON format. This format allows us to quickly show pinned information. In this JSON structure we also store some metadata of the document as well which is required to be displayed in Pinned section

Following is the JSON structure on how the information is stored in
UserPinDocumentDetails site column:

{

"Document URL": {

"DocumentName": "Document Name",

"DocumentVersion": "Document Verison",

"DocumentClient": "client Name",

"DocumentClientId": "client id",

"DocumentClientUrl": "Client Site collection URL",

"DocumentMatter": "Matter Name",

"DocumentMatterId": "Matter ID",

"DocumentOwner": "Document Owner",

"DocumentUrl": "Document URL",

"DocumentOWAUrl": "Document Office Web App URL in case if this an Office
document",

"DocumentExtension": "Document Extension",

"DocumentCreatedDate": "Created Date",

"DocumentModifiedDate": "Modified Date",

"DocumentCheckoutUser": "Document Checked out to",

"DocumentMatterUrl": "Matter Document library URL",

"DocumentParentUrl": "Absolute URL to Parent folder of this document",

"DocumentID": "Document ID generated by Document ID service"

},

"Document URL": {

"DocumentName": "Document Name",

"DocumentVersion": "Document Verison",

"DocumentClient": "client Name",

"DocumentClientId": "client id",

"DocumentClientUrl": "Client Site collection URL",

"DocumentMatter": "Matter Name",

"DocumentMatterId": "Matter ID",

"DocumentOwner": "Document Owner",

"DocumentUrl": "Document URL",

"DocumentOWAUrl": "Document Office Web App URL in case if this an Office
document",

"DocumentExtension": "Document Extension",

"DocumentCreatedDate": "Created Date",

"DocumentModifiedDate": "Modified Date",

"DocumentCheckoutUser": "Document Checked out to",

"DocumentMatterUrl": "Matter Document library URL",

"DocumentParentUrl": "Absolute URL to Parent folder of this document",

"DocumentID": "Document ID generated by Document ID service"

}

}

Matter Configurations
---------------------

In matter center we have a Settings page which allows you to configure
default values for a client for Matter Creation process. These default
values are stored in this list which are later used by Matter Creation
process to pre-populate default values. In the future any site
collection specific settings will be stored in this list itself. This
list should be present under each client site collection.

Following is the list structure:

  **Column name**      **Data type**           **Description**
  -------------------- ----------------------- ---------------------------------------------------------------------------------------
  Title                Single line of text     This column holds the static values as Matter Configurations
  ConfigurationValue   Multiple line of text   This column holds the default values for the Matter creation process in a JSON format

<span id="_Glossary" class="anchor"><span id="_Toc438063026" class="anchor"></span></span>Glossary 
===================================================================================================

This section identifies any and all acronyms used throughout this
document for the reader’s reference.

  **Acronym**          **Identification**
  -------------------- -------------------------------------------
  AES                  Advanced Encryption Standard
  API                  Application Programming Interfaces
  CSOM (diagram 5.1)   Client-Side Object Model
  DMS                  Document Management System
  HTTPS                Hypertext Transfer Protocol Secure
  IIS                  Internet Information Services
  IT                   Information Technology
  JSON                 JavaScript Object Notation
  MMS Store            Managed Metadata Store (Term Store)
  OAuth tokens         Open Authorization tokens
  OOB                  Out of the Box
  SKU                  Stock Keeping Unit
  SOAP                 Simple Object Access Protocol
  SPApp Token          SharePoint App Token
  TB                   Terabyte
  UI                   User Interface
  WCF Service          Windows Communication Foundation Services


