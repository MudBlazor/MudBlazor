Clear-Host

$dir = Split-Path -Parent $PSCommandPath
$dir_utilities = $dir.Replace("tools\CssUtilitiesGenerator", "src\MudBlazor\Styles\utilities")
$dir_generated_css = "$dir\css\"
$dir_generated_output = "$dir\output\"

$utilities = @()

Get-ChildItem -Path $dir_utilities | ForEach-Object { $utilities += $_.Name.Replace("_","").Replace(".scss","");}


if(Test-Path -Path $dir_generated_css -IsValid)
{
    Remove-Item -Path "$dir_generated_css\*.*"
}

if(Test-Path -Path $dir_generated_output -IsValid)
{
    Remove-Item -Path "$dir_generated_output\*.*"
}

foreach ($utility in $utilities)
{
    $generated_file = "$dir_generated_css\$utility.css"
    $generated_razor = "$dir_generated_output\$utility.razor"

    sass "$dir_utilities\_$utility.scss" $generated_file --no-source-map 

    #((Get-Content -Path $generated_file -Raw) -Replace "@media[^{]+\{([\s\S]+?})\s*}","") | Set-Content -Path $generated_file

    $razorOutput = "<table><tbody>"
    
    foreach($class in Get-Content -Path $generated_file){
        if($class.Length -gt 1)
        {
            $className = $class
            
            if($className.StartsWith(".") -and $className.EndsWith(","))
            {
                $razorOutput += "<tr>"
                $className = $className.Replace(".","").Replace(",","")
                $razorOutput += "<td>$className</td>"
            }
            elseif($className.StartsWith(".") -and $className.EndsWith(" {"))
            {
                $razorOutput += "<tr>"
                $className = $className.Replace(".","").Replace(" {","")
                $razorOutput += "<td>$className</td>"
            }
            else{
                $value = $className.Replace(" !important;",";")
                $razorOutput += "<td>$value</td>"
            }
        }
        if($class.StartsWith("}")){
            $razorOutput += "</tr>"
        }
    }
    $razorOutput += "</tbody></table>"
    $razorOutput | Out-File "$dir_generated_output\$utility.html"
}

Write-Host "Done" -ForegroundColor "green"
