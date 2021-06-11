SELECT 'INSERT INTO [AutoBot-Tao].dbo.[ObjectView]' 
		+ ' SELECT  ''' + syo.name + ''',[' + skey.name + '],''' + syc.name + ''',' + syc.name + ' FROM    [AutoBot-EnterpriseDB].dbo.[' + syo.name + '] where [' + syc.name + '] is not null'
                  AS Expr1
FROM    sys.sysobjects AS syo INNER JOIN
                 sys.syscolumns AS syc ON syc.id = syo.id INNER JOIN
                 sys.systypes AS syt ON syt.xtype = syc.xtype INNER JOIN
                     (SELECT name, id, xtype, typestat, xusertype, length, xprec, xscale, colid, xoffset, bitpos, reserved, colstat, cdefault, domain, number, colorder, autoval, offset, collationid, language, status, type, usertype, printfmt, prec, 
                                       scale, iscomputed, isoutparam, isnullable, collation, tdscollation
                      FROM     sys.syscolumns AS syc) AS skey ON skey.id = syo.id AND syc.name <> skey.name INNER JOIN
                     (SELECT COLUMN_NAME, TABLE_NAME
                      FROM     INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                      WHERE  (OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1) AND (TABLE_SCHEMA = 'dbo')) AS pKey ON pKey.COLUMN_NAME = skey.name AND 
                 syo.name = pKey.TABLE_NAME
WHERE (syo.xtype = 'U') AND (syt.name <> N'sysname') AND (syo.name not like N'sysdia%') and (syo.name = N'EntryDataDetails')

