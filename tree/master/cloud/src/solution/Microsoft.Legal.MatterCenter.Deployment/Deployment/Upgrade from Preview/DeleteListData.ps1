param
(
    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [String] $Listname,

    [parameter(Mandatory=$true)]            
    [ValidateNotNullOrEmpty()]             
    [String] $Username,

    [parameter(Mandatory=$true)]                         
    [ValidateNotNullOrEmpty()]
    [String] $Password,

    [parameter(Mandatory=$true)]                         
    [ValidateNotNullOrEmpty()]
    [String] $WebUrl
)

#---------------------------------------------------
#
#  Delete all list items from the list
#
#---------------------------------------------------

try
{    
    $ctx = New-Object Microsoft.SharePoint.Client.ClientContext($WebUrl)
    $ctx.Credentials = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($Username, $(convertto-securestring $Password -asplaintext -force))

    $web = $ctx.Web;
    $ctx.Load($web.Lists)
    $Ctx.ExecuteQuery()
    $list = $web.lists | where {$_.title -eq $Listname}

    $query = new-object Microsoft.SharePoint.Client.CamlQuery
    $query.ViewXml = '<Query><OrderBy><FieldRef Name="ID" /></OrderBy></Query>'
    $items = $list.GetItems($query)
    $ctx.Load($items)
    $ctx.ExecuteQuery()

    foreach ($item in $items)
    {        
        $list.getitembyid($Item.id).DeleteObject()
    }
    $ctx.ExecuteQuery()

    Write-Host 'Pin information is cleared successfully.' -ForegroundColor Green
}
catch [Exception] {
    Write-Log $ErrorLogFile $_.Exception.ToString();
}

