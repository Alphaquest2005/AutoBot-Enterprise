update emails
 set emailid = Subject + N'--' + format(EmailDate, N'yyyy-MM-dd-HH:mm:ss')


UPDATE EntryData
SET         EmailId1 = Emails.EmailId
FROM    Emails INNER JOIN
                 EntryData ON Emails.EmailUniqueId = EntryData.EmailId

UPDATE AsycudaDocumentSet_Attachments
SET         EmailId = Emails.EmailId
FROM    Emails INNER JOIN
                 AsycudaDocumentSet_Attachments ON Emails.EmailUniqueId = AsycudaDocumentSet_Attachments.EmailUniqueId

select * from AsycudaDocumentSet_Attachments
select * from entrydata

select max(len(emailid)) from emails