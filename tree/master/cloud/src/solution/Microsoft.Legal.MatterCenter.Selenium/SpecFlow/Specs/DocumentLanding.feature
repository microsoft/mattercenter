Feature: DocumentLanding

@E2E
Scenario:01. Open the browser and load document landing page
	When user enters credentials on document landing page
	Then document landing page should be loaded with element 'documentName'
	
@E2E
Scenario:02. Verify action links
	When user loads document landing page
	Then document action links should be present
	
@E2E
Scenario:04. Verify file properties
	When user expands file properties section
	Then all file properties should be present  

@E2E
Scenario:05. Verify version details
	When user expands version section
	Then all versions of the document should be seen
      
@E2E
Scenario:06. Verify the footer links
	When user clicks on footer on document landing page
	Then all links should be present on footer on document landing page  

@E2E
Scenario:03. Verify the pin/unpin functionality
	When user clicks on pin/unpin button
	Then document should get pinned/unpinned