ore-silo-ui-title = Блюспейс хранилище
ore-silo-ui-label-clients = Машины
ore-silo-ui-label-mats = Материалы
ore-silo-ui-itemlist-entry = {$linked ->
    [true] {"[Подключено] "}
    *[false] {""}
}{$name} ({$beacon}) {$inRange ->
    [true] {""}
    *[false] (Вне зоны)
}
