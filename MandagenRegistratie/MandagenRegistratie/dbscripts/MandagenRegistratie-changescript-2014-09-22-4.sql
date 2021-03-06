/****** Object:  Table [dbo].[Configuration]    Script Date: 09/22/2014 13:19:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration](
	[ConfigurationName] [nvarchar](50) NOT NULL,
	[ConfigurationValue] [nvarchar](50) NULL,
 CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
(
	[ConfigurationName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Configuration] ([ConfigurationName], [ConfigurationValue]) VALUES (N'ForeignDatabase', N'MDR')


