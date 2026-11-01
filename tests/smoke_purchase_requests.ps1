param(
    [string]$BaseUrl = "http://localhost:5004"
)

$ErrorActionPreference = "Stop"
$uri = "$BaseUrl/api/purchase-requests"

function Expect-PostFailure {
    param(
        [hashtable]$Data,
        [int]$ExpectedStatus,
        [string]$Name
    )

    try {
        Invoke-RestMethod -Uri $uri -Method Post `
            -ContentType "application/json" `
            -Body ($Data | ConvertTo-Json -Compress) `
            -ErrorAction Stop | Out-Null
    }
    catch {
        if (-not $_.Exception.Response) { throw }

        $actual = [int]$_.Exception.Response.StatusCode

        if ($actual -ne $ExpectedStatus) {
            throw "FAIL $Name : expected $ExpectedStatus, got $actual"
        }

        Write-Host "PASS $Name (HTTP $actual)"
        return
    }

    throw "FAIL $Name : request unexpectedly succeeded"
}

$before = [int](Invoke-RestMethod -Uri "$uri/count")

if ($before -gt 0) {
    $requests = @(Invoke-RestMethod -Uri $uri)
    $id = [int]$requests[0].id

    $found = Invoke-RestMethod -Uri "$uri/$id"

    if ([int]$found.id -ne $id) {
        throw "FAIL GET by ID"
    }

    Write-Host "PASS GET by ID"
}
else {
    Write-Host "SKIP GET by ID (database is empty)"
}

Expect-PostFailure -Name "Negative quantity" -ExpectedStatus 400 `
    -Data @{ productName = "Monitor"; quantity = -5 }

Expect-PostFailure -Name "Empty product name" -ExpectedStatus 400 `
    -Data @{ productName = " "; quantity = 1 }

$after = [int](Invoke-RestMethod -Uri "$uri/count")

if ($before -ne $after) {
    throw "FAIL Record count changed: $before -> $after"
}

Write-Host "PASS Record count unchanged ($before)"
Write-Host "All smoke tests passed."