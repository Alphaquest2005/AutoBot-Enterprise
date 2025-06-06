USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[FileTypeContacts]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileTypeContacts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileTypeId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
 CONSTRAINT [PK_FileTypeContacts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[FileTypeContacts]  WITH CHECK ADD  CONSTRAINT [FK_FileTypeContacts_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([Id])
GO
ALTER TABLE [dbo].[FileTypeContacts] CHECK CONSTRAINT [FK_FileTypeContacts_Contacts]
GO
ALTER TABLE [dbo].[FileTypeContacts]  WITH NOCHECK ADD  CONSTRAINT [FK_FileTypeContacts_FileTypes] FOREIGN KEY([FileTypeId])
REFERENCES [dbo].[FileTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileTypeContacts] CHECK CONSTRAINT [FK_FileTypeContacts_FileTypes]
GO
