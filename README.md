# KeepAwake

Windows 10 / 11 向けの小さなタスクトレイ常駐アプリです。
起動中、30秒ごとにごく小さなマウス移動イベントをWindowsへ送信し、無操作によるロック画面への遷移を防ぎます。

## 動かし方

VS Codeでこのフォルダを開き、ターミナルで:

```powershell
dotnet run
```

起動すると通常ウィンドウは表示されず、タスクトレイに常駐します。
タスクトレイアイコンを右クリックすると以下を操作できます。

- ロックを防止する
- Windows起動時に開始
- 終了

タスクトレイアイコンをダブルクリックすると、ロック防止のON/OFFを切り替えます。

## Releaseビルド

```powershell
dotnet build -c Release
```

## .NETが入っていないPCでも動く単体配布用ビルド例

Windows x64:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

生成物は通常、次の下にできます。

```text
bin\Release\net10.0-windows\win-x64\publish\
```

## 注意

- Windowsのグループポリシーや組織のセキュリティ設定そのものを書き換えません。
- 組織管理PCでは、ポリシーによって疑似入力がロック防止として扱われない場合があります。
- 現在のアイコンはWindows標準アイコンです。
