# Matter Center for Office 365
Microsoft’s Corporate, External, and Legal Affairs (CELA) group, in partnership with the Office extensibility team, developed Matter Center for Office 365 -- a document management and collaboration solution built using the Office app development model, Azure and SharePoint.

Learn more about [Matter Center for Office 365](http://www.microsoft.com/en-us/legal/productivity/mattercenter.aspx). 

# Intro
Microsoft’s Corporate, External, and Legal Affairs (CELA) group, in partnership with the Office extensibility team, developed Matter Center for Office 365 -- a document management and collaboration solution built using the Office app development model, Azure and SharePoint. It takes advantage of the enterprise content management capabilities of the SharePoint platform, and offers many additional benefits through Add-ins in Outlook and Office (e.g., Word, PowerPoint). This allows the solution to quickly create, find, and store documents on predefined projects or matter sites. Matter Center takes advantage of the enterprise-grade cloud platform of Office 365 and Azure, so these productivity gains are realized while also reducing risk and cost. 

# Roadmap
Where is Matter Center going in the future? 

### User Experience 
* Create and finalize design to enhance login experience [adopt ADAL]
* Create configuration tool for taxonomy 
* Enable UI branding 
* Add landing page to Matter Center app
* Allow for editing existing Matters

### Object Lifecycle
* Redesign the service to use REST APIs
* Implement a generic object model
* Adopt MVVC design pattern
* Add Matter Status 

### Integration
* Deliver CRUD API (RESTful) for an Object (Matter) 
* Implement REST response architecture system 

# Installation and build
Please note the following software must be installed before you can open and deploy the Matter Center solution:
* [SharePoint Online Management Shell](http://www.microsoft.com/en-us/download/details.aspx?id=35588)
*	[Windows PowerShell](http://www.microsoft.com/en-in/download/details.aspx?id=2560)
*	[Azure PowerShell](http://go.microsoft.com/?linkid=9811175)
*	 [Visual Studio 2015 (Community or Pro and above)](https://www.visualstudio.com/en-us/products/vs-2015-product-editions.aspx)
*	[Office Developer Tools for VS 2015](http://www.microsoft.com/web/handlers/WebPI.ashx?command=GetInstallerRedirect&appid=OfficeToolsForVS2015) 

Please find links to our additional resources:

 1.	[Matter Center Technical Specifications and System Architecture] (tree/master/cloud/docs/Matter%20Center%20Technical%20Specification.docx)
 2.	[Matter Center Build and Deployment Guide (+ prerequisites)] (tree/master/cloud/docs/Matter%20Center%20Build%20and%20Deployment%20Guide.docx)


#Questions, Issues and Feedback
Having trouble with Matter Center? Check-out [Issues] (https://github.com/Microsoft/mattercenter/issues) and if you don't see your issue, please open an [issue in GitHub](https://github.com/Microsoft/mattercenter/issues/new).

Have feedback? Notice a bug? Submit through the Matter Center page on [UserVoice](http://mattercenter.uservoice.com).

#Contribution
This project was originated by the Microsoft Matter Center Engineering team. We wanted to build a solution to help our own legal professionals improve their productivity. Based on our conversations with many people in the industry, we decided to open source the project so others can use and enhance this solution.
 
We look forward to maturing the project with your help.

### Priorities
We have been working on this code for some time and know its many limitations. We created an initial six (6) months list of [priorities](PRIORITIES.md) that include architectural changes we’d love your help with. Together, we are excited to see these priorities realized and to continue adding to this list. 

On December 17, 2015, we open sourced the project. We are thrilled to talk with as many contributors as possible to provide information and context:

*	For bug fixes, we suggest simply creating an issue through GitHub and/or UserVoice in order to alert people and chat about the design of the fix.

*	Please try to make your pull request(s) as small as possible and be sure to keep your change close to master to avoid merge conflicts.

For features, substantial code contributions, or interface changes, we would appreciate if you look first at the list of [priorities](PRIORITIES.md) and announce your intention to work on a specific subject via the mailing list. This will allow for broader visibility, and maybe help find other people who already are working on that particular subject. 

We would prefer initial work to be based on the priority list. Of course, if the priority list does not contain a specific subject you think is important, please help us identify new subjects and see how they fit in with the other priorities. There is some architectural work and refactoring that needs to happen first (as described in the priorities list) that will enable other work. Therefore, please discuss your suggestions on the mailing list to ensure it is an appropriate addition at this time. The committers will prioritize the list.
 
We encourage that you fork the repo if you’re planning to do substantial work. This lets you move forward at the pace you want and, when it comes time to submit a pull request, we’ll be able to refer to the fully working set of end to end code.

### Additional focus areas 

In addition to the priorities list, we bring Matter Center to GitHub with a wealth of existing feedback. Conversations with various communities has provided for a catalog of focus areas for us to consider as we move forward. 

We have already pushed these ideas to [UserVoice](http://mattercenter.uservoice.com) as well as to the [Issues list](https://github.com/Microsoft/mattercenter/issues). We encourage you to look to these two places.  

#Committers
There are currently three (3) [committers](https://github.com/Microsoft/mattercenter/graphs/contributors):
 
1. Amitabh Mukherjee, [Amitabh.Mukherjee@microsoft.com](mailto:amitabhm@microsoft.com)
2. Leo Conniff, [v-leconn@microsoft.com](mailto:v-leconn@microsoft.com)
3. Sunnykumar Mistry, [v-sumist@microsoft.com] (mailto:v-sumist@microsoft.com) 

Additional committers will be added later based on interest and deep technical involvement with the project.

# Pull Request Checklist
Pull Requests need to adhere to the checklist below. The committers may ask you to make changes before accepting your changes
* <b>Coding guidelines</b>

  Please follow the project [coding guidelines](tree/master/cloud/docs/Matter Center Coding guidelines.docx). 
*	<b>Third party libraries</b>

  External libraries for the project are located in folders:
["\Main\Microsoft.Legal.MatterCenter\packages"] (tree/master/cloud/src/solution/packages) - this folder contains main third party libraries used in this project. Some libraries are licensed in a way that prevents us from shipping them. Your pull request may be declined should that happen. If your contribution changes require adding or modifying a new third party library, please contact the mailing list first and get approval from the committers. 
*	<b>Security and privacy review requirements</b>

 We believe security and privacy are extremely important. Therefore, please place a lot of attention on this area. We will be asking members of the Microsoft IT security team to review the code to provide feedback on common security and privacy issues. 
*	<b>Licensing requirements</b>

 The code you submit should be licensed under the MIT license as is already done in the rest of the repo. If you use code that you did not author (like from Stack Overflow), you need to ensure correct attributions and license statements are made for those code blocks. Please work with the committers or the mailing list if you have any questions. 
*	<b>Description of the changes</b>

 Please include detailed descriptions of the changes:
  *	Purpose of the change(s)
  *	Description of functionality that was changed; and 
  *	Bugs and/or issues detail, if the pull request fixes it. 
 
We’re looking forward to amazing contributions!

#License
Matter Center for Office 365 is licensed under the [MIT Open Source license](http://opensource.org/licenses/MIT).
