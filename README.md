# VRM_AI (fork edition)

VRM_AI をCeVIO AI/CSに独自対応したフォーク版です。
独自対応として、ChatGPTのpromptで表現された感情表現に対して、
CeVIOトークの感情パラメータに対応しています。
（つまり、怒っているときは怒った声でしゃべります）

## フォーク版の設定(`config.ini`)

<details>

```ini
CeVIO_exe =
CeVIO_narrator = さとうささら
CeVIO_product = "CeVIO_AI"
```

`CeVIO_narrator`はキャスト名（トークボイス名）、`CeVIO_product`は`CeVIO_AI`(CeVIO AI)または`CeVIO_CS`(CeVIO CS)を指定してください。

`CeVIO_exe`はCeVIOのインストール先を変更しているとき、インストールフォルダを指定してください。通常は指定不要です。

### ボイスの感情設定

```ini
[VoiceEmotion]
Emotes = Happy | Angry | Sad | Relaxed | Surprised
Emotes_weight = 元気:100,普通:10|怒り:100|哀しみ:100|普通:100|元気:100,哀しみ:50
Emotes_option = |Volume:100|Volume:40|Volume:45,Speed:45|ToneScale:75
```

ChatGPTのpromptで表現された感情表現に対して、トークボイスの各種パラメータを指定します。

### `Emotes`

`Emotes`はChapGPTのpromptの感情のリストです。`|`区切りで、スペースが含まれていてもOKです。prompt内で感情パターンを変えている場合はこちらを書き換えてください。

### `Emotes_weight`

`Emotes_weight`はpromptの各感情に対する、トークソフト（ここではCeVIO）の感情パラメータ値です。CeVIOの場は感情名と値（0~100）で指定。一つの感情に対して複数の感情パラメータを値込みで指定できます。`|`で区切り、特に指定しない場合は`||`のように次の指定に飛んでください。

CeVIOの感情パラメータはキャスト毎に異なるため別のキャストを利用するなら書き換える必要があります（どのキャストがどういう感情パラメータを持つかは[InuInu2022/cevio-casts](https://github.com/InuInu2022/cevio-casts)にまとめています。）。

### `Emotes_option`

`Emotes_option`はpromptの各感情に対する、トークソフトの感情パラメータ以外のパラメータです。
CeVIOの場合 `Volume`＝大きさ、`Speed`＝速さ、`Tone`=高さ、`Alpha`＝声質、`ToneScale`=抑揚の5つで、値は`0~100`です。一つの感情に対して複数の感情パラメータを値込みで指定できます。`|`で区切り、特に指定しない場合は`||`のように次の指定に飛んでください。
</details>

## フォーク版のビルド

<details>
通常版の導入方法に加えて、`FluentCeVIOWrapper`が必要です。

`FluentCeVIOWrapper.unitypackage`を[こちら](https://github.com/InuInu2022/FluentCeVIOWrapper/releases/tag/v0.1.7)からDLして、Assets以下に展開してください。必要な独自サーバー、依存ライブラリが一気に導入できます。
</details>

----
# VRM_AI
https://note.com/tori29umai/n/n81f3dd2343f3

VRMファイルを読み込んでChatGPTのAPIとWhisperを連携させるソフトです。
つまりVRM（3Dモデル）とOpenAIのAPIキーさえあれば3DモデルアバターのAIとおしゃべりできます。
合成音声ソフトは現在『VoicePeak、VoiceVox、COEIROINK』に対応済み。
AssistantSeikaにも仮対応しています。（https://note.com/tori29umai/n/n121642310fd4）

PythonスクリプトとUnityのコードが書ければ自由に拡張できます。

# Usage
https://github.com/tori29umai0123/VRM_AI/releases
から最新版のVRM_AI_v〇〇.zipを解凍し、config.ini、Charactor_settings.txt、を設定し、VRM_AI.exeを実行して下さい。
設定の詳細はVRM_AI_v〇〇.zip内のreadme.txtに書いてあります。

# build
自力でビルドする場合、下記のアセットが必要です。

【UniVRM（VRM 1.0及びVRM 0.x）】
https://github.com/vrm-c/UniVRM
表情制御とまばたきや口パクが干渉しないようにするには、\Assets\VRM10\Runtime\IO\Vrm10Importer.cs"のExpression.OverrideBlink（以下略）を全部書き換える必要がある。

【Advanced INI Parser】
https://assetstore.unity.com/packages/tools/advanced-ini-parser-23706

【uLipSync】
https://github.com/hecomi/uLipSync
uLipSyncを動かすにはJobSystemとBurstCompilerが必要なのでインポートしておいてください。
サンプルスクリプト必要なのでインポートしておいてください。
 uLipSync-v〇〇〇-with-Samples.unitypackage等のwith-Samplesのunitypackageをインポートするとはじめからサンプルがついてきます。

【Unityの設定】
このままだと日本語フォントがTextMeshProで使えないので以下のサイトを参考に、NotoSansJP-〇〇 SDF.asset等を作成し各種コンポーネントのフォントに設定すること。
https://taidanahibi.com/unity/text-mesh-pro/

ビルド後のアスペクトの維持に以下のスクリプトを使用しています。
https://tech.drecom.co.jp/unity-keep-windows-screen-ratio/

※Unity 2021.3.15f1環境で開発しました。（動作を保証するものではありません）

# OpenAI_API.exe
Python製のスクリプトです。ローカルアプリAPIサーバーとしてUnityと連携します。以下解説記事。
https://note.com/tori29umai/n/n53e1db740e0b

# sample_API.py
Python製のスクリプトです。OpenAI_API.exeを使わずに独自スクリプトを使いたいとき用のサンプルです。
AItuber配信等したい人向け。

# 拡張スクリプト
有志の方が作ってくださったYoutube APIを使用して配信中のリスナーコメントに対応させる拡張スクリプトです
https://qiita.com/kamatari_san/items/ec89b53e408cbaff785a?utm_campaign=post_article&utm_medium=twitter&utm_source=twitter_share
