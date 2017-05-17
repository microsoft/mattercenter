Feature: Matter Dashboard Page

@E2E
Scenario: 01. Open the browser and load Matter Center home page
	When user enters credentials on matter dashboard page
	Then Matter Center home page should be loaded with element 'mcIcon'	   

@E2E
Scenario: 02. Verify the hamburger menu
	When user clicks on hamburger menu on Matter Center homepage
	Then hamburger menu should be loaded

@E2E
Scenario: 05. Verify the matter fly out on Matter Center homepage
	When user clicks on matter fly out
	Then a matter fly out should be seen

@E2E
Scenario: 06. Verify the search feature on matter center homepage
	When user types 'test' in search box on Matter Center Homepage
	Then all results having 'test' keyword should be displayed

@E2E
Scenario: 04. Verify the upload button functionality
	When user clicks on upload button
	Then an upload pop up should be seen

@E2E
Scenario: 03. Verify the pin/unpin functionality
	When user clicks on pin or unpin
	Then matter should get pinned or unpinned

@E2E
Scenario: 09. Verify the advance filter functionality
	When user clicks on advance filter
	Then filter results should be shown to user

@E2E
Scenario: 08. Verify the sort functionality in matter center home
	When user sorts data for All matters in ascending order
	Then all records should be sorted in ascending order
	When user sorts data for All matters in ascending order of created date
	Then all records should be sorted in ascending order of created date

	When user sorts data for Pinned matters in ascending order
	Then all records should be sorted in ascending order
	When user sorts data for Pinned matters in ascending order of created date
	Then all records should be sorted in ascending order of created date

	When user sorts data for My matters in ascending order
	Then all records should be sorted in ascending order
	When user sorts data for My matters in ascending order of created date
	Then all records should be sorted in ascending order of created date



@E2E
Scenario: 07. Verify the search feature using managed properties on matter center home page
	When user types 'MCMatterName:Test' in search box on Matter Center Homepage
	Then all results having 'Test' keyword should be displayed

@E2E
Scenario: 12. Verify enterprise search feature on matter center home page
	When user types 'Test' in enterprise search box on Matter Center Home page
	Then user should redirect to enterprise page with search results for 'Test'

@E2E
Scenario: 11. Verify no results on invalid search
	When user types gibberish in search box on Matter Center dashboard
	Then no results should be displayed on Matter Center dashboard