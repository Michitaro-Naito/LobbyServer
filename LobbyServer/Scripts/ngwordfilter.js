﻿(function () {
    var pattern = /馬鹿|阿呆|あほ|アホ|あほう|あふぉ|アフォ|かす|くず|クズ|ごみ|ゴミ|野郎|にーと|ニート|NEET|きもおた|キモオタ|[引ひ]きこもり|ひっきー|ヒッキー|ふざけ[るん]な|(嘘|うそ)つくな|(うざ|ウザ)[いイ]|うぜー|ウゼー|黙れ|だまれ|ざまー?みろ|[消き]えろ|[失う]せろ|(死|氏|タヒ)[ね]|シネ|(殺|ころ)[すせそ]|コロス|(凹|ぼこ|ボコ)る|くたばれ|クタバレ|粘着|ねんちゃく|障(害|碍|がい)者|しょうがいしゃ|身障|しんしょう|餓鬼|ガキ|(臭|くさ)い|臭|禿|ハゲ|ブス|(きも|キモ)[いイ]|きめえ|キメエ|変態|へんたい|ヘンタイ|不細工|ブサイク|基地外|気違い|きちがい|キチガイ|まじきち|マジキチ|気狂い|きぐるい|豚|ぶた|ブタ|[詰つ]まら(ない|ね|ん)|下手|無能|無脳|(潰|つぶ)せ|厨|低脳|(頭|あたま)おかしい|目障り|めざわり|麻薬|まやく|コカイン|ヘロイン|シャブ|しゃぶ|シンナー|注射|リーフ|ハーブ|彼氏|電話|080|090|docomo|softbank|willcom|ezweb|糞|[うウ][んン○●][こコちチ]|大便|小便|しょうべん|しっこ|シッコ|すかとろ|スカトロ|ぱこぱこ|パコパコ|(鼻|はな|ハナ)(糞|くそ|クソ)|尻|肛門|腋臭|ワキガ|[ちチ][んンソ○●][こコぽポ]|ちんちん|チンチン|ちんかす|チンカス|TNTN|ぺにす|ペニス|陰茎|金玉|きんたま|キンタマ|睾丸|[まマ][んンソ○●][こコぽポ]|まんまん|マンマン|おめこ|オメコ|(ヴァ|ワ|わ)[ギぎ][ナな]|膣|腟|くりすとす|クリストス|くりとりす|クリトリス|栗と栗鼠|くんに|クンニ|せっくす|セックス|せくろす|セクロス|えっち|エッチ|陰毛|いんもう|いんげ|インゲ|肉棒|にくぼう|勃起|おっき|オッキ|精子|せいし|せーし|射精|しゃせい|精液|せいえき|ざ[ーあ]めん|ザ[ーア]メン|体位|淫|初体験|はつたいけん|あなる|アナル|おっぱい|オッパイ|おっぱお|オッパオ|乳|(巨|きょ|キョ|貧|ひん|ヒン)(乳|にゅ|ニュ|ぬ|ヌ)[うウー]|谷間|何カップ|手ぶら|ぱんつ|パンツ|ぱんてぃ|パンティ|ぶるま|ブルマ|乳首|ちくび|チクビ|自慰|おなにー|オナニー|おなぬー|オナヌー|おなほ|オナホ|ますたー?べーしょん|マスター?ベーション|(しこ|シコ)([るル]|って)|しこしこ|シコシコ|脱げ|脱いで|(喘|あえ)[ぐいでげ]|ふぇら|フェラ|まんぐり|マングリ|ぱいずり|パイズリ|風俗|ふうぞく|そーぷ|ソープ|でりへる|デリヘル|へるす|ヘルス|姦|包茎|ほうけい|童貞|どうてい|性器|せいき|処女|しょじょ|やりまん|ヤリマン|乱交|らんこう|ばいぶ|バイブ|ろーたー|ローター|ぱいぱん|パイパン|中出し|なかだし|れいぷ|レイプ|犯|トルコ(嬢|風呂)|舐|股|膿|性病|淋病|梅毒|クラミジア|反日|小日本|朝鮮|支那|竹島|独島|尖閣|トンスル|とんする|初日の冴えわたる占いにより.*|https?|mixi|gree|mobage|(人狼|じんろう)(わーるど|ワールド|おんらいん|オンライン)|werewolfonline|landingapps|(人|[じ][ん])(狼|[ろ][う])[b][b][s]|[る][る](鯖|[さ][ば])|月下人狼|量子人狼|(過|[か])(疎|[そ])[つ][て][い]?[る]|過疎|オウム|真理教|アレフ|ひかりの輪|麻原彰晃|松本智津夫|統一協会|統一教会|世界基督教統一神霊協会|文鮮明|モルモン教|末日聖徒イエス・キリスト教会|サイエントロジー|ファミリー・インターナショナル|愛の家族|バハーイー教|ジッドゥ・クリシュナムルティ|ランドマーク・エデュケーション|人類の友|ラエリアン・ムーブメント|オプス・デイ|エホバ|アルカイダ|ヤマギシ|パナウェーブ|摂理|創価(学会)?|池田大作|法輪功|顕正会|日蓮上人|法の華|幸福の科学/g;

    var filter = {
        Pattern: {
            'ja-JP': pattern
        }
    };

    window.Apwei = window.Apwei || {};
    window.Apwei.NGWordFilter = filter;
})();