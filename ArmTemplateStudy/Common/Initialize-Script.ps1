function Select-Snapshot {
    # スナップショットの一覧を取得
    Write-Host "利用可能なスナップショットを取得中..."
    $snapshots = az snapshot list --resource-group $ResourceGroup --query "[].{Name:name, TimeCreated:timeCreated}" -o json | ConvertFrom-Json

    if ($snapshots.Count -eq 0) {
        Write-Error "利用可能なスナップショットが見つかりません。スクリプトを終了します。"
        exit 1
    }

    # スナップショットの一覧を表示
    Write-Host "利用可能なスナップショット:"
    for ($i = 0; $i -lt $snapshots.Count; $i++) {
        Write-Host "$($i + 1). $($snapshots[$i].Name) (作成日時: $($snapshots[$i].TimeCreated))"
    }

    # ユーザーにスナップショットを選択させる
    do {
        $selection = Read-Host "使用するスナップショットの番号を入力してください (1-$($snapshots.Count))"
    } while ($selection -lt 1 -or $selection -gt $snapshots.Count)

    return $snapshots[$selection - 1].Name
}

function Restore-Disk {
    param (
        [Parameter(Mandatory=$true)]
        [string]$SnapshotName
    )

    # スナップショットのリソースIDを取得
    $snapshotResourceId = az snapshot show --name $SnapshotName --resource-group $ResourceGroup --query id -o tsv

    if (-not $snapshotResourceId) {
        Write-Error "スナップショット '$SnapshotName' が見つかりません。"
        exit 1
    }

    # パラメーターファイルを読み込む
    $parameterFilePath = "$PSScriptRoot\template\disk.json"
    $parameters = Get-Content $parameterFilePath | ConvertFrom-Json

    # スナップショットのリソースIDを更新
    $parameters.parameters.snapshots_snp_arm_template_study_dev_eastjp_20240811073046_externalid.value = $snapshotResourceId

    # 更新したパラメーターを一時ファイルに保存
    $tempParameterFile = [System.IO.Path]::GetTempFileName()
    $parameters | ConvertTo-Json -Depth 10 | Set-Content $tempParameterFile
    try {
        # Bicepテンプレートをデプロイ
        Write-Host "Bicepテンプレートを使用してディスク '$DiskName' を作成中..."
        $deployment = az deployment group create `
            --resource-group $ResourceGroup `
            --template-file "$PSScriptRoot\template\disk.bicep" `
            --parameters "@$tempParameterFile" `
            --query properties.outputs

        if ($LASTEXITCODE -eq 0) {
            Write-Host "ディスク '$DiskName' をスナップショット '$SnapshotName' から正常に作成しました。"
        } else {
            Write-Error "ディスク '$DiskName' の作成に失敗しました。"
            exit 1
        }

        return $deployment
    }
    finally {
        # 一時ファイルを削除
        Remove-Item $tempParameterFile
    }
}

# 読み取り専用変数（定数）を定義
Set-Variable -Name Subscription -Value "Visual Studio Enterprise サブスクリプション" -Option ReadOnly -Scope Script
Set-Variable -Name ResourceGroup -Value "rg-arm-template-study-dev-eastjp-001" -Option ReadOnly -Scope Script
Set-Variable -Name Location -Value "japaneast" -Option ReadOnly -Scope Script
Set-Variable -Name DiskName -Value "ARM-TEMPLATE" -Option ReadOnly -Scope Script
Set-Variable -Name DiskSku -Value "Standard_LRS" -Option ReadOnly -Scope Script

# 現在のサブスクリプションを確認
Write-Host "サブスクリプションの確認中..."

# 現在のサブスクリプションが希望のものと異なる場合、切り替える
$currentSubscription = az account show --query name -o tsv
if ($currentSubscription -ne $Subscription) {
    Write-Output "サブスクリプションを '$Subscription' に切り替えています..."
    az account set --subscription $Subscription
    if ($LASTEXITCODE -eq 0) {
        Write-Output "サブスクリプションの切り替えに成功しました。"
    } else {
        Write-Error "サブスクリプションの切り替えに失敗しました。"
        exit 1
    }
}
