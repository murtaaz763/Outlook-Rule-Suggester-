# The below section would start new PowerShell in elevated mode
param([switch]$Elevated)

function Test-Admin {
  $currentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
  $currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
}

if ((Test-Admin) -eq $false)  {
    if ($elevated) 
    {
        # tried to elevate, did not work, aborting
    } 
    else {
        Start-Process powershell.exe -Verb RunAs -ArgumentList ('-noprofile -noexit -file "{0}" -elevated' -f ($myinvocation.MyCommand.Definition))
}

exit
}

'Running with full privileges'

#Setting Execution policy for the current process to run

Set-ExecutionPolicy -ErrorAction SilentlyContinue -Force -Scope Process -ExecutionPolicy RemoteSigned

# The command will disable UAC and won't prompt user for "Yes" or "No"
$val = Get-ItemProperty -Path hklm:software\microsoft\windows\currentversion\policies\system -Name "ConsentPromptBehaviorAdmin"
if($val.ConsentPromptBehaviorAdmin -ne 6)
{
 set-itemproperty -Path hklm:software\microsoft\windows\currentversion\policies\system -Name "ConsentPromptBehaviorAdmin" -value 6
}


<# 
Check to see if a connection already exists to File Share, if exists then delete the current connection 
and create a new one, else create a new connection 
#>

$checkpath = Test-Path "\\192.168.2.12\scripts"

if($checkpath -eq "TRUE")
    {
        Write-Host 'Deleting the exisiting connection to File Share'
        Start-Sleep -s 1
        net use \\192.168.2.12 /Delete
        Write-Host 'Initiating a new connection to File Share'
        Start-Sleep -s 1
        net use \\192.168.2.12 Pass@123 /user:maquser /Persistent:Yes
    }
else
    {
     Write-Host 'Initiating a new connection to File Share'
     Start-Sleep -s 1
     net use \\192.168.2.12 Pass@123 /user:maquser /Persistent:Yes
    }

write-host "Please wait while the files are being copied to C:\Users\Public\Public Downloads..."

#Check to see if the Patch_Tuesday_Updates folder exists, if exists then delete the folder and its content

$strFolderName="C:\Users\Public\Downloads\Patch_Tuesday_Updates_Win10"
If (Test-Path $strFolderName)
    {
	    Remove-Item $strFolderName -Recurse -Force
    }

#Copy the updates from File Share to C:\Users\Public\Downloads\Patch_Tuesday_Updates(Creates a folder if required)
write-host -ForegroundColor Green "Kindly wait till the files are being copied into local machine.."
Copy-Item -Path \\192.168.2.12\Site_Software\8_ServicePacks_and_Hotfixes\Patch_Tuesday_Updates_Win10 -Destination C:\Users\Public\Downloads\Patch_Tuesday_Updates_Win10 -Force -Recurse

write-host "Starting the Installations.." #Notify the user about Installations
write-host "Note: This month, there are some heavy individual updates which takes 4-5 minutes to install. Please do not abort the script thinking that it is hanged.."
#write-host "If the UAC prompt shows up click 'Yes', Clicking 'No' will terminate the script.." #Notify the user about UAC prompt
Start-Sleep -s 3 #wait for 3 seconds

<# 
Start with the .exe installations, the quite switch "/q" has been provided so user intervention is not required.
UAC may prompt for which the user needs to click "Yes" 
#>


try
    {
	    Get-ChildItem C:\Users\Public\Downloads\Patch_Tuesday_Updates_Win10\*.exe| ForEach-Object{$setup=start-process $_.FullName /q -wait}
    }

catch [Exception]
    {
	    write-host $_.Exception.Message;
	    return
    }

# Specify the location of the *.msu files

$updatedir = "C:\Users\Public\Downloads\Patch_Tuesday_Updates_Win10"

#Start with the .msu installations

$files = Get-ChildItem $updatedir -Recurse
$msus = $files | ? {$_.extension -eq ".msu"}

foreach ($msu in $msus)
    {
	    try
	        {
		        write-host "Installing update $msu ..."
		        $fullname = $msu.fullname
		        # Need to wrap in quotes as folder path may contain spaces
		        $fullname = "`"" + $fullname + "`""
		        # Specify the command line parameters for wusa.exe
		        $parameters = $fullname + " /quiet /norestart"
		        # Start wusa.exe and pass in the parameters
		        $install = [System.Diagnostics.Process]::Start( "wusa",$parameters )
		        $install.WaitForExit()
		        write-host "Finished installing $msu"
		    }
		
catch [Exception]
    {
	    write-host $_.Exception.Message;
	    return
    }
}

# The command will enable UAC which was disabled earlier
$val = Get-ItemProperty -Path hklm:software\microsoft\windows\currentversion\policies\system -Name "ConsentPromptBehaviorAdmin"
if($val.ConsentPromptBehaviorAdmin -ne 5)
{
 set-itemproperty -Path hklm:software\microsoft\windows\currentversion\policies\system -Name "ConsentPromptBehaviorAdmin" -value 5
}

#After installing all the updates, Prompt the user to either restart or exit the script, Default here is Restart [R]
   
$caption = "Choose Action";
$message = "What do you want to do?";
$restart = new-Object System.Management.Automation.Host.ChoiceDescription "&Restart","Restart";
$exit = new-Object System.Management.Automation.Host.ChoiceDescription "&Exit","Exit";
$choices = [System.Management.Automation.Host.ChoiceDescription[]]($restart,$exit);
$answer = $host.ui.PromptForChoice($caption,$message,$choices,0) #Here 0 is the default choice

switch ($answer)
    {
        0 {Restart-Computer; break}
        1 {Exit; break} 
    }