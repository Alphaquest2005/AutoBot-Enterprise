

drop table #Orignal
SELECT        AsycudaItemBasicInfo.ItemNumber, SUM(AsycudaItemBasicInfo.ItemQuantity) AS Quantity, AVG(AsycudaDocumentItemCost.ForexItemCost) AS ForexItemCost
into #Orignal
FROM            xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                         AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                         AsycudaItemBasicInfo ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = AsycudaItemBasicInfo.ASYCUDA_Id INNER JOIN
                         AsycudaDocumentBasicInfo ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id AND AsycudaItemBasicInfo.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                         AsycudaDocumentItemCost ON AsycudaItemBasicInfo.Item_Id = AsycudaDocumentItemCost.Item_Id
WHERE       /* (AsycudaDocumentBasicInfo.Reference LIKE N'%SEPIM7%')*/ (AsycudaDocumentSet.Declarant_Reference_Number = 'Old-Sep') AND (AsycudaDocumentBasicInfo.DocumentType = 'IM7' or AsycudaDocumentBasicInfo.DocumentType = 'OS7')
GROUP BY AsycudaItemBasicInfo.ItemNumber, AsycudaDocumentSet.Declarant_Reference_Number, AsycudaDocumentBasicInfo.DocumentType
ORDER BY AsycudaItemBasicInfo.ItemNumber

drop table  #Copy
SELECT        AsycudaItemBasicInfo.ItemNumber, SUM(AsycudaItemBasicInfo.ItemQuantity) AS Quantity
into #Copy
FROM            xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                         AsycudaItemBasicInfo ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = AsycudaItemBasicInfo.ASYCUDA_Id INNER JOIN
                         AsycudaDocumentBasicInfo ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id AND AsycudaItemBasicInfo.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
where  (AsycudaDocumentBasicInfo.Reference LIKE N'%SEpIM7%') AND (AsycudaDocumentBasicInfo.DocumentType = 'IM9')
GROUP BY AsycudaItemBasicInfo.ItemNumber, AsycudaDocumentBasicInfo.DocumentType     
ORDER BY AsycudaItemBasicInfo.ItemNumber

--select * from #Orignal where itemnumber = 'AB/2080014000002'
--select * from #Copy where itemnumber = 'AB/2080014000002'

SELECT        [#Orignal].ItemNumber, [#Copy].ItemNumber AS Expr1, [#Orignal].Quantity, [#Copy].Quantity AS Expr2, [#Orignal].ForexItemCost
FROM            [#Orignal] LEFT OUTER JOIN
                         [#Copy] ON [#Orignal].ItemNumber + CAST([#Orignal].Quantity AS varchar) = [#Copy].ItemNumber + CAST([#Copy].Quantity AS varchar) 
WHERE        ([#Copy].ItemNumber IS NULL)

select * from #Copy where itemnumber not in (select distinct itemnumber from EntryDataDetails)