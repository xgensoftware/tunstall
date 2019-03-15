declare @TempLog table (
	ID					bigint
	,Table_Id			bigint
	,Update_Type		char(1)
)

insert into @TempLog(ID,Table_Id)
SELECT
      ID
      ,TABLE_ID      
  FROM [KS_EtlChangeLog]  
  where is_Processed = 0  
  and Table_name = 'SubscriberResponder'
  order by id asc
  
  

SELECT distinct
	R.Subscriber_ID, R.SubscriberResponder_ID, R.RelationCode, CASE WHEN CHARINDEX(' ', R.[ResponderName]) = 0 THEN [ResponderName] WHEN CHARINDEX(' ', 
                      R.[ResponderName]) = PATINDEX('% _[., ]%', R.[ResponderName]) THEN RTRIM(SUBSTRING(R.[ResponderName], 1, CHARINDEX(' ', R.[ResponderName]) + 2)) 
                      ELSE SUBSTRING(R.[ResponderName], 1, CHARINDEX(' ', R.[ResponderName])) END AS firstname, CASE WHEN CHARINDEX(' ', R.[ResponderName]) 
                      = 0 THEN '' WHEN CHARINDEX(' ', R.[ResponderName]) = PATINDEX('% _[., ]%', R.[ResponderName]) THEN LTRIM(SUBSTRING(R.[ResponderName], CHARINDEX(' ', 
                      R.[ResponderName]) + 3, 60)) ELSE SUBSTRING(R.[ResponderName], CHARINDEX(' ', R.[ResponderName]) + 1, 60) END AS lastname, 
                      CASE WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 'AMB') AND LEN([dbo].[fnRemoveNonNumChar](R.[HomePhone])) = 3 AND 
                      LEFT([dbo].[fnRemoveNonNumChar](R.[HomePhone]), 3) = '911' THEN '1' + [dbo].[fnRemoveNonNumChar](R.[HomePhone]) WHEN R.RelationCode IN ('FIR', 'POL', 
                      'AMB1', 'AMB') AND LEN([dbo].[fnRemoveNonNumChar](R.[HomePhone])) = 10 THEN '1' + [dbo].[fnRemoveNonNumChar](R.[HomePhone]) 
                      WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 'AMB') AND LEN([dbo].[fnRemoveNonNumChar](R.[HomePhone])) > 10 AND 
                      LEFT([dbo].[fnRemoveNonNumChar](R.[HomePhone]), 3) <> '911' THEN '1' + LEFT([dbo].[fnRemoveNonNumChar](R.[HomePhone]), 10) WHEN R.RelationCode IN ('FIR', 
                      'POL', 'AMB1', 'AMB') AND LEN([dbo].[fnRemoveNonNumChar](R.[HomePhone])) > 10 AND LEFT([dbo].[fnRemoveNonNumChar](R.[HomePhone]), 3) 
                      = '911' THEN '1' + SUBSTRING([dbo].[fnRemoveNonNumChar](R.[HomePhone]), 3, 10) WHEN R.RelationCode NOT IN ('FIR', 'POL', 'AMB1', 'AMB') AND 
                      LEN([dbo].[fnRemoveNonNumChar](R.[HomePhone])) = 10 THEN '1' + [dbo].[fnRemoveNonNumChar](R.[HomePhone]) WHEN R.RelationCode NOT IN ('FIR', 'POL', 
                      'AMB1', 'AMB') AND LEFT([dbo].[fnRemoveNonNumChar](R.[HomePhone]), 3) <> '911' AND LEN([dbo].[fnRemoveNonNumChar](R.[HomePhone])) 
                      > 10 THEN '1' + LEFT([dbo].[fnRemoveNonNumChar](R.[HomePhone]), 10) ELSE '' END AS Phone_1, CASE WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 'AMB') AND 
                      LEN([dbo].[fnRemoveNonNumChar](R.[WorkPhone])) = 3 AND LEFT([dbo].[fnRemoveNonNumChar](R.[WorkPhone]), 3) 
                      = '911' THEN '1' + [dbo].[fnRemoveNonNumChar](R.[WorkPhone]) WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 'AMB') AND 
                      LEN([dbo].[fnRemoveNonNumChar](R.[WorkPhone])) = 10 THEN '1' + [dbo].[fnRemoveNonNumChar](R.[WorkPhone]) WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 
                      'AMB') AND LEN([dbo].[fnRemoveNonNumChar](R.[WorkPhone])) > 10 AND LEFT([dbo].[fnRemoveNonNumChar](R.[WorkPhone]), 3) 
                      <> '911' THEN '1' + LEFT([dbo].[fnRemoveNonNumChar](R.[WorkPhone]), 10) WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 'AMB') AND 
                      LEN([dbo].[fnRemoveNonNumChar](R.[WorkPhone])) > 10 AND LEFT([dbo].[fnRemoveNonNumChar](R.[WorkPhone]), 3) 
                      = '911' THEN '1' + SUBSTRING([dbo].[fnRemoveNonNumChar](R.[WorkPhone]), 3, 10) WHEN R.RelationCode NOT IN ('FIR', 'POL', 'AMB1', 'AMB') AND 
                      LEN([dbo].[fnRemoveNonNumChar](R.[WorkPhone])) = 10 THEN '1' + [dbo].[fnRemoveNonNumChar](R.[WorkPhone]) WHEN R.RelationCode NOT IN ('FIR', 'POL', 
                      'AMB1', 'AMB') AND LEFT([dbo].[fnRemoveNonNumChar](R.[WorkPhone]), 3) <> '911' AND LEN([dbo].[fnRemoveNonNumChar](R.[WorkPhone])) 
                      > 10 THEN '1' + LEFT([dbo].[fnRemoveNonNumChar](R.[WorkPhone]), 10) ELSE '' END AS Phone_2, CASE WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 'AMB') AND 
                      LEN([dbo].[fnRemoveNonNumChar](R.[CellPhone])) = 3 AND LEFT([dbo].[fnRemoveNonNumChar](R.[CellPhone]), 3) 
                      = '911' THEN '1' + [dbo].[fnRemoveNonNumChar](R.[CellPhone]) WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 'AMB') AND 
                      LEN([dbo].[fnRemoveNonNumChar](R.[CellPhone])) = 10 THEN '1' + [dbo].[fnRemoveNonNumChar](R.[CellPhone]) WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 
                      'AMB') AND LEN([dbo].[fnRemoveNonNumChar](R.[CellPhone])) > 10 AND LEFT([dbo].[fnRemoveNonNumChar](R.[CellPhone]), 3) 
                      <> '911' THEN '1' + LEFT([dbo].[fnRemoveNonNumChar](R.[CellPhone]), 10) WHEN R.RelationCode IN ('FIR', 'POL', 'AMB1', 'AMB') AND 
                      LEN([dbo].[fnRemoveNonNumChar](R.[CellPhone])) > 10 AND LEFT([dbo].[fnRemoveNonNumChar](R.[CellPhone]), 3) 
                      = '911' THEN '1' + SUBSTRING([dbo].[fnRemoveNonNumChar](R.[CellPhone]), 3, 10) WHEN R.RelationCode NOT IN ('FIR', 'POL', 'AMB1', 'AMB') AND 
                      LEN([dbo].[fnRemoveNonNumChar](R.[CellPhone])) = 10 THEN '1' + [dbo].[fnRemoveNonNumChar](R.[CellPhone]) WHEN R.RelationCode NOT IN ('FIR', 'POL', 'AMB1', 
                      'AMB') AND LEFT([dbo].[fnRemoveNonNumChar](R.[CellPhone]), 3) <> '911' AND LEN([dbo].[fnRemoveNonNumChar](R.[CellPhone])) 
                      > 10 THEN '1' + LEFT([dbo].[fnRemoveNonNumChar](R.[CellPhone]), 10) ELSE '' END AS Phone_3, R.CallOrder + 100 AS Priority, CASE WHEN ISNULL(R.[Comments], '')
                       NOT IN ('', '--', ' ') THEN 'Y' ELSE 'N' END AS Notes_YN, 'A' AS AVAILABILITY,MT.TYPE_REF, R.NVIOrder AS Email_Address
                       ,coalesce(rc.CONTACT_DEF,-1) as [Contact_def]
                       ,sr.LOCATION_REF as [LOCATION_DEF]
                       ,sr.RESIDENT_DEF
                       ,a.[AUTHORITY_REF]
					   ,r.HASKEY as [Keyholder_YN]
                       ,'N' as [NOK_YN]
                       ,2 as [Entity_Type]
                       ,s.Agency_Id
					   ,r.Deleted
FROM @TempLog l         
inner join dbo.KS_SubscriberResponder AS R WITH (nolock) on R.SubscriberResponder_id = l.Table_Id
INNER JOIN dbo.KS_Subscriber AS S WITH (nolock) ON R.Subscriber_ID = S.Subscriber_ID 
INNER JOIN dbo.MAP_RELATION_CODE_CONTACT_TYPE AS MT WITH (nolock) ON R.RelationCode = MT.Code 
left join dbo.MAP_RESPONDER_CONTACTS rc with (nolock) on r.SubscriberResponder_id = rc.SubscriberResponder_ID
join MAP_SUBSCRIBER_RESIDENT sr with (nolock) on sr.Subscriber_ID = s.Subscriber_Id
inner join [dbo].[MAP_AGENCY_AUTHORITY] a  with (nolock) on S.[Agency_ID] = a.AGENCY_ID 
where MT.TYPE_REF IS NOT NULL;

select * from @TempLog;