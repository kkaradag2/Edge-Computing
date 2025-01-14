## **Proje:**
Event-Driven Edge Node Management System Using ZooKeeper  

CSE-532 Dağıtık Sistemler dersi kapsamında terms project olarak geliştirilmektedir.

## **Proje Özeti:**
Bu proje, ZooKeeper kullanarak edge computing ortamlarında gerçek zamanlı bir izleme sistemi geliştirmeyi amaçlamaktadır. Sistem, düğümlerin sağlık durumlarını ve iş yüklerini takip eder, belirli eşiklerin aşılması durumunda veya düğünlerde network kesintisi durumlarında sistem yönlendirmelerini yönetir. Projenin temel hedefleri:

- Gerçek Zamanlı İzleme: Edge düğümlerinin sağlık durumlarını ve performanslarını düşük gecikmeyle takip etmek.
- Hata Yönetimi: ZooKeeper’ın izleme (watch) mekanizmasıyla anormal durumları algılayarak event tetiklemek ve Event Manager üzerinden sıkıntılı Edge noktasını configurasyon versinden çıkararak yeni istekleri almasına izin vermemek. Düzelme durumlarında tekrar configurasyon verisine iyileşen node düğümünü eklemek.
- Tutarlılık ve Performans Dengesi: Tutarlılığı korurken, ölçeklenebilir ve yüksek performanslı bir çözüm sunmak.

## **Projenin Uygulanması İçin Planlanan Ortam**
Docker Desktop üzerinde bir ZooKeeper cluster oluşturulacak. Edge node olarak basit bir uygulama yazılacak. Bu uygulama, 0-100 arasında rastgele oluşturulan CPU ve bellek (memory) bilgilerini ZooKeeper’a gönderecek. ZooKeeper, Event Manager ile birlikte çalışarak belirli eşik değerlerinin (örneğin, %80 CPU ve bellek kullanımı) aşılması durumunda, bu edge düğümüne client isteklerinin yönlendirilmesini engelleyecek. (Configurasyon bilgisi olarak client'a verilmeyecek)

Client’lar, uygun yapılandırmayı ZooKeeper’dan öğrenecek ve isteklerini uygun edge düğümüne gönderecek. Ayrıca, ZooKeeper’ın watch mekanizması sayesinde durumu düzelen edge düğümünden client’lar haberdar olacak ve bu düğüme tekrar istek göndermeye başlayabilecek.

Sistem healty durumlarındaki değişimleri  Prometheus  ve Grafana ile geriye dönük izlenebilirlik sağlanacak.

## **Topoloji**

<img src="/Assets/images/Topoloji.png" alt="Sistem Topolojisi" width="70%">

## **Docker-Compose**

ilk önce docker cluster üzerinde 3 nodlu bir zookeeper kurulumu yapılacaktır. Bu kurulumda açıklayıcı komutlar eklenmiştir.

```yaml
version: '3.8'

services:
  # ZooKeeper'ın birinci düğümü (Leader olabilir)
  zookeeper1:
    image: zookeeper  # ZooKeeper Docker imajını kullan
    container_name: zookeeper1  # Konteyner adı
    ports:
      - "2181:2181"  # ZooKeeper'ın dinleyeceği standart port
    environment:  # ZooKeeper'ın çalışma ortamı değişkenleri
      ZOO_MY_ID: 1  # Bu düğümün kimliği (1. düğüm)
      ZOO_SERVERS:  # ZooKeeper cluster'daki tüm düğümler
        server.1=zookeeper1:2888:3888
        server.2=zookeeper2:2888:3888
        server.3=zookeeper3:2888:3888

  # ZooKeeper'ın ikinci düğümü (Follower olabilir)
  zookeeper2:
    image: zookeeper  # ZooKeeper Docker imajını kullan
    container_name: zookeeper2  # Konteyner adı
    environment:  # Çalışma ortamı değişkenleri
      ZOO_MY_ID: 2  # Bu düğümün kimliği (2. düğüm)
      ZOO_SERVERS:  # ZooKeeper cluster'daki tüm düğümler
        server.1=zookeeper1:2888:3888
        server.2=zookeeper2:2888:3888
        server.3=zookeeper3:2888:3888

  # ZooKeeper'ın üçüncü düğümü (Follower olabilir)
  zookeeper3:
    image: zookeeper  # ZooKeeper Docker imajını kullan
    container_name: zookeeper3  # Konteyner adı
    environment:  # Çalışma ortamı değişkenleri
      ZOO_MY_ID: 3  # Bu düğümün kimliği (3. düğüm)
      ZOO_SERVERS:  # ZooKeeper cluster'daki tüm düğümler
        server.1=zookeeper1:2888:3888
        server.2=zookeeper2:2888:3888
        server.3=zookeeper3:2888:3888
```
- **ZOO_MY_ID:** Her ZooKeeper düğümünün kimliğini belirtir. Bu, cluster'daki rolleri (leader veya follower) tanımlamak için kullanılır.
- **ZOO_SERVERS:** Cluster içindeki diğer ZooKeeper düğümlerini tanımlar. Her düğümün adresi ve portları burada listelenir.
- **2888:** Diğer düğümlerle iletişim için kullanılır.
- **3888:** Lider seçimi gibi yüksek öncelikli işlemler için kullanılır.
- **2181 Portu:** ZooKeeper'ın istemci bağlantılarını dinlediği standart porttur.
- **Container Adı:** Her bir ZooKeeper düğümü için farklı bir container adı tanımlanır.

compose.yml dosyasının olduğu folder'da aşağıdaki komut çalıştırılarak zookeeper kurulumu sağlanır.
```bash
docker-compose up -d
```
Kurulum tamamlandığında aşağıdaki gibi zookeeper nodlarının kurulduğu görülmelidir.

<img src="/Assets/images/compose-up.png" alt="compose up" width="80%">

## **Edge Projesinin Eklenmesi ve Yapılandırılması**
Proje kapsamında, bir temel seviyede bir .NET Core API uygulaması ileride Edge düğümlerini taklit edebilmesi için çözüme eklendi. Bu uygulama, dağıtık mimarilerde düğüm bazlı hizmetleri simüle etmek için tasarlandı ve Docker tabanlı bir ortamda çalıştırılacak şekilde yapılandırıldı.

## Yapılan Çalışmalar

- Proje Eklemesi:
Edge düğümlerini temsil etmek üzere yeni bir API projesi oluşturuldu. Temel seviyede bir .NET Core projesi olduğu için havadurumu servisini veren bir API ile birlikte geldi. Bu projeye ileride zookeper nodlarına CPU ve RAM değerlerini gönderecek yapı kurulacak. Bu aşamada sadece Docker desktop üzerinde 4 adet node olarak çalışabilecek bir kod eklemiş oldu.

- Dockerfile Düzenlemeleri:
Projenin konteynerize edilmesi için gerekli olan Dockerfile oluşturuldu ve yapılandırıldı.
Dockerfile, uygulamanın bağımlılıklarını çözerek gerekli derlemeleri tamamladıktan sonra, çalıştırılabilir bir imaj oluşturacak şekilde ayarlandı.

- Docker Compose Entegrasyonu:
Docker Compose dosyasına gerekli servis tanımlamaları eklendi.
Projenin birden fazla düğüm olarak çalıştırılabilmesi için bağımsız portlar ve ortam değişkenleri tanımlandı.

- Port Ayarları:
Uygulama, Docker üzerinden aşağıdaki portlar üzerinden hizmet verecek şekilde ayarlandı:
8080, 8085, 8090, 8095
Her bir port, farklı bir düğümün hizmet vermesi için izole edildi ve böylece çoklu node çalıştırma senaryoları desteklendi.

Yapılan bu düzenlemeler sayesinde, Edge düğümlerini temsil eden API uygulaması, hem yerel geliştirme ortamlarında hem de Docker tabanlı üretim sistemlerinde kolayca çalıştırılabilir hale geldi. Bu yapılandırma, farklı senaryolar için esnek bir test ortamı sunmakta ve dağıtık sistemlerin mimarisini simüle etme olanağı sağlamaktadır.

<img src="/Assets/images/Edges.png" alt="compose up" width="80%">