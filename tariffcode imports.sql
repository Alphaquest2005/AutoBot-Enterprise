SELECT LicenceTariffCodes.TariffCode
FROM    TariffCodes right outer  JOIN
                 LicenceTariffCodes ON TariffCodes.TariffCode = LicenceTariffCodes.TariffCode
WHERE (TariffCodes.TariffCode IS NULL)

select * from TariffCodes where TariffCode like '%4079000'
select * from LicenceTariffCodes where TariffCode like '%4079000'

--update TariffCodes
--set LicenseRequired = 1
--where TariffCode in (select TariffCode from LicenceTariffCodes)

--update LicenceTariffcodes
--set tariffcode = LEFT(tariffcode, LEN(tariffcode) - 2) 

--update LicenceTariffcodes
--set tariffcode = '0' + tariffcode
--where len(TariffCode) = 7

select len('4079000')

--insert into TariffCodes(TariffCode, LicenseRequired)
--SELECT LicenceTariffCodes.TariffCode, 1
--FROM    TariffCodes right outer  JOIN
--                 LicenceTariffCodes ON TariffCodes.TariffCode = LicenceTariffCodes.TariffCode
--WHERE (TariffCodes.TariffCode IS NULL)