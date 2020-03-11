Релизы
=====================
- Чтобы очистить забаненные прокси- запустить программу два раза кликнув на нее, либо через консоль Bart.exe --checkallproxy
- Прокси берутся по очереди, чтобы добавить новые прокси, перетянуть .txt файл формата ниже на экзешник (работает только для прокси на создание)
```txt
VIP300014
FbI9EYN44g
==========
193.93.194.97:8080
178.57.67.158:8080
91.204.15.173:8080
185.101.69.132:8080
146.185.202.26:8080
146.185.202.16:8080
91.204.15.108:8080
5.62.155.64:8080
79.133.107.91:8080
91.243.93.44:8080
5.188.219.143:8080
178.159.97.155:8080
185.89.101.86:8080
91.243.91.113:8080
5.188.217.136:8080
```

https://github.com/igorbiliba/host_exchage_cs/releases/

### Зависимости
- https://github.com/igorbiliba/host_exchage_cs
- https://github.com/igorbiliba/netex_client_cs
- https://github.com/igorbiliba/365cash_client_cs
- https://github.com/igorbiliba/mine_exchange_cs

Init
=====================
### 1 Создать файл (Settings.json) рядом с .exe
https://github.com/igorbiliba/netex_client_cs/blob/master/netex_client_cs/bin/Release/Settings.json

- targetCurrenciesIds - выбирает один доступный ид, чекает через доступность балансов
- btcAddressTypeByTargetCurrenciesId - вернет тип btc адреса в зависимости от доступной валюты
### 2 Создать файл (ProxySettings.json) рядом с .exe
```js
[
	{ "host":"amsterdam1.perfect-privacy.com",  "ip":"85.17.28.145",    "username": "bankubeda7", "password": "*" },
	{ "host":"amsterdam2.perfect-privacy.com",  "ip":"95.211.95.232",   "username": "bankubeda7", "password": "*" },
	{ "host":"amsterdam3.perfect-privacy.com",  "ip":"95.211.95.244",   "username": "bankubeda7", "password": "*" },
	{ "host":"amsterdam4.perfect-privacy.com",  "ip":"37.48.94.1",      "username": "bankubeda7", "password": "*" },
	{ "host":"amsterdam5.perfect-privacy.com",  "ip":"85.17.64.131",    "username": "bankubeda7", "password": "*" },
	{ "host":"basel1.perfect-privacy.com",      "ip":"82.199.134.162",  "username": "bankubeda7", "password": "*" },
	............
]
```
##### *Как парсить сайт с прокси, см ниже
- ProxyLog.json - лог использования прокси. Регистрирует удачны и неудачные транзы через прокси, если разница между уданой и неудачной, либо от любой, до неудачной больше maxHoursTestPeriodProxy- прокси считается мертвым и больше не используется.

Капча
=====================
### Проверить или на клиенте капча
Дважды нажав на него, если вылезет html, значит капча, если xml с курсами, значит клиент работает

API
=====================
### Курс:
Bart.exe --rate
#### ответ:
```js
{
  "rate":698445.7366,
  "balance":2.68
}
```
### Создание:
Bart.exe --create 6000 +79060671232 3F12oBFJ72SdjZ5rAcVds1uRQTtgYLLFXz
#### ответ:
```js
{
  "account" : "+79060671232",
  "comment" : "#3877525#",
  "btc_amount" : 0.34564
}
```
### Вернет тип btc адреса, который нужно использовать:
Bart.exe --gettypebtcaddress
#### ответ:
```js
{
  "btc_addresstype":"",
  "target_currency_id":106
}
```
или
```js
{
  "btc_addresstype":"bech32",
  "target_currency_id":134
}
```
### Вернет доступные на сервере targetCurrencyId:
Bart.exe --getallowcurrenciesids
#### ответ:
```js
106, 134
```
Proxy
=====================
### Парсить https://www.perfect-privacy.com/en/customer/download/proxy-http:
#### Вставить в косоль:
```js
window.all = "";
$(".servergroup .col-8").each(function(key, el) {
	window.all += el.innerHTML + "\n----------";
})
window.list = window
	.all
	.replace(/<strong>Hostname:<\/strong>/gi, "")
	.replace(/<strong>IPv4:<\/strong>/gi, "")
	.replace(/<strong>Ports:<\/strong>/gi, "")
	.replace(/ /gi, "")
	.replace(/<br>/gi, "")
	.split("\n")
	.filter(
		el => el.indexOf("<strong>") === -1 && el.length > 0
	);
window.groups = window.list.join("\n").split("----------");
window.itemsInGroups = [];
for(var group of window.groups) {
	window.itemsInGroups.push(
		group
		.split("\n")
		.filter( el => el.length > 0 )
	);
}
var result = [];
for(var item of window.itemsInGroups) {
	if(typeof item[1] === "undefined") continue;
	if(item[1].indexOf(".") !== -1)
		result.push({
			host: item[0],
			ip: item[1]
		});
}
$("body").html(JSON.stringify(result));
```
