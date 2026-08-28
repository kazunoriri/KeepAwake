# KeepAwake

KeepAwakeは、Windowsの無操作によるロックを防止するシンプルなタスクトレイ常駐アプリです。

起動中は30秒ごとにごく小さなマウス移動イベントをWindowsへ送信します。
マウスポインターは元の位置に戻るため、通常の操作を邪魔しません。

## 特徴

- インストール不要
- 管理者権限不要
- タスクトレイに常駐
- ワンクリックでロック防止をON / OFF
- Windows起動時の自動起動に対応
- Windowsのグループポリシーやセキュリティ設定を変更しない

## ダウンロード

GitHub Releasesから最新版の `KeepAwake.exe` をダウンロードしてください。

.NETを別途インストールする必要はありません。

### ファイルサイズについて

`KeepAwake.exe` は約110MBあります。

これは、.NETがインストールされていないWindows PCでも `KeepAwake.exe` 単体で動作できるよう、.NETランタイムを実行ファイルに含めて配布しているためです。

KeepAwake本体のプログラムが110MBあるわけではありません。

## 使い方

`KeepAwake.exe` を起動します。

通常のウィンドウは表示されず、タスクトレイにKeepAwakeのアイコンが表示されます。

アイコンを右クリックすると、次の操作ができます。

- **ロックを防止する**  
  ロック防止機能のON / OFF

- **Windows起動時に開始**  
  Windowsへのサインイン時にKeepAwakeを自動起動

- **終了**  
  KeepAwakeを終了

タスクトレイアイコンをダブルクリックしても、ロック防止のON / OFFを切り替えられます。

起動時は「ロックを防止する」がONになっています。

## 自動起動について

「Windows起動時に開始」を使用する場合は、`KeepAwake.exe` を今後移動しない場所に置いてからONにしてください。

自動起動設定後にexeを別のフォルダへ移動した場合は、新しい場所からKeepAwakeを起動し、一度自動起動をOFFにしてから再度ONにしてください。

## 動作環境

- Windows 10
- Windows 11
- x64

## 注意事項

KeepAwakeはWindowsのグループポリシーやセキュリティ設定そのものを変更しません。

組織で管理されているPCなどでは、セキュリティポリシーによって疑似入力がユーザー操作として扱われず、ロックを防止できない場合があります。

また、初回起動時にWindows SmartScreenの警告が表示される場合があります。

## 開発者向け

### 必要環境

- .NET 10 SDK
- Windows 10 / 11

### 実行

```powershell
dotnet run
```

### ビルド

```powershell
dotnet build -c Release
```

### 単体配布用ビルド

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:DebugType=None
```

生成物:

```text
bin\Release\net10.0-windows\win-x64\publish\
```

## License

MIT License
