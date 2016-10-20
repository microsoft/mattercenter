Feature: Settings Page 

@E2E
Scenario: 01. Open the browser and load settings page
	When user enters credentials on settings page
	Then settings page should be loaded with element 'settingName'

@E2E
Scenario: 02. Verify deletion of team members 
	When user tries to delete all the team members
	Then last team member should not be deleted

@E2E
Scenario: 03. Verify error message on adding non-existing team member
	When user adds non-existing Attorney to the team
	Then error should be thrown on saving

@E2E
Scenario: 04. Set the value on settings page 
	When settings page is configured and save button is clicked
	Then settings should be saved and confirmation message should be displayed

@E2E
Scenario: 05. Verify values on Matter provision page 
	When user goes to matter provision page
	Then preset values should be loaded