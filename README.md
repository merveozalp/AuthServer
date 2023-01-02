# JWT Nedir?
Json Web Tokens yani JWT kullanıcının doğrulanması, web servis güvenliği, bilgi güvenliği gibi birçok görevi bulunmaktadır.
# JWT Yapısı :

### Header ,payload ve signature olarak 3 yapıdan oluşmaktadır.
### Header : token tipi ve imzalama için kullanılacak algoritmanın adı bulunmaktadır.
### Payload : Kayıtlı Claim bilgileri içermektedir.
   #### sub:(subject) : JWT kim için oluşturulduğunu belirtir.
   #### aud:(audince) : Token oluşturan server
   #### iss:(issuer) Token kullanacak server
   #### exp: (expiration time)Token geçerlilik süresi 
### Signature: Bu kısım tokenın son kısmıdır. Bu kısmın oluşturulabilmesi için header, payload ve gizli anahtar(secret) gereklidir. İmza kısmı ile veri bütünlüğü garanti altına alınır. Burada kullandığımız gizli anahtar Header kısmında belirttiğimiz algoritma için kullanılır. Header ve Payload kısımları bu gizli anahtar ile imzalanır.

![](https://cdn.auth0.com/blog/legacy-app-auth/legacy-app-auth-5.png)

### Refresh Token ve Acces Token Farkı Nedir?
### Access Token (Erişim Belirteci): Bir kaynağa ulaşmak için verilmiş belirteçtir. Refresh Token (Yenileme Belirteci): Bir erişim belirtecinin geçersiz olduğu durumlarda kullanılmak üzere oluşturulmuş olan ve bu geçersiz belirtecin güncellenmesini/yenilenmesini sağlayan belirteçtir. Projemizi gerçekleştirirken Access Token süresinin mümkün olduğu kadar kısa ve refresh token sürsini ise uzun tutmak sağlıklı olacaktır.

## Gözatılabilcek kaynaklar :
### [JWT Web Site] (https://jwt.io/introduction)
### [Medium Kaynak] (https://tugrulbayrak.medium.com/jwt-json-web-tokens-nedir-nasil-calisir-5ca6ebc1584a)
