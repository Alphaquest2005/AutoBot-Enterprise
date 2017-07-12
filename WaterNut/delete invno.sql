Select CounterPointSales.INVNO
FROM     EntryData INNER JOIN
                  CounterPointSales ON EntryData.EntryDataId = CounterPointSales.INVNO
WHERE  (CounterPointSales.INVNO IS NOT NULL AND CounterPointSales.DATE >= '4/24/2013' and CounterPointSales.DATE <= '7/31/2013')