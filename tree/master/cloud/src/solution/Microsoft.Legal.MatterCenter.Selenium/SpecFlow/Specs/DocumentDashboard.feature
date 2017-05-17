Feature: Document Dashboard Page

@E2E
Scenario: 01. Open the browser and load document dashboard page
	When user enters credentials on document dashboard page
	Then document dashboard page should be loaded with element 'myDocuments'

@E2E
Scenario: 02. Verify the document fly out on document dashboard 
	When user clicks on document
	Then a document fly out should be seen

@E2E
Scenario: 03. Verify the pin/unpin functionality
	When user clicks on pin or unpin icon
	Then document should get pinned or unpinned

@E2E
Scenario: 07. Verify the search feature on document dashboard
	When user types 'test' in search box on document dashboard
	Then all documents having 'test' keyword should be displayed

@E2E
Scenario: 09. Verify the advance filter functionality
	When user clicks on advance filter on document dashboard
	Then filtered results should be shown to user

@E2E
Scenario: 05. Verify the sort functionality on document dashboard
	When user sorts data in ascending order on document dashboard
	Then all records should be sorted in ascending order on document dashboard
	When user sorts data in ascending order of created date
	Then all records should be sorted in ascending order on document dashboard by created date

	When user sorts data for Pinned document in ascending order
	Then all records should be sorted in ascending order on document dashboard
	When user sorts data for Pinned document in ascending order of created date
	Then all records should be sorted in ascending order on document dashboard by created date

	When user sorts data for My document in ascending order
	Then all records should be sorted in ascending order on document dashboard
	When user sorts data for My document in ascending order of created date
	Then all records should be sorted in ascending order on document dashboard by created date

@E2E
Scenario: 04. Verify the mail cart functionality
	When user selects document and clicks on mail cart
	Then popup should display email as link or email as attachment options
	
@E2E    
Scenario: 08. Verify the search feature with managed properties on document dashboard
	When user types 'DocTitle:1' in search box on document dashboard
	Then all documents having '1' keyword should be displayed		  

@E2E
Scenario: 06. Verify invalid text search on document dashboard
	When user types gibberish in search box on document dashboard
	Then no documents should be displayed