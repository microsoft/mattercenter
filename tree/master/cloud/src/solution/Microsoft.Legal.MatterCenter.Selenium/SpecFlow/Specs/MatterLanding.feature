Feature: MatterLanding

@E2E
Scenario:01. Open the browser and load matter landing page
	When user enters credentials on matter landing page
	Then matter landing page should be loaded with element 'matterInfo'

@E2E
Scenario:02. Verify the matter components
    When user loads matter landing page
	Then all matter components - Task, RSS, Calender and OneNote should be present

@E2E
Scenario:03. Verify the matter profile and matter description
    When user clicks on matter profile tab
	Then all the matter details should be seen 
	When user clicks on matter description tab
	Then matter description should be seen

@E2E
Scenario:04. Verify the footer
    When user clicks on footer
	Then all links should be present in the footer

@E2E
Scenario:05. Verify the hamburger menu
    When user clicks on hamburger menu
	Then hamburger menu should be seen 

@E2E
Scenario:06. Verify the manage user functionality
    When user clicks on group icon
	Then popup should display list of Attorneys

@E2E
Scenario:07. Verify empty results on searching non existing files
	When user types random text in file search
	Then no results should be displayed

	    
