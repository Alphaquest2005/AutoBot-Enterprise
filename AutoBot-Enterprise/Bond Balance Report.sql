/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [Id], ReferenceNumber,
      AssessmentDate, RegistrationDate
      ,[Type]
      ,[Total]
  FROM [AutoBot-EnterpriseDB].[dbo].[BondBalance-SummaryReport]
  where RegistrationDate between '10/1/2020' and '10/31/2020'
  order by RegistrationDate asc