Feature: Matter Search Page

@E2E
Scenario: 01. Open the browser and load search matter page
	When user enters credentials on matter search page
	Then matter search page should be loaded with element 'matterCenterHeader'      

@E2E
Scenario: 07. Verify the matter drop down menu
	When user opens the search matter page
	Then My Matter tab should be loaded
	When user clicks on All Matters
	Then All Matters result should be loaded
	When user clicks on Pinned Matters
	Then Pinned Matters should be loaded

@E2E	
Scenario: 05. Verify the matter search box
	When user types 'test' in search box 
	Then all matters with 'test' keyword should be shown 

@E2E
Scenario: 09. Verify the matter column picker
	When user clicks on column picker icon
	Then a column picker should be shown
	When user checks all columns
	Then all columns should be shown in column header 
	When user removes all the checked columns  
	Then all columns should be hidden in column header except matter column 

@E2E
Scenario: 02. Verify the matter Ecb menu
	When user clicks on ECB menu
	Then a fly out should be shown
	When user clicks on upload to matter
	Then an upload to matter pop up should be shown
	When user clicks on view matter details
	Then matter landing page should load
	When user clicks on go to matter OneNote
	Then user should be redirected to OneNote
    When user clicks on pin this matter or unpin this matter
	Then matter should be pinned or unpinned

@E2E   
Scenario: 03. Verify the matter fly out
	When user clicks on matter
	Then a matter fly out should open
	When user clicks on view matter details in matter fly out
	Then matter landing page should open
	When user clicks on upload to matter in matter fly out
	Then an upload to matter pop up should open
 
@E2E
Scenario: 04. Verify the matter sort
	When user clicks on column name to sort the matter in ascending order
	Then it should sort the matter in ascending order

@E2E
Scenario: 08. Verify the matter filter search
	When user clicks on column filter to filter the matter with the keyword 'Test' on All Matters
	Then it should filter the matter based on filter keyword
	When user clicks on column filter to filter the matter with the keyword 'Test' on My Matters
	Then it should filter the matter based on filter keyword

@E2E	     
Scenario: 06. Verify the matter search box using managed properties
	When user types 'MCMatterName: test' in search box 
	Then all matters with 'test' keyword should be shown 

@E2E
Scenario:10. Verify no results on searching invalid text
	When user searches with random keywords on matter search page
	Then no results should be displayed on matter search page

@E2E
Scenario:11. Verify no results on searching invalid text on matter filter
	When user clicks on column filter to filter the matter using random keyword
	Then no matters should be displayed inside the fly out