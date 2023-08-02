# Name:    BusinessCoderCloudAPI
# Purpose: Execute the BusinessCoderCloudAPI program

######################### Parameters ##########################
param(
    $company = '', 
    $addressline1 = '',  
    $city = '',    
    $state = '',      
    $postal = '',     
    $country = '',       
    $license = '', 
    [switch]$quiet = $false
    )

# Uses the location of the .ps1 file 
# Modify this if you want to use 
$CurrentPath = $PSScriptRoot
Set-Location $CurrentPath
$ProjectPath = "$CurrentPath\BusinessCoderDotnet"
$BuildPath = "$ProjectPath\Build"

If (!(Test-Path $BuildPath)) {
  New-Item -Path $ProjectPath -Name 'Build' -ItemType "directory"
}

########################## Main ############################
Write-Host "`n===================== Melissa Business Coder Cloud API =====================`n"

# Get license (either from parameters or user input)
if ([string]::IsNullOrEmpty($license) ) {
  $license = Read-Host "Please enter your license string"
}

# Check for License from Environment Variables 
if ([string]::IsNullOrEmpty($license) ) {
  $license = $env:MD_LICENSE 
}

if ([string]::IsNullOrEmpty($license)) {
  Write-Host "`nLicense String is invalid!"
  Exit
}

# Start program
# Build project
Write-Host "`n=============================== BUILD PROJECT =============================="

dotnet publish -f="net7.0" -c Release -o $BuildPath BusinessCoderDotnet\BusinessCoderDotnet.csproj

# Run project
if ([string]::IsNullOrEmpty($company) -and [string]::IsNullOrEmpty($addressline1) -and [string]::IsNullOrEmpty($city) -and [string]::IsNullOrEmpty($state) -and [string]::IsNullOrEmpty($postal) -and [string]::IsNullOrEmpty($country)) {
  dotnet $BuildPath\BusinessCoderDotnet.dll --license $license 
}
else {
  dotnet $BuildPath\BusinessCoderDotnet.dll --license $license --company $company --addressline1 $addressline1 --city $city --state $state --postal $postal --country $country 
}
