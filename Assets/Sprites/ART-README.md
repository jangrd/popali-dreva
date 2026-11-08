# ART-README.md

Ovaj dokument sadrzi sva potrebna znanja i napomene potrebne za izradu arta
igre.


## Sprite dimenzije

Sav art su 64x64 spriteovi. Animacije su slijed takvih spriteova. Templateovi
sadrze isprekidane 64x64 kvadrate koji pokazuju rub svakog spritea (dakle obrub
je takoder dio spritea). Pri popunjavanju maknuti te obrube.


## Rezervirane boje

Boje koje su ekstremi rgb sustava koriste se kao maske za detail shadere. Ove
je boje zbog toga zabranjeno koristiti u artu. Ovime si olaksavamo posao jer
u jednom spritesetu (jednom png fileu) osiguravamo clean rubove i oznacavamo
maske za ostatak.

| MATERIAL | BOJA    |  R  |  G  |  B  |
|----------|---------|-----|-----|-----|
| zemlja   | red     | 255 |  0  |  0  |
| asfalt   | green   |  0  | 255 |  0  |
| trava    | blue    |  0  | 255 |     |
|          | cyan    |  0  | 255 | 255 |
|          | magenta | 255 |  0  | 255 |
|          | yellow  | 255 | 255 |  0  |


## Animacije

Sve animacije odvijaju se u 24 fps svijetu. To ne znaci da animacije moraju
biti crtane u 24 fps-a. `ANIMATION_TEMPLATE.png` je strukturiran od 3 reda i 8
stupaca. Ideja iza toga je da se animacije crtaju u 8 fps tako da se popuni
gornji redak, koji se onda moze copy pasteati u donja dva retka. To je tako da
u buducnosti imamo opciju laganog dodavanja dodatnih frameova u animaciju ako
je potrebno. Takoder daje vremensku razlucivost animaciji, ukoliko neka radnja
treba trajati duze ili krace od neke druge.
