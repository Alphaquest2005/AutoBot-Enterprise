USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[SessionSchedule]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SessionSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SesseionId] [int] NOT NULL,
	[RunDateTime] [datetime] NOT NULL,
	[ApplicationSettingId] [int] NULL,
	[ActionId] [int] NULL,
	[ParameterSetId] [int] NULL,
 CONSTRAINT [PK_SessionSchedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[SessionSchedule] ON 

INSERT [dbo].[SessionSchedule] ([Id], [SesseionId], [RunDateTime], [ApplicationSettingId], [ActionId], [ParameterSetId]) VALUES (4114, 40, CAST(N'2023-05-16T12:33:13.857' AS DateTime), NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[SessionSchedule] OFF
GO
ALTER TABLE [dbo].[SessionSchedule] ADD  CONSTRAINT [DF_SessionSchedule_RunDateTime]  DEFAULT (getdate()) FOR [RunDateTime]
GO
ALTER TABLE [dbo].[SessionSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SessionSchedule_ParameterSet] FOREIGN KEY([ParameterSetId])
REFERENCES [dbo].[ParameterSet] ([Id])
GO
ALTER TABLE [dbo].[SessionSchedule] CHECK CONSTRAINT [FK_SessionSchedule_ParameterSet]
GO
ALTER TABLE [dbo].[SessionSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SessionSchedule_Sessions] FOREIGN KEY([SesseionId])
REFERENCES [dbo].[Sessions] ([Id])
GO
ALTER TABLE [dbo].[SessionSchedule] CHECK CONSTRAINT [FK_SessionSchedule_Sessions]
GO
