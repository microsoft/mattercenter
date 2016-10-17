Feature: Home Page

@E2E
Scenario:01. Open the browser and load home page
	When user enters credentials on homepage
	Then home page should be loaded with element 'HomeContainer'	   

@E2E
Scenario:02. Open the hamburger menu and verify all the elements
	When user clicks on hamburger menu on homepage
	Then hamburger menu should display 'Home','Matters','Documents' and 'Create New Matter' menu

@E2E	
Scenario:03. Verify the links on home page
	When user clicks on learn more and dismiss link
	Then it should dismiss the link

@E2E	
Scenario:04. Verify all components of the page
	When user clicks on matters link
	Then it should open the matter search page
	When user clicks on documents link
	Then it should open the document search page
	When user clicks on upload attachments link 
	Then it should redirect to matter search page
	When user clicks on create a new matter
	Then it should open the matter provision page
	When user clicks on go to matter center home page
	Then it should open the matters page

@E2E
Scenario:05. Verify the Matter Center support link
	When user click on Matter Center support link
	Then it should open draft mail with recipient 'lcaweb2@microsoft.com' and subject as 'CELA Project Center Feedback and Support request'

@E2E
Scenario:06. Verify the contextual help section
	When user clicks on contextual help icon(?)
	Then it should open the contextual help menu

@E2E
Scenario:07. Verify the user profile icon
	When user clicks on user profile icon
	Then it should open user profile details




