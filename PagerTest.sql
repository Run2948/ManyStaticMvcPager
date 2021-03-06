USE [master]
GO
/****** Object:  Database [PagerTest]    Script Date: 2018-05-07 18:13:15 ******/
CREATE DATABASE [PagerTest] ON  PRIMARY 
( NAME = N'PagerTest', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQLMSSQL10_50.MSSQLSERVER\MSSQL\DATA\PagerTest.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'PagerTest_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\PagerTest_log.ldf' , SIZE = 3136KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [PagerTest] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PagerTest].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PagerTest] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PagerTest] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PagerTest] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PagerTest] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PagerTest] SET ARITHABORT OFF 
GO
ALTER DATABASE [PagerTest] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [PagerTest] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [PagerTest] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PagerTest] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PagerTest] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PagerTest] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PagerTest] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PagerTest] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PagerTest] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PagerTest] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PagerTest] SET  DISABLE_BROKER 
GO
ALTER DATABASE [PagerTest] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PagerTest] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PagerTest] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PagerTest] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PagerTest] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PagerTest] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PagerTest] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PagerTest] SET RECOVERY FULL 
GO
ALTER DATABASE [PagerTest] SET  MULTI_USER 
GO
ALTER DATABASE [PagerTest] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PagerTest] SET DB_CHAINING OFF 
GO
EXEC sys.sp_db_vardecimal_storage_format N'PagerTest', N'ON'
GO
USE [PagerTest]
GO
/****** Object:  StoredProcedure [dbo].[P_AspNetPage]    Script Date: 2018-05-07 18:13:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE  [dbo].[P_AspNetPage]
/*
nzperfect [no_mIss] 高效通用分页存储过程(双向检索) 2007.5.7   QQ:34813284
敬告：适用于单一主键或存在唯一值列的表或视图
ps:Sql语句为8000字节,调用时请注意传入参数及sql总长度不要超过指定范围
*/
@TableName VARCHAR(200),      --表名
@FieldList VARCHAR(2000),     --显示列名，如果是全部字段则为*
@PrimaryKey VARCHAR(100),     --单一主键或唯一值键
@Where VARCHAR(2000),         --查询条件 不含'where'字符，如id>10 and len(userid)>9
@Order VARCHAR(1000),         --排序 不含'order by'字符，如id asc,userid desc，必须指定asc或desc
--注意当@SortType=3时生效，记住一定要在最后加上主键，否则会让你比较郁闷
@SortType INT,                --排序规则 1:正序asc 2:倒序desc 3:多列排序方法
@RecorderCount INT,           --记录总数 0:会返回总记录
@PageSize INT,                --每页输出的记录数
@PageIndex INT,               --当前页数
@TotalCount INT OUTPUT,       --记返回总记录
@TotalPageCount INT OUTPUT    --返回总页数
AS
SET NOCOUNT ON
IF ISNULL(@TotalCount,'') = '' SET @TotalCount = 0
SET @Order = RTRIM(LTRIM(@Order))
SET @PrimaryKey = RTRIM(LTRIM(@PrimaryKey))
SET @FieldList = REPLACE(RTRIM(LTRIM(@FieldList)),' ','')
WHILE CHARINDEX(', ',@Order) > 0 OR CHARINDEX(' ,',@Order) > 0
BEGIN
SET @Order = REPLACE(@Order,', ',',')
SET @Order = REPLACE(@Order,' ,',',')
END
IF ISNULL(@TableName,'') = '' OR ISNULL(@FieldList,'') = ''
OR ISNULL(@PrimaryKey,'') = ''
OR @SortType < 1 OR @SortType >3
OR @RecorderCount   < 0 OR @PageSize < 0 OR @PageIndex < 0
BEGIN
PRINT('ERR_00')
RETURN
END
IF @SortType = 3
BEGIN
IF (UPPER(RIGHT(@Order,4))!=' ASC' AND UPPER(RIGHT(@Order,5))!=' DESC')
BEGIN PRINT('ERR_02') RETURN END
END
DECLARE @new_where1 VARCHAR(1000)
DECLARE @new_where2 VARCHAR(1000)
DECLARE @new_order1 VARCHAR(1000)
DECLARE @new_order2 VARCHAR(1000)
DECLARE @new_order3 VARCHAR(1000)
DECLARE @Sql VARCHAR(8000)
DECLARE @SqlCount NVARCHAR(4000)
IF ISNULL(@where,'') = ''
BEGIN
SET @new_where1 = ' '
SET @new_where2 = ' WHERE   '
END
ELSE
BEGIN
SET @new_where1 = ' WHERE ' + @where
SET @new_where2 = ' WHERE ' + @where + ' AND '
END
IF ISNULL(@order,'') = '' OR @SortType = 1   OR @SortType = 2
BEGIN
IF @SortType = 1
BEGIN
SET @new_order1 = ' ORDER BY ' + @PrimaryKey + ' ASC'
SET @new_order2 = ' ORDER BY ' + @PrimaryKey + ' DESC'
END
IF @SortType = 2
BEGIN
SET @new_order1 = ' ORDER BY ' + @PrimaryKey + ' DESC'
SET @new_order2 = ' ORDER BY ' + @PrimaryKey + ' ASC'
END
END
ELSE
BEGIN
SET @new_order1 = ' ORDER BY ' + @Order
END
IF @SortType = 3 AND   CHARINDEX(','+@PrimaryKey+' ',','+@Order)>0
BEGIN
SET @new_order1 = ' ORDER BY ' + @Order
SET @new_order2 = @Order + ','
SET @new_order2 = REPLACE(REPLACE(@new_order2,'ASC,','{ASC},'),'DESC,','{DESC},')
SET @new_order2 = REPLACE(REPLACE(@new_order2,'{ASC},','DESC,'),'{DESC},','ASC,')
SET @new_order2 = ' ORDER BY ' + SUBSTRING(@new_order2,1,LEN(@new_order2)-1)
IF @FieldList <> '*'
BEGIN
SET @new_order3 = REPLACE(REPLACE(@Order + ',','ASC,',','),'DESC,',',')
SET @FieldList = ',' + @FieldList
WHILE CHARINDEX(',',@new_order3)>0
BEGIN
IF CHARINDEX(SUBSTRING(','+@new_order3,1,CHARINDEX(',',@new_order3)),','+@FieldList+',')>0
BEGIN
SET @FieldList =
@FieldList + ',' + SUBSTRING(@new_order3,1,CHARINDEX(',',@new_order3))
END
SET @new_order3 =
SUBSTRING(@new_order3,CHARINDEX(',',@new_order3)+1,LEN(@new_order3))
END
SET @FieldList = SUBSTRING(@FieldList,2,LEN(@FieldList))
END
END
SET @SqlCount = 'SELECT @TotalCount=COUNT(*),@TotalPageCount=CEILING((COUNT(*)+0.0)/'
+ CAST(@PageSize AS VARCHAR)+') FROM ' + @TableName + @new_where1
IF @RecorderCount   = 0
BEGIN
EXEC SP_EXECUTESQL @SqlCount,N'@TotalCount INT OUTPUT,@TotalPageCount INT OUTPUT',
@TotalCount OUTPUT,@TotalPageCount OUTPUT
END
ELSE
BEGIN
SELECT @TotalCount = @RecorderCount
END
IF @PageIndex > CEILING((@TotalCount+0.0)/@PageSize)
BEGIN
SET @PageIndex =   CEILING((@TotalCount+0.0)/@PageSize)
END
IF @PageIndex = 1 OR @PageIndex >= CEILING((@TotalCount+0.0)/@PageSize)
BEGIN
IF @PageIndex = 1 --返回第一页数据
BEGIN
SET @Sql = 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM '
+ @TableName + @new_where1 + @new_order1
END
IF @PageIndex >= CEILING((@TotalCount+0.0)/@PageSize)   --返回最后一页数据
BEGIN
SET @Sql = 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM ('
+ 'SELECT TOP ' + STR(ABS(@PageSize*@PageIndex-@TotalCount-@PageSize))
+ ' ' + @FieldList + ' FROM '
+ @TableName + @new_where1 + @new_order2 + ' ) AS TMP '
+ @new_order1
END
END
ELSE
BEGIN
IF @SortType = 1   --仅主键正序排序
BEGIN
IF @PageIndex <= CEILING((@TotalCount+0.0)/@PageSize)/2   --正向检索
BEGIN
SET @Sql = 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM '
+ @TableName + @new_where2 + @PrimaryKey + ' > '
+ '(SELECT MAX(' + @PrimaryKey + ') FROM (SELECT TOP '
+ STR(@PageSize*(@PageIndex-1)) + ' ' + @PrimaryKey
+ ' FROM ' + @TableName
+ @new_where1 + @new_order1 +' ) AS TMP) '+ @new_order1
END
ELSE   --反向检索
BEGIN
SET @Sql = 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM ('
+ 'SELECT TOP ' + STR(@PageSize) + ' '
+ @FieldList + ' FROM '
+ @TableName + @new_where2 + @PrimaryKey + ' < '
+ '(SELECT MIN(' + @PrimaryKey + ') FROM (SELECT TOP '
+ STR(@TotalCount-@PageSize*@PageIndex) + ' ' + @PrimaryKey
+ ' FROM ' + @TableName
+ @new_where1 + @new_order2 +' ) AS TMP) '+ @new_order2
+ ' ) AS TMP ' + @new_order1
END
END
IF @SortType = 2   --仅主键反序排序
BEGIN
IF @PageIndex <= CEILING((@TotalCount+0.0)/@PageSize)/2   --正向检索
BEGIN
SET @Sql = 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM '
+ @TableName + @new_where2 + @PrimaryKey + ' < '
+ '(SELECT MIN(' + @PrimaryKey + ') FROM (SELECT TOP '
+ STR(@PageSize*(@PageIndex-1)) + ' ' + @PrimaryKey
+' FROM '+ @TableName
+ @new_where1 + @new_order1 + ') AS TMP) '+ @new_order1
END
ELSE   --反向检索
BEGIN
SET @Sql = 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM ('
+ 'SELECT TOP ' + STR(@PageSize) + ' '
+ @FieldList + ' FROM '
+ @TableName + @new_where2 + @PrimaryKey + ' > '
+ '(SELECT MAX(' + @PrimaryKey + ') FROM (SELECT TOP '
+ STR(@TotalCount-@PageSize*@PageIndex) + ' ' + @PrimaryKey
+ ' FROM ' + @TableName
+ @new_where1 + @new_order2 +' ) AS TMP) '+ @new_order2
+ ' ) AS TMP ' + @new_order1
END
END
IF @SortType = 3   --多列排序，必须包含主键，且放置最后，否则不处理
BEGIN
IF CHARINDEX(',' + @PrimaryKey + ' ',',' + @Order) = 0
BEGIN PRINT('ERR_02') RETURN END
IF @PageIndex <= CEILING((@TotalCount+0.0)/@PageSize)/2   --正向检索
BEGIN
SET @Sql = 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM ( '
+ 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM ( '
+ ' SELECT TOP ' + STR(@PageSize*@PageIndex) + ' ' + @FieldList
+ ' FROM ' + @TableName + @new_where1 + @new_order1 + ' ) AS TMP '
+ @new_order2 + ' ) AS TMP ' + @new_order1
END
ELSE   --反向检索
BEGIN
SET @Sql = 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM ( '
+ 'SELECT TOP ' + STR(@PageSize) + ' ' + @FieldList + ' FROM ( '
+ ' SELECT TOP ' + STR(@TotalCount-@PageSize *@PageIndex+@PageSize) + ' ' + @FieldList
+ ' FROM ' + @TableName + @new_where1 + @new_order2 + ' ) AS TMP '
+ @new_order1 + ' ) AS TMP ' + @new_order1
END
END
END
EXEC(@Sql)

GO
/****** Object:  Table [dbo].[Person]    Script Date: 2018-05-07 18:13:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Person](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Person] ON 

INSERT [dbo].[Person] ([Id], [Name]) VALUES (1, N'第一个人：A')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (2, N'第二个人：B')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (3, N'第三个人：C')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (4, N'第四个人：D')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (5, N'第五个人：E')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (6, N'第六个人：F')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (7, N'第七个人：G')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (8, N'第八个人：H')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (9, N'第九个人：I')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (10, N'第十个人：J')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (11, N'第十一个人：K')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (12, N'第十二个人：L')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (13, N'第十三个人：M')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (14, N'第十四个人：N')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (15, N'第十五个人：O')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (16, N'第十六个人：P')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (17, N'第十七个人：Q')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (18, N'第十八个人：R')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (19, N'第十九个人：S')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (20, N'第二十个人：T')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (21, N'第二十一个人：U')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (22, N'第二十二个人：V')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (23, N'第二十三个人：W')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (24, N'第二十四个人：X')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (25, N'第二十五个人：Y')
INSERT [dbo].[Person] ([Id], [Name]) VALUES (26, N'第二十六个人：Z')
SET IDENTITY_INSERT [dbo].[Person] OFF
USE [master]
GO
ALTER DATABASE [PagerTest] SET  READ_WRITE 
GO
