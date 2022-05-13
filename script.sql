USE [TOGIRRO_Kontrolnye_meropriyatiya]
GO
/****** Object:  Table [dbo].[Answer]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Answer](
	[Answer_ID] [int] IDENTITY(1,1) NOT NULL,
	[Question_FK] [int] NOT NULL,
	[RightAnswer] [varchar](100) NOT NULL,
	[Score] [smallint] NOT NULL,
 CONSTRAINT [PK_Answer_1] PRIMARY KEY CLUSTERED 
(
	[Answer_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AnswerCharacteristic]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnswerCharacteristic](
	[AnswerCharacteristic_ID] [int] IDENTITY(1,1) NOT NULL,
	[Subject_FK] [int] NOT NULL,
	[TaskNumber] [smallint] NOT NULL,
	[Criterion] [smallint] NULL,
	[AllowedChars] [varchar](200) NULL,
	[QuestionType_FK] [int] NOT NULL,
	[CheckType_FK] [int] NOT NULL,
	[MaxScore] [smallint] NOT NULL,
 CONSTRAINT [PK_AnswerType] PRIMARY KEY CLUSTERED 
(
	[AnswerCharacteristic_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BlankType]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlankType](
	[BlankType_ID] [int] IDENTITY(1,1) NOT NULL,
	[BlankTypeName] [varchar](100) NOT NULL,
	[ControlEvent_FK] [int] NOT NULL,
	[BlankFile] [varchar](200) NULL,
 CONSTRAINT [PK_BlankType] PRIMARY KEY CLUSTERED 
(
	[BlankType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BlankType-Subject]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlankType-Subject](
	[BTS_ID] [int] IDENTITY(1,1) NOT NULL,
	[BlankType_FK] [int] NOT NULL,
	[Subject_FK] [int] NOT NULL,
	[Pattern_R] [varchar](50) NULL,
	[Pattern_AB] [varchar](50) NULL,
	[Pattern_C] [varchar](50) NULL,
	[Pattern_P] [varchar](50) NULL,
	[Pattern_RU] [varchar](50) NULL,
 CONSTRAINT [PK_BlankType-Subject] PRIMARY KEY CLUSTERED 
(
	[BTS_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CheckType]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CheckType](
	[CheckType_ID] [int] IDENTITY(1,1) NOT NULL,
	[CheckTypeName] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CheckType] PRIMARY KEY CLUSTERED 
(
	[CheckType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ControlEvent]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ControlEvent](
	[ControlEvent_ID] [int] IDENTITY(1,1) NOT NULL,
	[ControlEventName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ControlEvents] PRIMARY KEY CLUSTERED 
(
	[ControlEvent_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ErrorCount]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorCount](
	[ErrorCount_ID] [int] IDENTITY(1,1) NOT NULL,
	[Count] [tinyint] NOT NULL,
	[Score] [tinyint] NOT NULL,
	[AnswerCharacteristic_FK] [int] NOT NULL,
 CONSTRAINT [PK_ErrorCount] PRIMARY KEY CLUSTERED 
(
	[ErrorCount_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Question]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Question](
	[Question_ID] [int] IDENTITY(1,1) NOT NULL,
	[Variant_FK] [int] NOT NULL,
	[QuestionType_FK] [int] NOT NULL,
	[TaskNumber] [int] NOT NULL,
	[AnswerCharacteristic_FK] [int] NOT NULL,
 CONSTRAINT [PK_Answer] PRIMARY KEY CLUSTERED 
(
	[Question_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuestionType]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionType](
	[QuestionType_ID] [int] IDENTITY(1,1) NOT NULL,
	[QuestionTypeName] [varchar](100) NOT NULL,
 CONSTRAINT [PK_AnswerType_1] PRIMARY KEY CLUSTERED 
(
	[QuestionType_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScaleUnit]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScaleUnit](
	[ScaleUnit_ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstScore] [smallint] NOT NULL,
	[Mark] [smallint] NOT NULL,
	[SecondScore] [smallint] NOT NULL,
	[Subject_FK] [int] NOT NULL,
 CONSTRAINT [PK_ScaleUnit] PRIMARY KEY CLUSTERED 
(
	[ScaleUnit_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subject]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subject](
	[Subject_ID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectCode] [nchar](10) NULL,
	[EventCode] [smallint] NULL,
	[MinScore] [int] NULL,
	[SubjectName] [nchar](10) NOT NULL,
	[Description] [varchar](100) NULL,
	[ControlEvents_FK] [int] NOT NULL,
	[ProjectFolderPath] [nchar](10) NULL,
	[RegistrationFormName] [varchar](100) NULL,
	[AnswersForm1Name] [varchar](100) NULL,
	[AnswersForm2Name] [varchar](100) NULL,
	[LogFileName] [varchar](100) NULL,
	[IsMark] [bit] NULL,
 CONSTRAINT [PK_Subjects] PRIMARY KEY CLUSTERED 
(
	[Subject_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Variant]    Script Date: 05.05.2022 16:52:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Variant](
	[Variant_ID] [int] IDENTITY(1,1) NOT NULL,
	[Subject_FK] [int] NOT NULL,
	[VariantName] [varchar](4) NOT NULL,
	[VariantFIlePath] [varchar](200) NULL,
 CONSTRAINT [PK_Variant] PRIMARY KEY CLUSTERED 
(
	[Variant_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Answer]  WITH CHECK ADD  CONSTRAINT [FK_Answer_Question] FOREIGN KEY([Question_FK])
REFERENCES [dbo].[Question] ([Question_ID])
GO
ALTER TABLE [dbo].[Answer] CHECK CONSTRAINT [FK_Answer_Question]
GO
ALTER TABLE [dbo].[AnswerCharacteristic]  WITH CHECK ADD  CONSTRAINT [FK_AnswerCharacteristic_AnswerType] FOREIGN KEY([QuestionType_FK])
REFERENCES [dbo].[QuestionType] ([QuestionType_ID])
GO
ALTER TABLE [dbo].[AnswerCharacteristic] CHECK CONSTRAINT [FK_AnswerCharacteristic_AnswerType]
GO
ALTER TABLE [dbo].[AnswerCharacteristic]  WITH CHECK ADD  CONSTRAINT [FK_AnswerCharacteristic_CheckType] FOREIGN KEY([CheckType_FK])
REFERENCES [dbo].[CheckType] ([CheckType_ID])
GO
ALTER TABLE [dbo].[AnswerCharacteristic] CHECK CONSTRAINT [FK_AnswerCharacteristic_CheckType]
GO
ALTER TABLE [dbo].[AnswerCharacteristic]  WITH CHECK ADD  CONSTRAINT [FK_AnswerType_Subjects] FOREIGN KEY([Subject_FK])
REFERENCES [dbo].[Subject] ([Subject_ID])
GO
ALTER TABLE [dbo].[AnswerCharacteristic] CHECK CONSTRAINT [FK_AnswerType_Subjects]
GO
ALTER TABLE [dbo].[BlankType]  WITH CHECK ADD  CONSTRAINT [FK_BlankType_ControlEvents] FOREIGN KEY([ControlEvent_FK])
REFERENCES [dbo].[ControlEvent] ([ControlEvent_ID])
GO
ALTER TABLE [dbo].[BlankType] CHECK CONSTRAINT [FK_BlankType_ControlEvents]
GO
ALTER TABLE [dbo].[BlankType-Subject]  WITH CHECK ADD  CONSTRAINT [FK_BlankType-Subject_BlankType] FOREIGN KEY([BlankType_FK])
REFERENCES [dbo].[BlankType] ([BlankType_ID])
GO
ALTER TABLE [dbo].[BlankType-Subject] CHECK CONSTRAINT [FK_BlankType-Subject_BlankType]
GO
ALTER TABLE [dbo].[BlankType-Subject]  WITH CHECK ADD  CONSTRAINT [FK_BlankType-Subject_Subject] FOREIGN KEY([Subject_FK])
REFERENCES [dbo].[Subject] ([Subject_ID])
GO
ALTER TABLE [dbo].[BlankType-Subject] CHECK CONSTRAINT [FK_BlankType-Subject_Subject]
GO
ALTER TABLE [dbo].[ErrorCount]  WITH CHECK ADD  CONSTRAINT [FK_ErrorCount_AnswerCharacteristic] FOREIGN KEY([AnswerCharacteristic_FK])
REFERENCES [dbo].[AnswerCharacteristic] ([AnswerCharacteristic_ID])
GO
ALTER TABLE [dbo].[ErrorCount] CHECK CONSTRAINT [FK_ErrorCount_AnswerCharacteristic]
GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD  CONSTRAINT [FK_Answer_AnswerType] FOREIGN KEY([QuestionType_FK])
REFERENCES [dbo].[QuestionType] ([QuestionType_ID])
GO
ALTER TABLE [dbo].[Question] CHECK CONSTRAINT [FK_Answer_AnswerType]
GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD  CONSTRAINT [FK_Answer_Variant] FOREIGN KEY([Variant_FK])
REFERENCES [dbo].[Variant] ([Variant_ID])
GO
ALTER TABLE [dbo].[Question] CHECK CONSTRAINT [FK_Answer_Variant]
GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD  CONSTRAINT [FK_Question_AnswerCharacteristic] FOREIGN KEY([AnswerCharacteristic_FK])
REFERENCES [dbo].[AnswerCharacteristic] ([AnswerCharacteristic_ID])
GO
ALTER TABLE [dbo].[Question] CHECK CONSTRAINT [FK_Question_AnswerCharacteristic]
GO
ALTER TABLE [dbo].[ScaleUnit]  WITH CHECK ADD  CONSTRAINT [FK_ScaleUnit_Subject] FOREIGN KEY([Subject_FK])
REFERENCES [dbo].[Subject] ([Subject_ID])
GO
ALTER TABLE [dbo].[ScaleUnit] CHECK CONSTRAINT [FK_ScaleUnit_Subject]
GO
ALTER TABLE [dbo].[Subject]  WITH CHECK ADD  CONSTRAINT [FK_Subjects_ControlEvents] FOREIGN KEY([ControlEvents_FK])
REFERENCES [dbo].[ControlEvent] ([ControlEvent_ID])
GO
ALTER TABLE [dbo].[Subject] CHECK CONSTRAINT [FK_Subjects_ControlEvents]
GO
ALTER TABLE [dbo].[Variant]  WITH CHECK ADD  CONSTRAINT [FK_Variant_Subjects] FOREIGN KEY([Subject_FK])
REFERENCES [dbo].[Subject] ([Subject_ID])
GO
ALTER TABLE [dbo].[Variant] CHECK CONSTRAINT [FK_Variant_Subjects]
GO
