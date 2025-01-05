## **Proje:**
Event-Driven Edge Node Management System Using ZooKeeper  

CSE-532 Dağıtık Sistemler dersi kapsamında terms project olarak geliştirilmektedir.

## **Proje Özeti:**
Bu proje, ZooKeeper kullanarak edge computing ortamlarında gerçek zamanlı bir izleme sistemi geliştirmeyi amaçlamaktadır. Sistem, düğümlerin sağlık durumlarını ve iş yüklerini takip eder, belirli eşiklerin aşılması durumunda veya düğünlerde network kesintisi durumlarında sistem yönlendirmelerini yönetir. Projenin temel hedefleri:

- Gerçek Zamanlı İzleme: Edge düğümlerinin sağlık durumlarını ve performanslarını düşük gecikmeyle takip etmek.
- Hata Yönetimi: ZooKeeper’ın izleme (watch) mekanizmasıyla anormal durumları algılayarak event tetiklemek ve Event Manager üzerinden sıkıntılı Edge noktasını configurasyon versinden çıkararak yeni istekleri almasına izin vermemek. Düzelme durumlarında tekrar configurasyon verisine iyileşen node düğümünü eklemek.
- Tutarlılık ve Performans Dengesi: Tutarlılığı korurken, ölçeklenebilir ve yüksek performanslı bir çözüm sunmak.

## **Topoloji**

[Sistem Topolojisi] [/Assets/images/Topoloji.png]