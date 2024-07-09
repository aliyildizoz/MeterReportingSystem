# Akıllı Sayaç Verisi İşleme ve Raporlama Sistemi

Bu proje, akıllı sayaçlardan gelen verilerin toplanması, işlenmesi ve raporlanmasını sağlamak amacıyla geliştirilmiştir. Proje kapsamında .NET Core, Angular, RabbitMQ, PostgreSQL, ve MSSQL gibi teknolojiler kullanılmıştır. Aşağıda proje hakkında detaylı bilgiler ve kurulum talimatları bulunmaktadır.

## Kullanılan Teknolojiler

- **MeterService**:
  - **.NET Core Web API**
  - **Entity Framework Core**
  - **MS SQL Database**

- **ReportService**:
  - **.NET Core Web API**
  - **Entity Framework Core**
  - **PostgreSQL Database**
  - **RabbitMQ**
  - **SignalR (Raporlar için Realtime Notification)**
  - **EPPlus (Excel dosyaları için)**

- **Web UI**:
  - **Angular**
  - **Bootstrap**

- **Diğer Kullanılan Araçlar**:
  - **Docker** 
  - **xUnit** 
  - **Docker Compose**

## Projenin Kurulumu

### Docker Kullanarak Kurulum

1. **Docker Compose ile Servisleri Başlatın**:
`docker-compose up -d` komutu ile Docker Compose dosyasındaki tüm servisleri oluşturup ve başlatabilirsiniz.
2. **Web UI'ya Erişim:**
    Tarayıcınızı açın ve [http://localhost:4200](http://localhost:4200) adresine gidin.

### Manuel Kurulum
Bilgisayrınızda ilgili teknolojiler varsa, veritabanı ve rabbitmq connection stringlerini appsettingsten ayarlayıp, uygulamayı çalıştırabilirsiniz.
   
