Açıklama:
Bu C# web uygulaması, güçlü bir kullanıcı yönetim sistemi ile kimlik doğrulama ve yetkilendirme özelliklerini birleştirir. Projenin temel özellikleri:

Başlangıç Veritabanı (Seed): Uygulamayı hızlı bir başlangıç için örnek verilerle doldurur.

Kullanıcı Yönetimi: Yeni kullanıcılar eklenebilir, mevcut kullanıcıları güncelleyin veya sistemden kaldırılır.

Rol Yönetimi: Farklı kullanıcı rolleri oluşturulur ve kullanıcılara atanarak yetkilendirme yapılabilir.

Authentication & Authorization: Güvenli kullanıcı girişi ve yetkilendirme mekanizmaları.

Gelişmiş Register (Kayıt): Yeni kullanıcı kayıtlarını etkili bir şekilde alınır.

Başlarken:
1. Gereksinimler:
	.NET Core SDK
	Veritabanı (örneğin SQL Server)

2. Kurulum:
	dotnet restore

3.  Veritabanı Migrasyonu:
	dotnet ef database update

4. Uygulamayı Başlatma:
	dotnet run

Kullanım
Uygulama başladığında tarayıcıda https://localhost:7061/ adresine giderek uygulamaya erişebilirsiniz.

Başlangıç ​​veritabanıyla gelen örnek kullanıcı bilgileri:

Kullanıcı Adı: "Admin"
Şifre: "Admin_123"
