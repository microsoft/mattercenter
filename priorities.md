#Matter Center for Office 365 Priorities List 

This solution was originally developed with the Microsoft specific requirements. The feature implementations were specific to Microsoft. There are a few shortcomings that would need to be addressed for the solution to be easily build and better managed by the community. Here are the top priority items that we have identified that we would make the it easier for the solution to be deployed.   

# Engineering updates:

1.	MVC (Model View Controller) architecture for application.
2.	Login Experience (making using for Azure Graph API/ADAL)
3.	Update Namespace for the application
4.	Naming convention updates
•	Provider Service > Upload Helper Functions comments
•	Provider Service > Search Helper Functions comments
•	Provider Service > EditMatterHelperFunctions.cs comments
•	Provider Service > BriefcaseUtilityHelperFunctions
•	Provider Service > BriefCaseHelperFunction comments
•	Remove BriefcaseContentTypeHelperFunctions.cs
•	Provider Service > SearchHelperFunction
•	Provider Service > ProvisionHelperFunctions comments
•	Provider Service > MatterProvision.svc.cs comments
•	Provider Service > LegalBriefcase.svc.cs comments
•	App Web: Prefix the Event Name in Constants file with App_Insights in Name column
•	App Web UIConstantString.cs: Convert it to similar structure of SearchConstants.cs
•	App web > Generic Function > Set Response Method: Change bool bContainsEdit to C# coding practice
•	App Web > Generic Functions: Make use of Colon, Comma, Double Quotes Constants variable
•	Provider Service > Constants.resx comments
•	Provider Service > Constants Folder comments
•	Utility > MimeReader.cs comments
•	Remove all references of LCA DMS
•	Resolve all Style Cop warnings
•	Entity project comments

5.	Code enhancements to improve maintainability 
•	Remove references of NewtonSoft JSON from code
•	App Web: Use Constants file from one single location
•	AppWeb: Kindly add version.aspx page which will only have current version of the application
•	DataLayer > Constants.cs move it under Entity Namespace
•	Divide classes in Data Layer > Constants into individual Files
•	Update OneClick tool as per new folder structure
•	Update name of Sample Matter Utility to Create Matter Utility
•	Convert all the absolute URL's to relative URL's in the resource files 

# Build and Deployment:

1.	One key (F5) build and deploy
2.	Open Deployment script issues
•	Builds (Migrate Deployment scripts to VS build tasks)
•	CreateSiteCollection.ps1 > Loop starting value and index incorrect
•	AppInstall.ps1 > Uninstall if block never executed
•	Deploy.ps1 > Add comments for exe parameters
•	Fix variable casing in deployment scripts

# Feature updates:

1.	Revised External Sharing
2.	REST API for CRUD operation on Matters/Object
3.	OneDrive Integration

# Configuration based deployment:

1.	Object and Properties that are configurable for:
•	Matter/Project
•	Document
•	User/Role
•	Security/Permission
2.	Configurable Taxonomy
3.	Configurable Search schema

# Branding:

1.	UI banding – Ability to change logo and template (Color, font etc.) 
2.	Branding Tools (Admin)
3.	Configuration Wizard to setup the application post deployment

