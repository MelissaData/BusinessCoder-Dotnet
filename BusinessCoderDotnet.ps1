<#
.SYNOPSIS
    Builds and runs the Melissa Business Coder Cloud API .NET sample.

.DESCRIPTION
    This script builds BusinessCoderDotnet with dotnet publish, then runs the resulting
    executable, passing along the license and (if supplied) the lookup fields.

    Overall flow:
      1. Resolve the license (parameter, prompt, or MD_LICENSE environment variable).
      2. Publish BusinessCoderDotnet in Release configuration to .\BusinessCoderDotnet\Build.
      3. Run the built executable: one-shot mode if any lookup field was supplied,
         otherwise interactive mode (the .NET program prompts for each field).

.PARAMETER company
    Business/company name to test in one-shot mode.

.PARAMETER addressline1
    Street address to test in one-shot mode.

.PARAMETER city
    City to test in one-shot mode.

.PARAMETER state
    State to test in one-shot mode.

.PARAMETER postal
    Postal code to test in one-shot mode.

.PARAMETER country
    Country to test in one-shot mode.

.PARAMETER license
    License string. Resolved in this order:
      1. This parameter.
      2. An interactive prompt, if the parameter was not supplied.
      3. The MD_LICENSE environment variable, if the prompt was left blank.
    Note that the environment variable is the last resort, not the first: running
    without -license always prompts, even when MD_LICENSE is set.

.PARAMETER quiet
    Accepted for parity with other sample scripts; not currently used to suppress output.

.EXAMPLE
    .\BusinessCoderDotnet.ps1 -license "your-license"

.EXAMPLE
    .\BusinessCoderDotnet.ps1 -company "Melissa Data" -addressline1 "22382 Avenida Empresa" -city "Rancho Santa Margarita" -state "CA" -postal "92688" -country "USA" -license "your-license"
#>

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
# No lookup fields supplied -> run interactively; otherwise pass them through for one-shot mode.
if ([string]::IsNullOrEmpty($company) -and [string]::IsNullOrEmpty($addressline1) -and [string]::IsNullOrEmpty($city) -and [string]::IsNullOrEmpty($state) -and [string]::IsNullOrEmpty($postal) -and [string]::IsNullOrEmpty($country)) {
  dotnet $BuildPath\BusinessCoderDotnet.dll --license $license 
}
else {
  dotnet $BuildPath\BusinessCoderDotnet.dll --license $license --company $company --addressline1 $addressline1 --city $city --state $state --postal $postal --country $country 
}
