Feature: ManagePermission

@E2E
Scenario:01. Open the browser and load manage permission page
	When user enters credentials on manage permissions page
	Then manage permission page should be loaded with default permission	

@E2E
Scenario: :02. User will add Attorney to the Matter
	When user adds new Attorney to the matter
	Then Attorney should be added in the matter

@E2E
Scenario: :03. User will save updated Attorney to the matter
	When user clicks on save button on manage permission page
	Then updated Attorney should be added in the matter

