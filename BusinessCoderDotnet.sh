#!/bin/bash

# Builds and runs the Melissa Business Coder Cloud API .NET sample.
#
# This script builds BusinessCoderDotnet with dotnet publish, then runs the resulting
# executable, passing along the license and (if supplied) the lookup fields.
#
# Overall flow:
#   1. Parse the command-line options below.
#   2. Resolve the license (--license, then a prompt, then the MD_LICENSE environment variable).
#   3. Publish BusinessCoderDotnet in Release configuration to ./BusinessCoderDotnet/Build.
#   4. Run the built executable: one-shot mode if any lookup field was supplied,
#      otherwise interactive mode (the .NET program prompts for each field).
#
# Options (each takes a value):
#   --company        Business/company name to test.
#   --addressline1   Street address to test.
#   --city           City to test.
#   --state          State to test.
#   --postal         Postal code to test.
#   --country        Country to test.
#   --license        License string. If omitted, the script prompts for it; if the prompt
#                    is left blank, it falls back to MD_LICENSE. Running without --license
#                    always prompts, even when MD_LICENSE is set.
#
# Paths are relative to the current directory, so run the script from its own folder.
#
# Examples:
#   ./BusinessCoderDotnet.sh --license "your-license"
#   ./BusinessCoderDotnet.sh --company "Melissa" --addressline1 "22382 Avenida Empresa" --city "Rancho Santa Margarita" --state "CA" --postal "92688" --country "United States" --license "your-license"

######################### Constants ##########################

RED='\033[0;31m' #RED
NC='\033[0m' # No Color

######################### Parameters ##########################

company=""
addressline1=""
city=""
state=""
postal=""
country=""
license=""

# Read each --flag and its value. A flag with no value (or followed by another
# option) is an error. Unrecognized options are ignored.
while [ $# -gt 0 ] ; do
  case $1 in
    --company) 
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'company\'.${NC}\n"  
            exit 1
        fi 

        company="$2"
        shift
        ;;
    --addressline1)  
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'addressline1\'.${NC}\n"  
            exit 1
        fi 

        addressline1="$2"
        shift
        ;;
    --city) 
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'city\'.${NC}\n"  
            exit 1
        fi 

        city="$2"
        shift
        ;;
    --state)         
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'state\'.${NC}\n"  
            exit 1
        fi 
        
        state="$2"
        shift
        ;;
    --postal) 
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'postal\'.${NC}\n"  
            exit 1
        fi 

        postal="$2"
        shift
        ;;
    --country) 
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'country\'.${NC}\n"  
            exit 1
        fi 

        country="$2"
        shift
        ;;
    --license) 
        if [ -z "$2" ] || [[ $2 == -* ]];
        then
            printf "${RED}Error: Missing an argument for parameter \'license\'.${NC}\n"  
            exit 1
        fi 

        license="$2"
        shift 
        ;;
  esac
  shift
done


# Build paths are relative to the current directory (not the script's location)
CurrentPath="$(pwd)"
ProjectPath="$CurrentPath/BusinessCoderDotnet"
BuildPath="$ProjectPath/Build"


if [ ! -d "$BuildPath" ];
then
    mkdir "$BuildPath"
fi

########################## Main ############################
printf "\n===================== Melissa Business Coder Cloud API =====================\n"

# Get license (either from parameters or user input)
if [ -z "$license" ];
then
  printf "Please enter your license string: "
  read license
fi

# Check for License from Environment Variables 
if [ -z "$license" ];
then
  license=`echo $MD_LICENSE` 
fi

if [ -z "$license" ];
then
  printf "\nLicense String is invalid!\n"
  exit 1
fi

# Start program
# Build project
printf "\n=============================== BUILD PROJECT ==============================\n"

dotnet publish -f="net7.0" -c Release -o "$BuildPath" BusinessCoderDotnet/BusinessCoderDotnet.csproj

# Run project
# No lookup fields supplied -> run interactively; otherwise pass them through for one-shot mode.
# Bash passes empty quoted values as real empty arguments, so unsupplied fields arrive
# empty and the program prompts for them.
if [ -z "$company" ] && [ -z "$addressline1" ] && [ -z "$city" ] && [ -z "$state" ] && [ -z "$postal" ] && [ -z "$country" ];
then
    dotnet "$BuildPath"/BusinessCoderDotnet.dll --license "$license"
else
    dotnet "$BuildPath"/BusinessCoderDotnet.dll \
		--license "$license" \
		--company "$company" \
		--addressline1 "$addressline1" \
		--city "$city" \
		--state "$state" \
		--postal "$postal" \
		--country "$country"
fi


