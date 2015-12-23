Matter Center for Office 365

Build and deployment guide

Matter Center deployment 
=========================

Pre-requisites
--------------

**Office 365 and Azure requirements**

  ----------------------------------------------------------------------------------------------------------------------------------------------------------------------
  **Infrastructure**                   **Service Accounts**   **Comments**
  ------------------------------------ ---------------------- ----------------------------------------------------------------------------------------------------------
  Plan                                 -                      Purchase Office 365 Enterprise E4 plan:
                                                              
                                                              <http://office.microsoft.com/en-us/business/office-365-enterprise-e4-business-software-FX104034562.aspx>
                                                              
                                                              Other plans:
                                                              
                                                              <http://office.microsoft.com/en-us/business/compare-all-office-365-for-business-plans-FX104051403.aspx>

  Domain                               Admin account          Domain for O365 portal
                                                              
                                                              If you already have the domain that will work with Office 365, please add same through admin portal
                                                              
                                                              Otherwise Purchase and register domain

  SSL certificate                      -                      Purchase and configure trusted authority certificate for domain

  Active Directory Integration         Admin account          Setup and synchronize existing Organization Active Directory on O365 portal
                                                              
                                                              <http://technet.microsoft.com/en-us/library/hh967642>

  Exchange Online                      -                      Configure Exchange 2013 with latest service packs

  Users                                Admin account          Configure users and assign licenses (SharePoint Online, Exchange Online, etc.) to each user

  Azure Subscription                                          Purchase Azure subscription which will be used to host 2 web sites
                                                              
                                                              UI layer
                                                              
                                                              Service layer
                                                              
                                                              And storage account to maintain error logs in table storage

  Domain and certificates (optional)   -                      Domain & certificate for UI layer (website on azure)
                                                              
                                                              Purchase and configure domain with website on Azure
                                                              
                                                              Purchase and configure trusted authority certificate for domain

  Azure websites                       Admin account          Provision 2 Websites for UI and Service component
                                                              
  And storage account                                         Websites should be configured based on options given in below table (Azure Configurations)
                                                              
                                                              Website for UI should also be configured with Domain and SSL certificate
                                                              
                                                              Provision storage account
  ----------------------------------------------------------------------------------------------------------------------------------------------------------------------

**Azure configurations**

  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  **Azure feature**    **Configuration**                                                                                                                                                                                                                                                                                                                                                                                                      **Details**
  -------------------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Scalability          Mode – Standard                                                                                                                                                                                                                                                                                                                                                                                                        Web Sites Standard (Promotional Pricing): The Standard tier offers multiple instance sizes as well as scaling to meet changing capacity needs. Prices for Standard are as follows:
                                                                                                                                                                                                                                                                                                                                                                                                                                              
                       Instances – A single instance in Shared or Standard mode already benefits from high availability, but you can provide even greater throughput and fault tolerance by running additional web site instances. In Standard mode, you can choose from 1 through 10 instances, and if you enable the Auto scale feature, you can set the minimum and maximum number of virtual machines to be used for automatic scaling.     --------------------------------------------------
                                                                                                                                                                                                                                                                                                                                                                                                                                                SIZE     CPU CORES   MEMORY    PRICE PER HOUR
                       <http://www.windowsazure.com/en-us/documentation/articles/web-sites-scale/>                                                                                                                                                                                                                                                                                                                                              -------- ----------- --------- -------------------
                                                                                                                                                                                                                                                                                                                                                                                                                                                Small    1           1.75 GB   \$0.10\
                                                                                                                                                                                                                                                                                                                                                                                                                                                                               (\~\$74 / month)
                                                                                                                                                                                                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                                                                                                                                                Medium   2           3.5 GB    \$0.20\
                                                                                                                                                                                                                                                                                                                                                                                                                                                                               (\~\$149 / month)
                                                                                                                                                                                                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                                                                                                                                                Large    4           7 GB      \$0.40\
                                                                                                                                                                                                                                                                                                                                                                                                                                                                               (\~\$298 / month)
                                                                                                                                                                                                                                                                                                                                                                                                                                                --------------------------------------------------
                                                                                                                                                                                                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                                                                                                                                              **Note**: Refer to the below link to know more about the pricing models: <http://www.windowsazure.com/en-us/pricing/details/web-sites/>[](http://www.windowsazure.com/en-us/documentation/articles/web-sites-scale/)

  Backups (Optional)   Use the services in the Recovery Services category to protect your data and clouds.                                                                                                                                                                                                                                                                                                                                      COMPRESSED DATA STORED PER MONTH   PRICE
                                                                                                                                                                                                                                                                                                                                                                                                                                                ---------------------------------- -------------------------
                                                                                                                                                                                                                                                                                                                                                                                                                                                First 5 GB / Month 1               Free
                                                                                                                                                                                                                                                                                                                                                                                                                                                Greater than 5 GB / Month          \$0.50 per GB per month
                                                                                                                                                                                                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                                                                                                                                              
  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

**Configurations**

  ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  **Infrastructure**   **Service Accounts**                                                                                                                    **Comments**                                                                                                                                        **Client**
  -------------------- --------------------------------------------------------------------------------------------------------------------------------------- --------------------------------------------------------------------------------------------------------------------------------------------------- ----------------------------
  Office 365 tenant    Service account with admin rights to be able to:                                                                                        Following things should be available or created manually:                                                                                           Outlook 2013, Office 2013,
                                                                                                                                                                                                                                                                                                                   
                       1.  Setup App catalog                                                                                                                   1.  App Catalog                                                                                                                                     Exchange 2013,
                                                                                                                                                                                                                                                                                                                   
                       2.  Add and install SP apps in app catalog                                                                                                  **Note:** Steps to create App catalog is present in Step 2 of deployment steps                                                                  IE 10+
                                                                                                                                                                                                                                                                                                                   
                       3.  Add Office apps in app catalog                                                                                                      2.  Organizational metadata hierarchy (Client detail, Practice group & hierarchy)                                                                   
                                                                                                                                                                                                                                                                                                                   
                       4.  Add mail apps at Organization\\Individual level in Exchange Online                                                                  > **Note**: Corresponding term set groups should accordingly be updated to Excel config file                                                        
                                                                                                                                                                                                                                                                                                                   
                       5.  Admin access on term store to add Organizational metadata                                                                           1.  Client site collections and updating clients taxonomy details                                                                                   
                                                                                                                                                                                                                                                                                                                   
                       6.  Add Crawled\\Managed properties                                                                                                     2.  Adding Content types, publishing same and                                                                                                       
                                                                                                                                                                                                                                                                                                                   
                       7.  Create site collections for client                                                                                                      updating corresponding taxonomy details                                                                                                         
                                                                                                                                                                                                                                                                                                                   
                       8.  Create config lists in catalog site                                                                                                 Also Exchange service account details should be added to Matter Center Deployment Configuration Excel                                               
                                                                                                                                                                                                                                                                                                                   
                       9.  Setup content type hub                                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                   
                       Exchange service account that will be used to generate mail on the fly for mail cart functionality                                                                                                                                                                                          
                                                                                                                                                                                                                                                                                                                   
                       **Note: **                                                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                   
                       a.  Service account represents a single account to be used throughout the process                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                   
                       b.  Whenever a new client is added to the system, you can follow the steps mentioned in Appendix A for properly setting up the client                                                                                                                                                       
                                                                                                                                                                                                                                                                                                                   
                                                                                                                                                                                                                                                                                                                   

  Azure Subscription   1 account with admin rights to deploy and maintain build. This same account should have Admin permission on 0365 tenancy                **Azure**:                                                                                                                                          
                                                                                                                                                                                                                                                                                                                   
                                                                                                                                                               This subscription will be used to deploy Web site, Web service and maintain error logs. Hence we will require a subscription which has following:   
                                                                                                                                                                                                                                                                                                                   
                                                                                                                                                               a.  Web role                                                                                                                                        
                                                                                                                                                                                                                                                                                                                   
                                                                                                                                                               b.  Azure Storage                                                                                                                                   
                                                                                                                                                                                                                                                                                                                   
                                                                                                                                                                                                                                                                                                                   
  ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

**Other requirements:**

1.  For mail cart functionality on Web Dashboard, we will require a
    service account to generate the e-mail with attachments on the fly
    and to ensure that the mail will be deleted from drafts once sent

2.  **Note**: If there is a change in password of account used, you can
    follow steps mentioned in Appendix B to update it for Matter Center

3.  For a brand new Office 365 tenant subscription, the account being
    used for deployment needs admin access to the Term Store in the
    SharePoint admin center

4.  Below are the pre-requistes for running the scripts on the user’
    machine:

-   [SharePoint Online Management
    > Shell](http://www.microsoft.com/en-us/download/details.aspx?id=35588)

-   [Windows
    > PowerShell](http://www.microsoft.com/en-in/download/details.aspx?id=2560)

-   [Azure PowerShell](http://go.microsoft.com/?linkid=9811175)

-   [Visual Studio 2015 (Community or Pro
    > and above)](https://www.visualstudio.com/en-us/products/vs-2015-product-editions.aspx)

-   Office Developer Tools for VS 2015

**Note**: Sometimes the scripts are not allowed to execute on the
machine, we can follow below steps to allow execution of the script:

-   Open PowerShell.exe in administrator mode

-   Type the following command: Set-ExecutionPolicy “RemoteSigned”, it
    > will ask for confirmation then type “Y” for yes.

Deployment steps 
-----------------

  -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  > **Deployment Steps/Description**                                                                                                                                                                                                                                                                                       **Automated/Manual**
  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ ----------------------
  > **Windows Azure** - Create and configure websites                                                                                                                                                                                                                                                                      Manual
  >                                                                                                                                                                                                                                                                                                                        
  >                                                                                                                                                                                                                                                                                                                        
  >                                                                                                                                                                                                                                                                                                                        
  > **Steps to create and configure Azure sites:**                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
  a.  Create 2 websites on Azure                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
      For creating website, please follow below steps:                                                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                                                                                                           
  <!-- -->                                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  i.  Click on New                                                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
      ![](media/image1.png)                                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
  ii. Select Compute &gt; Web App &gt; Quick Create as shown below:                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                                                                                           
      ![](media/image2.png)                                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
  iii. Enter the name of the site and select appropriate App Service plan as shown below:                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                           
      ![](media/image3.png)                                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
      Then click on “Create Web App“                                                                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                                                                                           
  <!-- -->                                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  a.  Scaling - **Website mode**                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
    > For **UI** set website mode to Standard                                                                                                                                                                                                                                                                              
    --------------------------------------------------------                                                                                                                                                                                                                                                               
    > For **Service** set website mode to Shared\\Standard                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  >                                                                                                                                                                                                                                                                                                                        
  >                                                                                                                                                                                                                                                                                                                        
  > For websites in Standard mode set instance size based on need as mentioned in Pre-requisites section                                                                                                                                                                                                                   
  >                                                                                                                                                                                                                                                                                                                        
  > **Steps to create and configure Azure storage account:**                                                                                                                                                                                                                                                               
                                                                                                                                                                                                                                                                                                                           
  a.  Click on New                                                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
      ![](media/image1.png)                                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
  b.  Select Data Services -&gt; Storage -&gt; Quick Create                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
      ![](media/image4.png)                                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
  c.  Fill in the details and click Create Storage account                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  d.  This storage account name will be present in Matter Center Excel config file                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
  e.  Get the Azure storage account key and make a note of it in below table                                                                                                                                                                                                                                               
                                                                                                                                                                                                                                                                                                                           
      ![](media/image6.png)                                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
      ![](media/image7.png)                                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
    **Name**                    **Value**                                                                                                                                                                                                                                                                                  
    --------------------------- -----------                                                                                                                                                                                                                                                                                
    Azure Storage Account Key                                                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                           
  > Create cloud storage connection string using the above value as shown below:                                                                                                                                                                                                                                           
  >                                                                                                                                                                                                                                                                                                                        
  > DefaultEndpointsProtocol=**&lt;Protocol&gt;**;AccountName=**&lt;Name of Storage account&gt;**;AccountKey=**&lt;Key Generated in Step 1&gt;**                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
    **Name**                          **Value**                                                                                                                                                                                                                                                                            
    --------------------------------- -----------                                                                                                                                                                                                                                                                          
    Cloud Storage Connection String                                                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                                                                                           
  E.g. DefaultEndpointsProtocol=**https**;AccountName=**mattercenterlog**;AccountKey=**Rse45dwBYaka1ybdgYL0b+v34sEASsa8593LDxsd23iegs3HYTBfJ1hOulCGg/Mhaw4bLZ+RymydCpdqDYi2KuDfa==**                                                                                                                                       
                                                                                                                                                                                                                                                                                                                           
  > **Steps to confirm Current Azure subscription and set it**                                                                                                                                                                                                                                                             
  >                                                                                                                                                                                                                                                                                                                        
  > In case there are multiple Azure subscriptions for the user account, perform following steps (using PowerShell):                                                                                                                                                                                                       
                                                                                                                                                                                                                                                                                                                           
  a.  Check the Current Azure subscription                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
    > Get-AzureSubscription -Current                                                                                                                                                                                                                                                                                       
    ----------------------------------                                                                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                                                                                                           
  ![](media/image8.png)                                                                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                                                                                           
  a.  Check if the UI and service web sites are created in the Current subscription                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                                                                                           
  <!-- -->                                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  i.  In Azure portal, click Subscriptions and in the filter, select the option with the Subscription ID matching with that of the Current subscription (as found in step \#a)                                                                                                                                             
                                                                                                                                                                                                                                                                                                                           
      ![](media/image9.png)                                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
  ii. If not, select another filter, to find the subscription in which the UI and service web sites are present, in order to set it as Current subscription                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
  iii. This is required because, in order to do automated deployment on Azure site, the site needs to be present in the Current subscription for the user account                                                                                                                                                          
                                                                                                                                                                                                                                                                                                                           
  <!-- -->                                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  a.  If UI and service web sites are present in a subscription which is not the Current subscription, set this as Current subscription (using the Subscription ID)                                                                                                                                                        
                                                                                                                                                                                                                                                                                                                           
    Select-AzureSubscription –SubscriptionId “&lt;Subscription ID&gt;”                                                                                                                                                                                                                                                     
    --------------------------------------------------------------------                                                                                                                                                                                                                                                   
                                                                                                                                                                                                                                                                                                                           
  ![](media/image10.png)                                                                                                                                                                                                                                                                                                   

  > **Office 365 - Create App Catalog**                                                                                                                                                                                                                                                                                    Manual
  >                                                                                                                                                                                                                                                                                                                        
  > **Steps to create App Catalog site:**                                                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                           
  a.  Sign in to the Office 365 admin center with your SharePoint Online admin user name and password                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                                                                                           
  b.  Go to Admin &gt; SharePoint                                                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                                                                                           
  c.  Click Apps on the left, and then click App Catalog                                                                                                                                                                                                                                                                   
                                                                                                                                                                                                                                                                                                                           
  > ![](media/image11.png)                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  a.  Select Create a new app catalog site, and then click OK                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                           
  > ![](media/image12.png)                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  a.  On the Create App Catalog Site Collection page, enter the required information, and then click OK                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                                                                                           
  > ![](media/image13.png)                                                                                                                                                                                                                                                                                                 
  >                                                                                                                                                                                                                                                                                                                        
  > After the App Catalog site is created, you can navigate to it within the SharePoint admin center by clicking Apps &gt;App Catalog. The App Catalog site will have a document library for Apps for Office and a document library for Apps for SharePoint, as well as a list that tracks App Requests from site users.   
  >                                                                                                                                                                                                                                                                                                                        
  > ![](media/image14.png)                                                                                                                                                                                                                                                                                                 

  > **Office 365 – Register App**                                                                                                                                                                                                                                                                                          Manual
                                                                                                                                                                                                                                                                                                                           
  a.  Open a browser                                                                                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                                                                                           
  b.  Go to the following secure link                                                                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                                                                                           
  > https://&lt;**domain**&gt;.sharepoint.com/\_layouts/15/appregnew.aspx                                                                                                                                                                                                                                                  
  >                                                                                                                                                                                                                                                                                                                        
  > Note: If you are not logged on to Office365, log on when requested to do so                                                                                                                                                                                                                                            
  >                                                                                                                                                                                                                                                                                                                        
  > ![](media/image15.png)                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  a.  Click Generate to generate a **Client Id** and **Client Secret** on the displayed page                                                                                                                                                                                                                               
                                                                                                                                                                                                                                                                                                                           
  b.  Enter a Title                                                                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                                                                                           
  c.  Enter the App Domain (Domain that is used for UI site on Azure, created in step 2, example: matterexamplesite.azurewebsites.net)                                                                                                                                                                                     
                                                                                                                                                                                                                                                                                                                           
  d.  Enter the Redirect URL (This is the UI start page URL – Azure UI site, example https://matterexamplesite.azurewebsites.net/pages/FindMatter.aspx)                                                                                                                                                                    
                                                                                                                                                                                                                                                                                                                           
  e.  Click Create                                                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
  f.  Click OK                                                                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                                                                                           
  > **Note**: Keep a note of your Client ID, Client Secret, Title, App domain and Redirect URL                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                                                                                           
    > **Client Id:   **                                                                                                                                                                                                                                                                                                    
    ------------------------- --                                                                                                                                                                                                                                                                                           
    > **Client Secret:   **                                                                                                                                                                                                                                                                                                
    > **Title:   **                                                                                                                                                                                                                                                                                                        
    > **App Domain:   **                                                                                                                                                                                                                                                                                                   
    > **Redirect URI:   **                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           

  > Add admin account to term store administrators, for adding account to term store administrators refer Appendix D                                                                                                                                                                                                       

  > Open the main Matter Center solution and update the files listed in Appendix F using the example provided in the code.                                                                                                                                                                                                 Manual

  > Build the solution by either pressing F5 or going to Build &gt; Build Solution                                                                                                                                                                                                                                         Manual

  > Publish the Microsoft.Legal.MatterCenter project in order to create a SharePoint app.                                                                                                                                                                                                                                  Manual
                                                                                                                                                                                                                                                                                                                           
  1.  Right click the Microsoft.Legal.MatterCenter project and click Publish…                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                           
  2.  Click Package the add-in                                                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                                                                                           
  3.  Visual Studio will create a file called Microsoft.Legal.MatterCenter.app at the bin/debug/app.publish/1.0.0.0 folder                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           

  > Copy and create the following structure in the deployment folder under the src folder:                                                                                                                                                                                                                                 Manual
                                                                                                                                                                                                                                                                                                                           
  1.  Copy Microsoft.Legal.MatterCenter.ProviderService in the solution folder to the deployment folder and rename it Service Publish                                                                                                                                                                                      
                                                                                                                                                                                                                                                                                                                           
  2.  Copy Microsoft.Legal.MatterCenter.SharePointAppWeb in the solution folder to the deployment folder and rename it Web Publish                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
  3.  Create a folder called Exchange App and copy the Microsoft.Legal.MatterCenter\_Outlook.xml file from the Microsoft.Legal.MatterCenter\_Outlook\\Microsoft.Legal.MatterCenter\_OutlookManifest folder to the newly created folder                                                                                     
                                                                                                                                                                                                                                                                                                                           
  4.  Create a folder called Office App and copy the Microsoft.Legal.MatterCenter\_OfficeManifest.xml file from the Microsoft.Legal.MatterCenter\_Office\\Microsoft.Legal.MatterCenter\_OfficeManifest folder to the newly created folder                                                                                  
                                                                                                                                                                                                                                                                                                                           
  5.  Create a folder called SharePoint App and copy the file created in Step 7 to the newly created folder                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           

  > Update the configuration values in Excel                                                                                                                                                                                                                                                                               Manual
  >                                                                                                                                                                                                                                                                                                                        
  > **Location:** src/deployment/MCDeploymentConfig.xlsx                                                                                                                                                                                                                                                                   
  >                                                                                                                                                                                                                                                                                                                        
  > **Sheets**: Config, Create\_Group, TermStore\_Config, Client\_Config, Sample\_Data                                                                                                                                                                                                                                     
  >                                                                                                                                                                                                                                                                                                                        
  > **Note**:                                                                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                           
  a.  For fields that are not applicable for your application should be given the value NA. There should be no fields left blank                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
  b.  Update the cloud storage connection string with the value formulated in Step 1                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           

  > **Office 365** – Run Setup (Deploy.ps1 PowerShell script)                                                                                                                                                                                                                                                              Automated
  >                                                                                                                                                                                                                                                                                                                        
  > **Location:** &lt;solution location&gt;/Deployment/Scripts/Deploy.ps1                                                                                                                                                                                                                                                  
  >                                                                                                                                                                                                                                                                                                                        
  > **Script will create following**:                                                                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                                                                                           
    -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------                                                      
    > \#   > Item                                                                                                                                                                                                                                                                                                          
    ------ ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------                                                      
    1.     > **Pre-requisite checker**                                                                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                                                                                                           
           a.  .NET version and installation check                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
           b.  IIS version and installation check                                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                                                                                           
           c.  Excel configuration file existence check                                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                                                                                           
           d.  SharePoint Online version check                                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                                                                                           
           e.  Solution file existence check                                                                                                                                                                                                                                                                               
                                                                                                                                                                                                                                                                                                                           
           f.  .CSPROJ file existence check                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
           g.  Web.config, Web\_Cloud.config, Web\_OnPremise.config file existence check for two solutions                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
    1.     > **Configures permissions to catalog site**                                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                                                                                           
           a.  Add SharePoint Group                                                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                                                                                           
           b.  Add members/users to Group                                                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                           
           c.  Assign permission to Group                                                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
    1.     > **Creates Configuration lists**                                                                                                                                                                                                                                                                               
                                                                                                                                                                                                                                                                                                                           
           a.  Matter Center Matters                                                                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                                                                                           
           b.  Matter Center Roles                                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
           c.  User Pinned Details (for Documents)                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
           d.  User Pinned Matters (for Matters)                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
           e.  Matter Configurations (Default values for the client)                                                                                                                                                                                                                                                       
                                                                                                                                                                                                                                                                                                                           
           f.  Matter Center Help Links                                                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                                                                                           
           g.  Matter Center Help Section                                                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                           
           > **Adds Role details**                                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
    1.     > **Configures Term store**                                                                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                                                                                                           
           a.  Client Ids                                                                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                           
           b.  Clients                                                                                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                                                                                                           
           c.  Practice Group -&gt; AOL -&gt; SOAL                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
           d.  Setting custom property values                                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                           
               i.  Folder Hierarchy                                                                                                                                                                                                                                                                                        
                                                                                                                                                                                                                                                                                                                           
               ii. Content Type                                                                                                                                                                                                                                                                                            
                                                                                                                                                                                                                                                                                                                           
                   Document Templates                                                                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                                                                                           
               iii. Is Folder Structure Present                                                                                                                                                                                                                                                                            
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
    1.     > **Adds metadata to parent content type**                                                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                                                                                           
           a.  Create site columns (predefined site columns are required for apps)                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
           b.  Add these site columns to specified parent content type                                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                                                                                                           
           c.  Create additional content types based on inputs in Excel                                                                                                                                                                                                                                                    
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
    1.     > **Updates app schema files (Office, Outlook and SharePoint)**                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
           a.  Client ID                                                                                                                                                                                                                                                                                                   
                                                                                                                                                                                                                                                                                                                           
           b.  Start page URL                                                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                           
           c.  Domain                                                                                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
    1.     > Updates constants in UI and Service layer based on inputs in Excel                                                                                                                                                                                                                                            
           >                                                                                                                                                                                                                                                                                                               
           > Updates configurations in web.config for Service and UI build                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
    1.     > Activate SharePoint Server Publishing Infrastructure feature on catalog site                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                           
    1.     > Encrypt the appSettings section in web.config files                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
    1.     > Publish UI and service solution to Azure website                                                                                                                                                                                                                                                              
                                                                                                                                                                                                                                                                                                                           
    1.     > Deploys SharePoint Apps\                                                                                                                                                                                                                                                                                      
           > Imports search configuration for app\                                                                                                                                                                                                                                                                         
           > Updates app list permissions                                                                                                                                                                                                                                                                                  
                                                                                                                                                                                                                                                                                                                           
    1.     > Deploys Office Apps                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
    1.     > Deploys Outlook Apps                                                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                                                                                           
    1.     > Adds App to Exchange                                                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                                                                                           
    1.     > Upload files required for Matter landing page, Settings page, Web Dashboard page and Document Details page to SharePoint library                                                                                                                                                                              
           >                                                                                                                                                                                                                                                                                                               
           > Updates the values of ApplicationInsightsID, URL’s for pages on Azure and links on the page in Matter Landing, SPCommon and Document Details JS file. Also updates the references in the Document Details HTML file                                                                                           
                                                                                                                                                                                                                                                                                                                           
    1.     > Create site collection(s) on SharePoint library based on inputs in Excel, creates Matter Center Restricted groups in each site collection, activates Document ID service feature on each of the site collection and on the tenant root site collection                                                        
                                                                                                                                                                                                                                                                                                                           
    1.     > Provision Web Dashboard page(s) and Settings page(s) at tenant level and site collection(s) created in step 15. Also provision Document Details page on Catalog site.                                                                                                                                         
    -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------                                                      
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           

  > Publish all the created content types along with parent content type                                                                                                                                                                                                                                                   Manual
  >                                                                                                                                                                                                                                                                                                                        
  > To know how to publish content types, refer to Appendix C                                                                                                                                                                                                                                                              

  > **Trust the SharePoint App**                                                                                                                                                                                                                                                                                           Manual
                                                                                                                                                                                                                                                                                                                           
  a.  Navigate to site collection where Matter Center App is deployed                                                                                                                                                                                                                                                      
                                                                                                                                                                                                                                                                                                                           
  b.  Click on ellipsis next to apps and then click “PERMISSIONS”                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                                                                                           
      ![](media/image16.png)                                                                                                                                                                                                                                                                                               
                                                                                                                                                                                                                                                                                                                           
  c.  Click on "here" to trust the app                                                                                                                                                                                                                                                                                     
                                                                                                                                                                                                                                                                                                                           
      ![](media/image17.png)                                                                                                                                                                                                                                                                                               
                                                                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           

  > **Install Matter Center app to all site collection across Tenant**                                                                                                                                                                                                                                                     Manual
                                                                                                                                                                                                                                                                                                                           
  a.  Verify if the app is installed successfully                                                                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                                                                                           
  b.  Click on ellipsis next to apps and then click 'Deployment'                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
  > ![](media/image18.png)                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  a.  Select “(All Paths)” and click “on Add &gt;”                                                                                                                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
  > ![](media/image19.png)                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  a.  Click 'OK' button to deploy the apps to all the sites                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
  b.  Click 'Trust It' button to trust the apps                                                                                                                                                                                                                                                                            
                                                                                                                                                                                                                                                                                                                           
  > ![](media/image20.png)                                                                                                                                                                                                                                                                                                 
                                                                                                                                                                                                                                                                                                                           
  a.  The apps will be installed in users My site and the custom action menu item will appear for all users within the 'OneDrive' document library                                                                                                                                                                         
                                                                                                                                                                                                                                                                                                                           
      i.  Go to following URL:                                                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                                                                                                           
  > For e.g. <https://mysharepointtenant-my.sharepoint.com>                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                                           
  i.  Upload a document, in the Documents folder                                                                                                                                                                                                                                                                           
                                                                                                                                                                                                                                                                                                                           
  ii. After the upload is successful, click on the ECB (“…”) menu of the document as shown below:                                                                                                                                                                                                                          
                                                                                                                                                                                                                                                                                                                           
  > ![](media/image21.png)                                                                                                                                                                                                                                                                                                 
  -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

<span id="_One_Click_Deployment_1" class="anchor"><span id="_Toc438060988" class="anchor"></span></span>Appendix A
==================================================================================================================

Adding new client to the Tenant
-------------------------------

**Pre-requisite:**

Site collection should already be created.

**Steps:**

1.  Term store configuration changes:

-   Go to Admin center by typing the following URL in the browser:

-   For e.g. <https://mysharepointtenant-admin.sharepoint.com>

-   Select “term store” from the left navigation menu.

![](media/image22.png)

-   Expand the Taxonomy node, look for MatterCenterTerms node and expand
    it

-   Right-click on the “Clients” term set and select “Create Term” as
    shown below:

![](media/image23.png)

-   Enter the name of the site collection, the term should be created

![](media/image24.png)

-   Now, go to the Custom Properties tab and in the Shared Properties
    section, select Add

-   Enter following values one after other:

  **Shared Property Name**   **Value**
  -------------------------- -------------------------------
  ClientID                   Some number (for e.g. 100002)
  ClientURL                  &lt;site collection URL&gt;

-   After adding above values, click on save button.

<!-- -->

-   Right-click on the Client ID term set and then click on create term
    as shown below:

![](media/image25.png)

-   Enter the number which we previously inserted in step \#6

-   The new term should be added as shown below:

![](media/image26.png)

1.  Activate Document ID feature on the site collection:

-   Navigate to site settings

-   Click on site collection features

![](media/image27.png)

-   Enable the Document ID settings feature

![](media/image28.png)

-   Navigate to Document ID settings tab

![](media/image29.png)

-   Enter the details and click OK

![](media/image30.png)

1.  Provision Web dashboard page:

-   To provision web dashboard at site collection level

-   Navigate to site collection that you just created

-   Navigate to ‘Site contents’

![](media/image31.png)

-   Navigate to ‘Site Pages’

![](media/image32.png)

-   Update web dashboard URL in ‘WebDashboardSP’ HTML

-   Location for ‘WebDashboardSP’ = “&lt;solution location&gt;/Html
    Chunk/webdashboardsp.html”

-   URL should be Azure web dashboard

![](media/image33.png)

-   Update the URL present in WebDashboardFooter.html

![](media/image34.png)

-   Update the application insight id in the JavaScript code

![](media/image35.png)

-   Upload following HTML pages under ‘Site Pages’

    1.  WebDashboardSP.html

    2.  WebDashboardChart.html

    3.  WebDashboardFooter.html

-   Create a new page using Wiki template and provide any name. For
    example, ‘webdashboard’

![](media/image36.png)

-   To create new web part we have to per perform following steps

    1.  Add “Content Editor” web part on the page

![](media/image37.png)

-   Configure web part with following settings

    1.  Provide URL of HTML page which is uploaded in step 5

    2.  Make chrome type as ‘None’

![](media/image38.png)

-   We have to create following three web parts by performing steps
    mentioned in point \#8

  **Web Part**   **File name**
  -------------- -------------------------
  Settings       WebDashboardSP.html
  Charts         WebDashboardChart.html
  Footer         WebDashboardFooter.html

-   Click on ‘Save’ button

-   Verify that web dashboard is loading fine

![](media/image39.png)

1.  Provision Settings page:

**Settings page configurations**

-   Navigate to the site collection that was just provisioned

1.  Navigate to ‘Site contents’

![](media/image31.png)

1.  Navigate to ‘Site Pages’

![](media/image32.png)

1.  Update settings page URL in ‘SettingsSP’ HTML

2.  Location for ‘SettingsSP’ = “&lt;solution location&gt;/Html
    Chunk/settingssp.html”

    Here,

    &lt;solution location&gt; = Matter Center Solution location

    URL should be Azure settings page

![](media/image40.png)

1.  Upload following HTML page under ‘Site Pages’

-   SettingsSP.html

-   Create a new page using Wiki template and provide any name. For
    example, ’settings’

![](media/image41.png)

1.  To add new web part we have to perform following steps

-   Add “Content Editor” web part on the page

![](media/image37.png)

1.  Configure web part with following settings

-   Provide URL of HTML page which is uploaded in step 5

-   Make chrome type as ‘None’

![](media/image42.png)

1.  Click on ‘Save’ button

2.  Select “Site Contents” and then select “Site Pages” as shown below:

![](media/image32.png)

1.  Click on “…” icon on “Settings.aspx” row as shown below:

![](media/image43.png)

1.  In the fly out that opens, click on “…” and then select “Shared
    With” as shown below:

![](media/image44.png)

1.  In the popup that opens up, select “Advanced” as shown below:

![](media/image45.png)

1.  In the page that opens up, click on “Stop Inheriting Permissions”

![](media/image46.png)

1.  Remove the permissions of all the users/groups except for “Owners
    group” as shown below:

![](media/image47.png)

**Settings list configurations**

1.  Create configurations list by following below steps:

-   Go to “Site Contents”:

![](media/image31.png)

-   Click on “add an app, in the page that opens up Select “Custom list”
    as shown below:

![](media/image48.png)

-   Provide the name as “Matter Configurations” as shown below:

![](media/image49.png)

-   In the page that opens up, click on “List” tab on the top and select
    “Create Column” as shown below:

![](media/image50.png)

-   In the popup that opens up, enter “ConfigurationValue” and Select
    “Multiple lines of text” and click on Ok button as shown below:

![](media/image51.png)

-   Go to “List Settings” from the “List” tab and select “Permissions
    for this list” as shown below:

![](media/image52.png)

-   In the page that opens up, Click on “Stop Inheriting Permissions”
    and in the alert box click on “Ok” button as shown below:

![](media/image46.png)

-   Change the permissions to “Read” of all the groups/users except for
    Owners group as shown below:

![](media/image53.png)

-   Verify that settings page is loading fine

![](media/image54.png)

**Note**: We cannot hide a SharePoint list which is created using UI

Appendix B
==========

Change app settings on Azure
----------------------------

1.  Go to Azure Portal using the following URL:
    <https://manage.windowsazure.com>

<!-- -->

1.  Enter the credentials it asks for, then from the left navigation
    select “Web Apps” and then select the web app where you hosted
    “Matter Center Service” or “Matter Center Site”, as shown below:

![](media/image55.png)

1.  Now click on “Configure” tab and go to “app settings” section as
    shown below:

![](media/image56.png)

1.  Update the field as per the requirement and then click on “Save”
    button as shown below:

![](media/image57.png)

1.  Success message should be displayed after clicking on “Save”.

Appendix C
==========

Publish Content types from Content Type Hub
-------------------------------------------

1.  Go to Content type hub site collection, by using the following url:

<!-- -->

1.  For e.g.
    <https://myexampletenant.sharepoint.com/sites/contentTypeHub>

2.  Go to Site Settings &gt; Site Content types as shown below:

![](media/image58.png)

1.  In the page that opens up, there will be a group called
    \_MatterCenter as shown below:

![](media/image59.png)

1.  For each content type, click the link, choose the Manage publishing
    for this content type and select ok to publish the content type.
    Content types can take between 1-48 hours to publish to all the
    site collection. To verify the publishing is complete, go to the
    site collection you would like to create a matter on, click
    Settings &gt; Site Content Types and verify that the \_MatterCenter
    group and the content types are listed there.

![](media/image60.png)

Appendix D
==========

Add admin account to term store admin
-------------------------------------

1.  Go to Admin center by typing the following URL in the browser:

<!-- -->

1.  For e.g. <https://mysharepointtenant-admin.sharepoint.com>

2.  Select “term store” from the left navigation menu

![](media/image22.png)

1.  Select “Taxonomy” node and in the Term Store Administrators, add the
    admin account as shown below:

![](media/image61.png)

1.  Click on Save button

Appendix E
==========

Steps to create Azure redis cache
---------------------------------

1.  Login to new [Azure](https://portal.azure.com/) management portal

<!-- -->

1.  Click on New from the left panel

![](media/image62.png)

1.  Click on Data + Storage &gt; Redis Cache

![](media/image63.png)

1.  Enter required details and click on Create Button (fill in the
    fields shown below):

![](media/image64.png)

Recommendation:

-   Select Pricing tier as per need. Recommendation is to use C0 Basic

1.  It will take around 20-30 minutes for Azure Redis cache to be
    completely ready, once ready open the Azure Redis cache

2.  Click on the keys icon as shown below:

![](media/image65.png)

1.  In the window that opens on the right side, copy the value of the
    primary key and add it in below table:

![](media/image66.png)

1.  Also copy the host name of the cache and add it in below table:

![](media/image67.png)

  **Name**                      **Value**
  ----------------------------- ---------------
  Azure Redis Primary Key       &lt;Value&gt;
  Azure Redis cache host name   &lt;Value&gt;

<span id="_Toc425958927" class="anchor"><span id="_Toc438060998" class="anchor"></span></span>Update service’ appsettings
-------------------------------------------------------------------------------------------------------------------------

1.  Login to new [Azure](https://portal.azure.com/) management portal

<!-- -->

1.  Click on Browse All &gt; Web Apps

![](media/image68.png)

1.  Select the service web app that you created in Create and configure
    websites step

2.  In the window that opens up, select “All Settings”

![](media/image69.png)

1.  Enter following values in the app settings section:

  **Key**               **Value**
  --------------------- -----------------------------------------------------
  Cache\_Host\_Name     Value added in previous table for host name
  Cache\_Primary\_Key   Value added in previous table for Cache primary key

1.  After the changes are done, click on “Save button”

![](media/image70.png)

Appendix F
==========

List of resource files to update in the solution
------------------------------------------------

-   Microsoft.Legal.MatterCenter

    -   Break URL (Elements.xml)

        -   UrlAction

    -   Send To OneDrive (Elements.xml)

        -   UrlAction

    -   Sync (Elements.xml)

        -   UrlAction

    -   UpdateAndCheckIn (Elements.xml)

        -   UrlAction

    -   UpdateAndCheckOut (Elements.xml)

        -   UrlAction

    -   AppManifest (Right click view code)

        -   Product ID (Generate GUID with Visual Studio tool)

        -   Start page

        -   ClientId

-   Microsoft.Legal.MatterCenter.OneDriveRibbon

    -   RibbonCustomAction (Elements.xml)

        -   CommandActions

    -   AppManifest (Right click view code)

        -   ProductID

        -   StartPage

        -   ClientId

-   Microsoft.Legal.MatterCenter.ProviderService

    -   App\_GlobalResource\\Constants.resx

        -   Central\_Repository\_URL (app catalog url)

        -   Common\_CSS\_File\_Location

        -   Common\_JS\_File\_Location

        -   LogTableName

        -   Matter\_Landing\_Page\_CSS\_File\_Name (If your app catalog
            is not at /sites/catalog)

        -   Matter\_Landing\_Page\_jQuery\_File\_Name (If your app
            catalog is not at /sites/catalog)

        -   Matter\_Landing\_Page\_Script\_File\_Name (If your app
            catalog is not at /sites/catalog)

        -   Search\_Result\_Source\_ID (Only if doing manual deployment,
            otherwise scripts will handle)

        -   Site\_Url

        -   Tenant\_WebDashboard\_Link – Update for Localized installs

        -   Template\_Document\_Library\_Link – Update for Localized
            installs

    -   Web.config

        -   ClientId

        -   ClientSecret

        -   HostedAppHostNameOverride

        -   Access-Control-Allow-Origin

-   Microsoft.Legal.MatterCenter.SharePointAppWeb

    -   App\_GlobalResources\\Constants.resx

        -   App\_Redirect\_URL

        -   Application\_Insight\_App\_Id

        -   Central\_Repository\_Url

        -   ClientId

        -   Delve\_Link

        -   Image\_Document\_Icon (needs to be updated for
            manual deployments)

        -   Image\_General\_Document (needs to be updated for
            manual deployments)

        -   Learn\_More\_Link

        -   Legal\_Briefcase\_Service\_Url

        -   Matter\_Provision\_Service\_Url

        -   Office\_JS\_URL

        -   Search\_Service\_Url

        -   Site\_Url

        -   Tenant\_Url

    -   ApplicationInsights.config

        -   InstrumentationKey (Fill in as NA if not using
            Application Insights)

    -   Web.config

        -   ClientId

        -   ClientSecret

        -   HostedAppHostNameOverride

-   Microsoft.Legal.MatterCenter.Utility

    -   Contsants.resx

        -   Provision\_Matter\_App\_URL

    -   Log.resx

        -   CloudStorageConnectionString

        -   UtilityLogTableName

-   Microsoft.Legal.MatterCenter\_Office

    -   Microsoft.Legal.MatterCenter\_OfficeManifest

        -   Id (Generate GUID with Visual Studio tool)

        -   IconURL

        -   AppDomains (UI, Service, SharePoint)

        -   SouceLocation

-   Microsoft.Legal.MatterCenter\_Outlook

    -   Microsoft.Legal.MatterCenter\_OutlookManifest

        -   Id (Generate GUID with Visual Studio tool)

        -   IconUrl

        -   AppDomains (UI, Service, SharePoint)

        -   SouceLocation x 4


