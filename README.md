# LogisticCentrWinform

Сделано приложение для Логистического центра и спроектирвоана база данных



# Установка
1. Установить бэкап, который находится в папке AppData
2. Бэкап нужно устанавливать на тот sqlServer, который указан в файле конфига App.Config. По умолчанию в нем указана такая строчка:   
<add name="DefaultConnection" connectionString="Data Source=(localdb)\mssqllocaldb;Initial Catalog=LogicCentr;Integrated Security=True" providerName="System.Data.SqlClient"/>
Сервер по умолчанию: (localdb)\mssqllocaldb, если вы ставите БД  на другой сервер, нужно указать другой сервер, но если ставите на (localdb)\mssqllocaldb ничего указывать не нужно
3. Запустить приложение
