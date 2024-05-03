# BoxNestGroup

Box の グループをネスト(入れ子)管理するWPFアプリ

## 1.概要

｢WPF学習しながら作成中｣

Box の グループをネスト(入れ子)管理して、グループに設定したユーザーを上位のグループを含めて設定できるアプリ

｢グループ A｣
├｢グループ B｣
├｢グループ C｣
:└｢グループ D｣

｢グループD｣にユーザー設定すると、上位の｢グループC｣｢グループA｣も一緒に登録します。

承認　　　：オフラインでオンラインで更新できます

オフライン：Excelファイルを読み込んで、編集後書き出しできます。


## 2.注意点

グループ名記入(グリッドビュー内)の改行は｢Shift+Enter｣です。

Box API を凄く使いますのでご注意下さい。

## 3.今後

①ユーザー名の絞り込みフィルターなどの機能追加

②ユーザー作成機能の追加

③ユーザー個人フォルダ作成

④速度アップ

## 4.ライセンス

MIT license

## 5.利用

developer.box.com：APACHE LICENSE

　https://developer.box.com/

  https://github.com/box/box-windows-sdk-v2

Microsoft.Web.WebView2 : Copyright (C) Microsoft Corporation. 
  
  https://www.nuget.org/packages/Microsoft.Web.WebView2/1.0.2478.35/license

ClosedXML : MIT

  https://github.com/ClosedXML/ClosedXML


## 6.バージョン履歴

 2024/03/06 0.0.1 C# WPF で作成開始

 2024/05/01 0.1.0 基本的な作成終了
 
 2024/05/03 0.1.1 不具合

　オフライン時の新規でフォルダ(グループ)作成した時に更新しない不具合を修正

　ユーザー一覧の表示が最大化したときにサイズが変わらない不具合を更新

　既存フォルダ(グループ)を再度作ってしまう不具合を修正

　アイコン追加

## 7.連絡

nzds23@yahoo.co.jp

## 8.商用利用

ライセンス条項を守って頂ければ特に制限ありません。

可能なら記載したいので、メールアドレスに連絡頂ければ幸いです。

