USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Departure_arrival_information]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Departure_arrival_information](
	[Departure_arrival_information_Id] [int] IDENTITY(1,1) NOT NULL,
	[Means_of_transport_Id] [int] NULL,
	[Identity] [nvarchar](50) NULL,
	[Nationality] [nvarchar](50) NULL,
 CONSTRAINT [PK_xcuda_Departure_arrival_information] PRIMARY KEY CLUSTERED 
(
	[Departure_arrival_information_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Departure_arrival_information]  WITH NOCHECK ADD  CONSTRAINT [FK_Means_of_transport_Departure_arrival_information] FOREIGN KEY([Means_of_transport_Id])
REFERENCES [dbo].[xcuda_Means_of_transport] ([Means_of_transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Departure_arrival_information] CHECK CONSTRAINT [FK_Means_of_transport_Departure_arrival_information]
GO
