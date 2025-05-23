USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[OCR-ChildPart]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCR-ChildPart](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentPartId] [int] NOT NULL,
	[ChildPartId] [int] NOT NULL,
 CONSTRAINT [PK_OCR-ChildPart] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[OCR-ChildPart] ON 

INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (4, 4, 5)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (6, 4, 6)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (7, 4, 7)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (14, 1017, 1020)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (20, 1032, 1033)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (21, 1034, 1035)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (26, 1028, 1030)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (27, 1036, 1037)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (28, 1036, 1038)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (29, 1017, 1039)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (30, 1040, 1041)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (31, 1040, 1042)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (32, 1043, 1044)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (33, 1043, 1045)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (36, 1046, 1047)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (37, 1046, 1048)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (38, 1051, 1052)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (39, 1051, 1053)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (40, 1054, 1055)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (41, 1054, 1056)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (42, 1057, 1058)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (43, 1057, 1059)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (44, 1060, 1061)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (45, 1060, 1062)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (46, 1063, 1064)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (47, 1063, 1065)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (48, 1066, 1067)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (49, 1066, 1068)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (50, 1069, 1070)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (51, 1069, 1071)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (52, 1072, 1073)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (53, 1072, 1074)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (54, 1075, 1076)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (55, 1075, 1077)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (56, 1078, 1079)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (57, 1078, 1080)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (58, 1084, 1085)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (59, 1084, 1086)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (60, 1087, 1088)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (61, 1087, 1089)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (62, 1090, 1091)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (63, 1092, 1093)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (64, 1092, 1094)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (65, 1095, 1096)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (66, 1095, 1097)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (67, 1098, 1099)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (69, 1098, 1100)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (70, 1101, 1102)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (71, 1101, 1103)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (72, 1104, 1105)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (74, 1104, 1106)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (76, 1107, 1109)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (77, 1110, 2260)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (78, 1113, 1114)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (79, 1113, 1115)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (80, 1116, 1118)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (82, 1120, 1103)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (83, 1121, 1122)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (84, 1121, 1123)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (86, 1125, 1126)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (87, 1125, 1127)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (88, 1049, 1050)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (89, 1049, 1128)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (90, 1129, 1130)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (91, 1129, 1132)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (93, 1133, 1134)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (94, 1133, 1136)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (96, 1137, 1138)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (97, 1137, 1139)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (98, 1140, 1141)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (99, 1140, 1142)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (101, 1144, 1145)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (102, 1144, 1146)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (104, 1148, 1149)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (105, 1148, 1150)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (106, 1151, 1152)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (108, 1154, 1155)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (109, 1154, 1156)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (110, 1157, 1158)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (111, 1157, 1159)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (112, 1160, 1162)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (113, 1164, 1165)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (114, 1166, 1167)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (115, 1166, 1175)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (116, 1169, 1170)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (117, 1169, 1171)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (119, 1057, 1173)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (121, 1175, 1168)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (122, 1176, 1177)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (123, 1176, 1178)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (124, 1179, 1180)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (125, 1180, 1181)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (126, 1182, 1183)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (127, 1182, 1184)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (128, 1185, 1186)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (129, 1185, 1187)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (130, 1188, 1190)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (131, 1191, 1192)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (132, 1191, 1193)
GO
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (133, 1196, 1197)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (134, 1196, 1198)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (135, 1194, 1195)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (136, 1199, 1200)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (137, 1199, 1201)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (138, 1202, 1203)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (139, 1202, 1204)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (140, 1205, 1206)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (141, 1205, 1207)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1140, 2205, 2206)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1141, 2205, 2207)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1142, 2208, 2209)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1143, 2208, 2210)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1144, 2211, 2212)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1145, 2211, 2213)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1146, 2214, 2216)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1147, 2217, 2218)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1148, 2217, 2219)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1149, 2220, 2221)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1150, 2220, 2222)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1151, 2223, 2224)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1152, 2223, 2225)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1153, 2226, 2227)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1154, 2226, 2228)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1155, 2229, 2230)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1156, 2229, 2231)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1157, 2232, 2233)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1158, 2232, 2234)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1159, 2235, 2236)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1160, 2235, 2237)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1161, 2238, 2239)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1162, 2238, 2240)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1163, 1110, 2241)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1164, 2242, 2243)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1165, 2242, 2244)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1166, 1028, 2245)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1167, 2246, 2247)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1168, 2246, 2248)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1169, 2251, 2252)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1170, 2251, 2253)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1171, 2254, 2255)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1172, 2254, 2256)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1173, 2257, 2258)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1174, 2257, 2259)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1175, 2260, 1112)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1176, 2261, 2262)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1177, 2261, 2263)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1178, 2264, 2265)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1179, 2264, 2266)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1181, 2267, 2269)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1182, 2270, 2272)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1183, 2273, 2274)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1184, 2273, 2275)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1185, 1049, 2276)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1186, 1049, 2277)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1187, 2278, 2279)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1188, 2278, 2280)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1191, 2284, 2285)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1192, 2284, 2286)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1193, 1051, 2287)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1194, 2288, 2289)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1195, 2288, 2290)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1196, 2291, 2292)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1197, 2291, 2293)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1198, 2294, 2295)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1199, 2296, 2297)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1200, 2296, 2298)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1201, 2299, 2300)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1202, 2299, 2301)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1203, 2302, 2303)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1204, 2302, 2304)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1205, 2305, 2306)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1206, 2305, 2307)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1207, 2308, 2309)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1208, 2308, 2310)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1209, 2311, 2312)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1210, 2311, 2313)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1211, 2314, 2315)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1212, 2314, 2316)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1213, 2318, 2319)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1214, 2318, 2320)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1215, 2321, 2322)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1216, 2321, 2323)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1217, 2324, 2325)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1219, 2327, 2328)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1220, 2329, 2330)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1221, 2331, 2332)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1222, 2331, 2333)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1223, 2334, 2335)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1224, 2336, 2337)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1225, 2338, 2339)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1226, 2340, 2341)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1227, 2344, 2345)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1231, 2350, 2351)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1232, 2352, 2353)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1233, 2354, 2355)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1234, 2356, 2357)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1235, 2358, 2359)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1236, 2360, 2361)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1237, 2362, 2363)
GO
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1238, 2364, 2365)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1239, 2366, 2367)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1240, 2368, 2369)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1241, 2370, 2371)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1242, 2372, 2373)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1243, 2374, 2375)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1244, 2376, 2377)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1245, 2378, 2379)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1247, 2380, 2381)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1248, 2382, 2383)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1249, 2384, 2385)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1250, 2386, 2387)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1251, 2388, 2389)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1252, 2390, 2391)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1253, 2392, 2393)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1254, 2394, 2395)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1255, 2396, 2397)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1256, 2398, 2399)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1257, 2400, 2401)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1258, 2402, 2403)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1259, 2404, 2405)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1260, 2406, 2407)
INSERT [dbo].[OCR-ChildPart] ([Id], [ParentPartId], [ChildPartId]) VALUES (1261, 1028, 2409)
SET IDENTITY_INSERT [dbo].[OCR-ChildPart] OFF
GO
ALTER TABLE [dbo].[OCR-ChildPart]  WITH CHECK ADD  CONSTRAINT [FK_OCR-ChildPart_OCR-Parts] FOREIGN KEY([ChildPartId])
REFERENCES [dbo].[OCR-Parts] ([Id])
GO
ALTER TABLE [dbo].[OCR-ChildPart] CHECK CONSTRAINT [FK_OCR-ChildPart_OCR-Parts]
GO
ALTER TABLE [dbo].[OCR-ChildPart]  WITH CHECK ADD  CONSTRAINT [FK_OCR-ChildPart_OCR-Parts1] FOREIGN KEY([ParentPartId])
REFERENCES [dbo].[OCR-Parts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OCR-ChildPart] CHECK CONSTRAINT [FK_OCR-ChildPart_OCR-Parts1]
GO
