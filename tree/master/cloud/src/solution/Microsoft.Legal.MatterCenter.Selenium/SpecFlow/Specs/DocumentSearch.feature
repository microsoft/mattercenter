Feature: Document Search Page

@E2E
Scenario: 01. Open the browser and load search document page
	When user enters credentials on document search page
	Then document search page should be loaded with element 'documentgrid'

@E2E	
Scenario:07. Verify the document drop down menu
	When user clicks on My Documents item from drop down menu
	Then it should display My Documents in header
	When user clicks on Pinned Documents item from drop down menu
	Then it should display Pinned Documents in header
	When user clicks on All Documents item from drop down menu
	Then it should display All Documents in header

@E2E
Scenario: 06. Verify the column picker
	When user clicks on column picker and checks all columns
	Then it should display all the columns in header
	When user clicks on column picker and remove all checked columns
	Then it should not display any columns except document column in header

@E2E
Scenario:05. Verify the document search box
	When user searches with keyword 'Test'
	Then it should display all the document which consist of 'test' keyword

@E2E
Scenario:04. Verify the document sort
	When user click on column name to sort the document in Ascending order
	Then it should sort the document in ascending order

@E2E
Scenario:08. Verify the document filter search
	When user clicks on column filter to filter the documents using keyword 'url' on My Documents
	Then it should filter the document based on filtered keyword
	When user clicks on column filter to filter the documents using keyword 'url' on All Documents
	Then it should filter the document based on filtered keyword 
	    
@E2E
Scenario:02. Verify the document Ecb menu
	When user clicks on ECB menu in document search page
	Then a fly out should open
	When user clicks on open this document
	Then that document should open
	When user clicks on view matter details in fly out
	Then user should be redirected to matter landing page
    When user clicks on pin this document or unpin this document
	Then document should be pinned or unpinned

@E2E
Scenario:03. Verify the document fly out
	When user clicks on document on document search page
	Then a document fly out should open
	When user clicks on open this document in document fly out
	Then that document should open when clicked
	When user clicks on view document details
	Then document landing page should open

@E2E	      
Scenario:09. Verify the document search box for managed search
 	When user searches with keyword 'DocTitle:test'
	Then it should display all the document which consist of 'test' keyword

@E2E
Scenario:10. Verify no results on searching invalid text
	When user searches with random keywords on document search page
	Then no results should be displayed on document search page

@E2E
Scenario:11. Verify no results on searching invalid text on document filter
	When user clicks on column filter to filter the documents using random keyword
	Then no documents should be displayed inside the fly out
