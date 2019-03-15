declare @TempLog table (
	ID					bigint
	,SMI_ID		varchar(25)
);


insert into @TempLog(ID,SMI_ID)
SELECT  
      ID
      ,TABLE_ID
  FROM [KS_EtlChangeLog]  
  where is_Processed = 0  
  and Table_name = 'SubscriberMedicalInfo'
  order by id asc; 

 


SELECT 
	RESIDENT_REF
	,ROW_NUMBER() OVER(PARTITION BY RESIDENT_REF ORDER BY [SMI_ID]) AS KEYWORD_NO
	,[KEYWORD_DEF]
	,KEYWORD_TEXT
	,Subscriber_Id
	,Agency_Id
FROM 
(
	SELECT S.SUBSCRIBER_ID,
		0 AS SMI_ID
		,r.RESIDENT_DEF As RESIDENT_REF
		,1 AS KEYWORD_NO
		,100001156 As KEYWORD_DEF
		,CAST(S.[SpecialInst] AS VARCHAR(1024)) AS KEYWORD_TEXT
		,s.Agency_Id
	FROM [dbo].[KS_Subscriber] s WITH(NOLOCK) 
	inner join [dbo].[MAP_SUBSCRIBER_RESIDENT] r WITH(NOLOCK) on s.[Subscriber_ID] = r.Subscriber_ID 
	WHERE s.Subscriber_Id in (
		select mi.subscriber_id
		from @TempLog l
		join [dbo].[KS_SubscriberMedicalInfo] mi on mi.SMI_ID = l.SMI_ID
	)
	union all
	SELECT  S.SUBSCRIBER_ID,
		S.[SMI_ID],
		r.RESIDENT_DEF As RESIDENT_REF,
		S.[SMI_ID] AS KEYWORD_NO,
		K.[KEYWORD_DEF],
		CAST(k.text AS VARCHAR(1024)) as KEYWORD_TEXT
		,ss.Agency_Id
	FROM [dbo].[KS_Subscriber] ss WITH(NOLOCK) 
	inner join [dbo].[KS_SubscriberMedicalInfo] s WITH(NOLOCK) on ss.Subscriber_ID = s.Subscriber_ID  
	inner join [dbo].[MAP_SUBSCRIBER_RESIDENT] r WITH(NOLOCK) on s.[Subscriber_ID] = r.Subscriber_ID 
	inner join [dbo].[MAP_SUBMEDINFO_KEYWORD] k WITH(NOLOCK) on k.code = s.id and k.type = s.[Type] 
	where k.KEYWORD_DEF <> 0 
	AND K.DELETED_YN <> 'Y'
	AND K.Deleted <> 'Y'
	AND ss.Subscriber_Id in(
	select mi.subscriber_id
	from @TempLog l
	join [dbo].[KS_SubscriberMedicalInfo] mi on mi.SMI_ID = l.SMI_ID
)
) AS KEYWORDS;

select * from @TempLog;