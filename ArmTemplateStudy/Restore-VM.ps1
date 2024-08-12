function Select-Snapshot {
    param (
        [string]$Prefix
    )

    # スナップショットの一覧を取得し、プレフィックスに一致するものをフィルタリング
    Write-Host "利用可能なスナップショットを取得中..."
    $filteredSnapshots = az snapshot list --resource-group $ProductResourceGroup --query "[].{Name:name, TimeCreated:timeCreated, Id:id}" -o json |
                         ConvertFrom-Json | Where-Object { $_.Name -like "$Prefix*" }

    # フィルタリングされたスナップショットがない場合、エラーを表示して終了
    if (-not $filteredSnapshots) {
        Write-Error "スナップショットが見つかりません。スクリプトを終了します。"
        exit 1
    }

    # スナップショットの一覧を表示（サフィックスのみ）
    Write-Host "利用可能なスナップショット (プレフィックス: $Prefix):"
    $suffixes = @()
    for ($i = 0; $i -lt $filteredSnapshots.Count; $i++) {
        $suffix = $filteredSnapshots[$i].Name.Substring($Prefix.Length + 1)
        $suffixes += $suffix
        Write-Host "$($i + 1). $suffix (作成日時: $($filteredSnapshots[$i].TimeCreated))"
    }

    # ユーザーにスナップショットを選択させる
    do {
        $selection = Read-Host "使用するスナップショットの番号を入力してください (1-$($filteredSnapshots.Count))"
    } while ($selection -lt 1 -or $selection -gt $filteredSnapshots.Count)

    # 選択されたスナップショットのサフィックスを返す
    return $suffixes[$selection - 1]
}

function Get-Snapshot {
    param (
        [string]$Prefix,
        [string]$Suffix
    )

    # スナップショットの名前を結合
    $snapshotName = "${Prefix}-${Suffix}"

    # スナップショットのIDと名称を取得
    $snapshot = az snapshot show --name $snapshotName --resource-group $ProductResourceGroup --query "{Name:name, Id:id}" -o json | ConvertFrom-Json
    if ($snapshot.Count -eq 0) {
        Write-Error "スナップショット '$snapshotName' が見つかりません。スクリプトを終了します。"
        exit 1
    }



    # スナップショットのIDを返す
    return $snapshot[0]
}


# 共通スクリプトをロード
. $PSScriptRoot\Common\Initialize-Script.ps1

# スナップショットを選択する
$SnapshotSuffix = Select-Snapshot -Prefix (Get-SnapshotPrefix -VirtualMachineName $VirtualMachineNames[0])

foreach ($virtualMachineName in $VirtualMachineNames) {
    # ディスクのリソースIDを取得し、スナップショットのリソースIDと比較
    # ディスクのリソースIDがスナップショットのリソースIDと一致しない場合、VMを削除してディスクを作成する
    $diskName = Get-DiskName -VirtualMachineName $virtualMachineName
    $snapshot = Get-Snapshot -Prefix (Get-SnapshotPrefix -VirtualMachineName $virtualMachineName) -Suffix $SnapshotSuffix
    $diskSourceResourceId = az disk show --name $diskName --resource-group $MyResourceGroup --query "creationData.sourceResourceId" -o tsv
    if($diskSourceResourceId) {
        if ($diskSourceResourceId -ne $snapshot.Id) {
            Write-Host "VM $virtualMachineName のソースリソースIDがスナップショットと一致しません。VM削除中..."
            az vm delete --name $virtualMachineName --resource-group $ResourceGroup --yes
        }
    }

    # Bicepテンプレートをデプロイ
    Write-Host "VM $virtualMachineName をスナップショット $($snapshot.Name) から復元中..."
    $nicName = Get-NicName -VirtualMachineName $virtualMachineName
    az deployment group create `
        --resource-group $MyResourceGroup `
        --template-file "$PSScriptRoot\template\vm.bicep" `
        --parameters "$PSScriptRoot\template\vm.json" `
        --parameters snapshotId=$Snapshot.Id `
        --parameters virtualMachineName=$virtualMachineName `
        --parameters diskName=$diskName `
        --parameters networkInterfaceName=$nicName > $null

    # 作成に成功したのに、失敗したとエラーがでることがあるため、VMの存在を確認
    $vm = az vm show --name $virtualMachineName --resource-group $MyResourceGroup -o json | ConvertFrom-Json

    if ($vm) {
        Write-Host -ForegroundColor Cyan "VM '$virtualMachineName' の作成に成功しました。"
    } else {
        Write-Host -ForegroundColor Red "VM '$virtualMachineName' の作成に失敗しました。"
        exit 1
    }
}

