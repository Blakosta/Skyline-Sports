$ErrorActionPreference = "Stop"
$root = $env:GAME_MANAGED_DIR
if (-not $root -or -not (Test-Path $root)) { throw "GAME_MANAGED_DIR is missing." }

$assemblies = @()
Get-ChildItem -LiteralPath $root -Filter "*.dll" -File | ForEach-Object {
    try { $assemblies += [Reflection.Assembly]::LoadFrom($_.FullName) }
    catch { Write-Warning ("Assembly load failed: " + $_.Name + ": " + $_.Exception.Message) }
}

$targets = @(
    "HouseholdInitializeSystem", "CitizenInitializeSystem", "Worker", "Household",
    "HouseholdCitizen", "Employee", "WorkProvider", "HouseholdData", "PropertyRenter",
    "Citizen", "CurrentBuilding", "PersonalCarSelectData", "CitizenUIUtils",
    "HouseholdWealthKey", "SelectedInfoUISystem", "Camera", "TripNeeded",
    "TravelPurpose", "HouseholdMember", "Created", "PropertySeeker", "ResidentialProperty",
    "SpawnCitizen", "m_InitialWealthRange", "m_InitialWealthOffset"
)

$out = New-Object System.Collections.Generic.List[string]
$out.Add("Managed API inspection")
$out.Add("Root: $root")
$out.Add("Assemblies loaded: $($assemblies.Count)")
$out.Add("")

foreach ($assembly in $assemblies) {
    try { $types = $assembly.GetTypes() }
    catch [Reflection.ReflectionTypeLoadException] { $types = $_.Exception.Types | Where-Object { $null -ne $_ } }
    catch { continue }

    foreach ($type in $types) {
        if ($null -eq $type) { continue }
        $hit = $false
        foreach ($needle in $targets) {
            if ($type.Name -like "*$needle*") { $hit = $true; break }
        }
        if (-not $hit) { continue }

        $out.Add("=== $($type.FullName) [$($assembly.GetName().Name)] ===")
        try {
            foreach ($field in $type.GetFields([Reflection.BindingFlags]"Public,NonPublic,Instance,Static,DeclaredOnly")) {
                $out.Add("FIELD $($field.FieldType.FullName) $($field.Name)")
            }
            foreach ($prop in $type.GetProperties([Reflection.BindingFlags]"Public,NonPublic,Instance,Static,DeclaredOnly")) {
                $out.Add("PROPERTY $($prop.PropertyType.FullName) $($prop.Name)")
            }
            foreach ($method in $type.GetMethods([Reflection.BindingFlags]"Public,NonPublic,Instance,Static,DeclaredOnly")) {
                if ($method.IsSpecialName) { continue }
                $args = ($method.GetParameters() | ForEach-Object { "$($_.ParameterType.FullName) $($_.Name)" }) -join ", "
                $out.Add("METHOD $($method.ReturnType.FullName) $($method.Name)($args)")
            }
            foreach ($ctor in $type.GetConstructors([Reflection.BindingFlags]"Public,NonPublic,Instance")) {
                $args = ($ctor.GetParameters() | ForEach-Object { "$($_.ParameterType.FullName) $($_.Name)" }) -join ", "
                $out.Add("CTOR $($type.Name)($args)")
            }
        } catch { $out.Add("INSPECTION ERROR: $($_.Exception.Message)") }
        $out.Add("")
    }
}

$destination = Join-Path $env:GITHUB_WORKSPACE "game-api-inspection.txt"
$out | Set-Content -LiteralPath $destination -Encoding utf8
Write-Host "Wrote API inspection to $destination"
